using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float correctPosSpeed = 2f;
    [SerializeField] private float yMinLimit = -80;
    [SerializeField] private float yMaxLimit = 80;
    [SerializeField] private float rotationSensitivity;
    [SerializeField] private Vector3 positionOffset;

    private Camera mainCamera;
    private Transform cameraTransform;
    private Vector3 currentOffset;
    private float x, y;

    void Start()
    {
        mainCamera = Camera.main;
        cameraTransform = mainCamera.transform;
        currentOffset = positionOffset;

        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                RaycastHit hit;
                var dir = cameraTransform.position - target.position;
                if (Physics.Raycast(target.position, dir, out hit, positionOffset.magnitude))
                {
                    var distance = Vector3.Distance(hit.point, target.position) * 0.8f;
                    currentOffset = Vector3.Lerp(currentOffset, currentOffset.normalized * distance, Time.deltaTime * correctPosSpeed);
                }
                else
                {
                    currentOffset = Vector3.Lerp(currentOffset, positionOffset, Time.deltaTime * correctPosSpeed);
                }
            });

        this.UpdateAsObservable()
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
