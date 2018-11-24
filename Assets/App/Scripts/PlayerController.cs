using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerController : MonoBehaviour
{
    [SerializeField] GameObject dragon;
    [SerializeField] Animator animator;
    [SerializeField] Rigidbody rigid;
    [SerializeField] ParticleSystem flameParticle;
    [SerializeField] float moveSpeed;
    [SerializeField] float rotateSpeed;
    [SerializeField] float liftSpeed;
    [SerializeField] float angleToVelResistor;
    [SerializeField] float velToAngleResistor;

    [SerializeField] float rollSpeed;
    [SerializeField] float maxRoll;
    [SerializeField] float minRoll;

    float currentMoveSpeed;

    void Start()
    {
        currentMoveSpeed = moveSpeed;
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                float dx = Input.GetAxis("Horizontal");
                float dy = Mathf.Clamp(Input.GetAxis("Vertical"), 0, 1);

                var dxx = Mathf.Abs(dx) * angleToVelResistor;
                if (dxx > 1) dy /= dxx;
                var dyy = dy * velToAngleResistor;
                if (dyy > 1) dx /= dyy;

                transform.position += transform.forward * dy * currentMoveSpeed;
                transform.Rotate(0, dx * rotateSpeed, 0);

                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Translate(0, liftSpeed, 0);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    transform.Translate(0, -liftSpeed, 0);
                }

                // if (velocity > 0.1 && angularVelocity > 0.1)
                // {
                //     if (minRoll < dragon.transform.localEulerAngles.z && dragon.transform.localEulerAngles.z < maxRoll)
                //     {
                //         dragon.transform.Rotate(0, 0, dx * rollSpeed * -1);

                //     }
                // }
                // else
                // {
                //     var v = new Vector3(0, dragon.transform.localEulerAngles.y, 0);
                //     dragon.transform.localEulerAngles = Vector3.Slerp(dragon.transform.localEulerAngles, v, Time.deltaTime);
                // }

                animator.SetFloat("Move Y", dy / 2);
            });


        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Input.GetMouseButtonDown(0))
                {
                    currentMoveSpeed /= 3;
                    animator.SetBool("IsFlaming", true);
                }
                if (Input.GetMouseButtonUp(0))
                {
                    currentMoveSpeed *= 3;
                    animator.SetBool("IsFlaming", false);
                    flameParticle.Stop();
                }
                if (Input.GetMouseButtonDown(1))
                    animator.SetTrigger("Fireball");
            });
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

    public void StartFrame()
    {

        Debug.Log("frame");
    }

    public void StopFrame()
    {

        Debug.Log("frame stop");
    }


}
