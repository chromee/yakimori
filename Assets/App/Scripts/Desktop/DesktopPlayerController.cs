using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class DesktopPlayerController : PlayerController
{
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float liftSpeed;
    [SerializeField] float flyingHeight;
    [SerializeField] float flyingRange;
    [SerializeField] float angleToVelResistor;
    [SerializeField] float velToAngleResistor;
    // [SerializeField] float rollSpeed;
    // [SerializeField] float maxRoll;
    // [SerializeField] float minRoll;

    float currentMoveSpeed;

    [SerializeField] bool debugFlag = true;

    protected override void Init()
    {
        base.Init();

        currentMoveSpeed = moveSpeed;
        var moveStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                float dx = Input.GetAxis("Horizontal");
                float dy = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);

                var dxx = Mathf.Abs(dx) * angleToVelResistor;
                if (dxx > 1) dy /= dxx;
                var dyy = dy * velToAngleResistor;
                if (dyy > 1) dx /= dyy;

                transform.position += transform.forward * dy * currentMoveSpeed;
                transform.Rotate(Vector3.up, dx * rotateSpeed);

                if (Input.GetKey(KeyCode.Q) && dy < 0.01f)
                {
                    flyingHeight += liftSpeed / 2;
                }
                if (Input.GetKey(KeyCode.E) && dy < 0.01f)
                {
                    flyingHeight -= liftSpeed / 2;
                }

                // if (-0.1 < dx && dx < 0.1)
                // {
                //     dragon.transform.localEulerAngles = Vector3.Lerp(dragon.transform.localEulerAngles, Vector3.zero, Time.deltaTime);
                // }
                // else
                // {
                //     if (minRoll < dragon.transform.localEulerAngles.z && dragon.transform.localEulerAngles.z < maxRoll)
                //     {
                //         dragon.transform.Rotate(0, 0, dx * rollSpeed * -1);
                //     }
                // }

                animator.SetFloat("Move Y", dy / 2);
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


        var flameStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartFireBreath();
                }
                if (Input.GetMouseButtonUp(0))
                {
                    StopFireBreath();
                }
                if (Input.GetMouseButtonDown(1))
                {
                    Fireball();
                }
            });

        if (GameManager.Instance == null) return;
        GameManager.Instance.GameEndStream.Subscribe(_ =>
        {
            moveStream.Dispose();
            transformFixStream.Dispose();
            flameStream.Dispose();
            animator.SetFloat("Move Y", 0);
            animator.SetBool("IsFlaming", false);
            flameParticle.Stop();
        });
    }

    public override void StartFireBreath()
    {
        currentMoveSpeed /= 3;
        base.StartFireBreath();
    }

    public override void StopFireBreath()
    {
        currentMoveSpeed *= 3;
        base.StopFireBreath();
    }

    public override void Fireball()
    {
        base.Fireball();
    }

}
