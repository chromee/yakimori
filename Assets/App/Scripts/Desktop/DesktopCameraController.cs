using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DesktopCameraController : MonoBehaviour
{
    [SerializeField] Transform target;
    [SerializeField] float correctPosSpeed = 2f;
    [SerializeField] float yMinLimit = -80;
    [SerializeField] float yMaxLimit = 80;
    [SerializeField] float rotationSensitivity = 1;
    [SerializeField] Vector3 positionOffset = new Vector3(0, 8, -10);

    Camera mainCamera;
    Transform cameraTransform;
    Vector3 currentOffset;
    float x, y;
    int layerMask = 1 << 9;


    void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        currentOffset = positionOffset;

        if (target == null)
            target = GameObject.FindGameObjectWithTag("CameraTarget").transform;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                RaycastHit hit;
                var dir = cameraTransform.position - target.position;
                if (Physics.Raycast(target.position, dir, out hit, positionOffset.magnitude, layerMask))
                {
                    var distance = Vector3.Distance(hit.point, target.position) * 0.8f;
                    currentOffset = Vector3.Lerp(currentOffset, currentOffset.normalized * distance, Time.deltaTime * correctPosSpeed);
                }
                else
                {
                    currentOffset = Vector3.Lerp(currentOffset, positionOffset, Time.deltaTime * correctPosSpeed);
                }
            });

        this.FixedUpdateAsObservable()
            .Subscribe(_ =>
            {
                var input = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
                x += input.x * rotationSensitivity;
                y = ClampAngle(y - input.y * rotationSensitivity, yMinLimit, yMaxLimit);
                LookPlayer();
            });
    }

    public void LookPlayer()
    {
        var rotation = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);
        var position = target.position + rotation * currentOffset;

        cameraTransform.position = position;
        cameraTransform.rotation = rotation;
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }


}
