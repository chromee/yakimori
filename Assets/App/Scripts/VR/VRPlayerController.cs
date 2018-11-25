using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [SerializeField] float liftSpeed;
    [SerializeField] float flyingHeight;
    [SerializeField] float flyingRange;
    [SerializeField] float flameDelay;
    [SerializeField] AudioClip flameSoundClip;
    [SerializeField] AudioSource audioSource;

    [SerializeField] TextMeshProUGUI devText;

    protected override void Init()
    {
        base.Init();

        if (hmdTransform == null)
            hmdTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        var headMoveDistance = 0f;
        System.IDisposable moveStream = null;
        Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
         {
             headMoveDistance = 0;
             devText.text = $"{headMoveDistance}";

             moveStream = this.UpdateAsObservable()
                 .Where(__ => headMoveDistance > moveThreshold)
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
                    headMoveDistance += dotPosDiffAndCamForward;
                    devText.text = $"{headMoveDistance}";
                }
            });

        RaycastHit hit;
        int layerMask = 1 << 9;
        var transformFixStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 500f, layerMask))
                {
                    var distance = Vector3.Distance(transform.position, hit.point);
                    if (distance < flyingHeight - flyingRange)
                        transform.Translate(0, liftSpeed, 0);
                    else if (distance > flyingHeight + flyingRange)
                        transform.Translate(0, -liftSpeed, 0);
                }
                else
                    Debug.LogError("範囲外に出たかTerrainのレイヤーがGroundになってない");

                if (transform.eulerAngles.x > 0 || transform.eulerAngles.z > 0)
                {
                    var x = Mathf.Lerp(transform.eulerAngles.x, 0, Time.deltaTime);
                    var z = Mathf.Lerp(transform.eulerAngles.z, 0, Time.deltaTime);
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                }

            });

        if (GameManager.Instance == null) return;
        GameManager.Instance.GameEndStream.Subscribe(_ =>
        {
            moveStream.Dispose();
            hmdMoveStream.Dispose();
            transformFixStream.Dispose();
            animator.SetFloat("Move Y", 0);
            animator.SetBool("IsFlaming", false);
            flameParticle.Stop();
            Fadeout();
        });
    }

    float ClampAngle(float angle)
    {
        if (angle > 180)
            return angle = angle - 360;
        return angle;
    }

    bool isFlaming = false;
    public override void StartFireBreath()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameContinue.Value) return;
        isFlaming = true;
        animator.SetBool("IsFlaming", true);
        Observable.Timer(System.TimeSpan.FromSeconds(flameDelay)).Where(_ => isFlaming).Subscribe(_ =>
        {
            audioSource.Play();
            StartCoroutine(Fadein());
            flameParticle.Play();
        });
    }

    public override void StopFireBreath()
    {
        if (GameManager.Instance != null && !GameManager.Instance.isGameContinue.Value) return;
        isFlaming = false;
        animator.SetBool("IsFlaming", false);
        flameParticle.Stop();
        StartCoroutine(Fadeout());
    }

    float volume = 0;
    IEnumerator Fadein()
    {
        audioSource.clip = flameSoundClip;
        volume = 0;
        for (; volume < 100; volume++)
        {
            if (!isFlaming)
            {
                StartCoroutine(Fadeout());
                yield break;
            }
            audioSource.volume = volume / 100;
            yield return null;
        }
        audioSource.volume = 1;
    }

    IEnumerator Fadeout()
    {
        for (; volume > 0; volume--)
        {
            audioSource.volume = volume / 100;
            yield return null;
        }
        volume = 0;
        audioSource.Stop();
    }
}
