using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class VRCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float targetTrackingSpeed;
    [SerializeField] float headTrackingDetectThreshold;
    [SerializeField] float moveThreshold;
    [SerializeField] float moveSpeed;

    [SerializeField] TextMeshProUGUI devText;

    void Start()
    {
        if (cameraTarget == null)
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;

        var headMoveDistance = 0f;
        Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(_ =>
        {
            headMoveDistance = 0;
            devText.text = $"{headMoveDistance}";

            this.UpdateAsObservable()
                .Where(__ => headMoveDistance > moveThreshold)
                .Subscribe(__ =>
                {
                    var dir = cameraTransform.forward;
                    dir.y = 0;
                    transform.position += dir.normalized * moveSpeed;
                });
        });

        this.UpdateAsObservable()
            .Select(_ => cameraTransform.localPosition)
            .Buffer(2, 1)
            .Subscribe(v =>
            {
                if (v.Count < 2) return;
                var posDiff = v[1] - v[0];
                var localForward = transform.worldToLocalMatrix.MultiplyVector(cameraTransform.forward);
                var dotPosDiffAndCamForward = Vector3.Dot(localForward, posDiff);

                if (posDiff.magnitude * 1000 > headTrackingDetectThreshold)
                {
                    headMoveDistance += dotPosDiffAndCamForward;
                    devText.text = $"{headMoveDistance}";
                    // if (vv != Vector3.zero)
                    //     devText.text = $"{moveDir}";
                    // else
                    //     Debug.Log("ZERO");
                }
            });
    }

}
