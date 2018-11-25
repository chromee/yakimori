using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class VRCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;
    [SerializeField] float trackingSpeed;

    void Start()
    {
        if (cameraTarget == null)
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position = Vector3.Lerp(transform.position, cameraTarget.position, Time.deltaTime * trackingSpeed);
            });
    }

}
