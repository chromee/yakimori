using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class VRCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTarget;

    void Start()
    {
        if (cameraTarget == null)
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position = cameraTarget.position;
                transform.rotation = cameraTarget.rotation;
            });
    }

}
