using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using TMPro;

public class VRCameraController : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;
    [SerializeField] Transform IkTarget;
    [SerializeField] Transform cameraTarget;

    void Start()
    {
        if (cameraTarget == null)
            cameraTarget = GameObject.FindGameObjectWithTag("CameraTarget").transform;

        RaycastHit hit;
        int layerMask = 1 << 9 | 1 << 11;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                transform.position = cameraTarget.position;
                transform.rotation = cameraTarget.rotation;

                if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out hit, 500f, layerMask))
                {
                    IkTarget.position = hit.point;
                }
            });
    }

}
