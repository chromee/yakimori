using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class VRPlayerController : PlayerController
{
    [SerializeField] Transform cameraParentTransform;
    [SerializeField] Transform cameraTransform;
    [SerializeField] float liftSpeed;
    [SerializeField] float flyingHeight;
    [SerializeField] float flyingRange;
    [SerializeField] Vector3 positionTrackingOffset;

    protected override void Init()
    {
        base.Init();

        if (cameraParentTransform == null)
            cameraParentTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;

        RaycastHit hit;
        int layerMask = 1 << 9;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 500f, layerMask))
                {
                    // var distance = Vector3.Distance(transform.position, hit.point);
                    // if (distance < flyingHeight - flyingRange)
                    //     cameraParentTransform.Translate(0, liftSpeed, 0);
                    // else if (distance > flyingHeight + flyingRange)
                    //     cameraParentTransform.Translate(0, -liftSpeed, 0);
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
    }

}
