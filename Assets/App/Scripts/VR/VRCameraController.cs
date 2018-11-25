using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class VRCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] Transform hmdTransform;
    [SerializeField] float targetTrackingSpeed;
    [SerializeField] float headTrackingDetectThreshold;
    [SerializeField] float moveThreshold;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateThreshold;
    [SerializeField] float rotateSpeed;

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
                    transform.position += transform.forward * moveSpeed;
                    var z = ClampAngle(hmdTransform.localEulerAngles.z);
                    if (Mathf.Abs(z) > rotateThreshold)
                    {
                        transform.Rotate(transform.up, z * rotateSpeed * -1);
                    }
                });
        });

        this.UpdateAsObservable()
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
                    // if (vv != Vector3.zero)
                    //     devText.text = $"{moveDir}";
                    // else
                    //     Debug.Log("ZERO");
                }
            });
    }

    float ClampAngle(float angle)
    {
        if (angle > 180)
            return angle = angle - 360;
        return angle;
    }

}
