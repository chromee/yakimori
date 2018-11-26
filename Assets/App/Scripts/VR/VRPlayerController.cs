using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class VRPlayerController : PlayerController
{
    [SerializeField] Transform hmdTransform;
    [SerializeField] float headTrackingDetectThreshold;
    [SerializeField] float moveThreshold;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateThreshold;
    [SerializeField] float rotateSpeed;

    public ReactiveProperty<float> hmdMoveAmount = new ReactiveProperty<float>(0);

    protected override void Init()
    {
        base.Init();

        if (hmdTransform == null)
            hmdTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        flameTrigger.enabled = false;
        System.IDisposable moveStream = null;
        Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
         {
             hmdMoveAmount.Value = 0;

             moveStream = this.UpdateAsObservable()
                 .Where(__ => hmdMoveAmount.Value > moveThreshold)
                 .Subscribe(__ =>
                 {
                     transform.position += transform.forward * moveSpeed;
                     var z = ClampAngle(hmdTransform.localEulerAngles.z);
                     if (Mathf.Abs(z) > rotateThreshold)
                     {
                         transform.Rotate(transform.up, z * rotateSpeed * -1);
                     }
                 });
         });

        var hmdMoveStream = this.UpdateAsObservable()
            .Select(_ => hmdTransform.localPosition)
            .Buffer(2, 1)
            .Subscribe(v =>
            {
                if (v.Count < 2) return;
                var posDiff = v[1] - v[0];
                var localForward = transform.worldToLocalMatrix.MultiplyVector(hmdTransform.forward);
                var dotPosDiffAndCamForward = Vector3.Dot(localForward, posDiff);

                if (posDiff.magnitude * 1000 > headTrackingDetectThreshold)
                {
                    hmdMoveAmount.Value += dotPosDiffAndCamForward;
                }
                if (hmdMoveAmount.Value < 0)
                    hmdMoveAmount.Value = 0;
            });

        this.UpdateAsObservable()
            .Where(_ => Input.GetKeyDown(KeyCode.F1))
            .Subscribe(_ =>
            {
                hmdMoveAmount.Value = 0;
            });

        if (GameManager.Instance == null) return;
        GameManager.Instance.GameEndStream.Subscribe(_ =>
        {
            moveStream.Dispose();
            hmdMoveStream.Dispose();
            FireBreathSEFadeout();

            Observable.Timer(System.TimeSpan.FromSeconds(10)).Subscribe(__ => SceneManager.LoadScene("App/Scenes/0_StartScene"));
        });
    }

    float ClampAngle(float angle)
    {
        if (angle > 180)
            return angle = angle - 360;
        return angle;
    }

    public override void StartFireBreath()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameContinue.Value) return;
        isFlaming = true;
        animator.SetBool("IsFlaming", true);
        Observable.Timer(System.TimeSpan.FromSeconds(flameDelay)).Where(_ => isFlaming).Subscribe(_ =>
        {
            audioSource.Play();
            StartCoroutine(FireBreathSEFadein());
            flameParticle.Play();
            flameTrigger.enabled = true;
        });
    }

    public override void StopFireBreath()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameContinue.Value) return;
        isFlaming = false;
        animator.SetBool("IsFlaming", false);
        flameParticle.Stop();
        StartCoroutine(FireBreathSEFadeout());
        flameTrigger.enabled = false;
    }
}
