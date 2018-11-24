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
        GameManager.Instance.GameStartStream.Subscribe(_ => Init());
        GameManager.Instance.GameRestartStream.Subscribe(_ => Init());
    }

    void Init()
    {
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
                transform.Rotate(0, dx * rotateSpeed, 0);

                if (Input.GetKey(KeyCode.Q))
                {
                    transform.Translate(0, liftSpeed, 0);
                }
                if (Input.GetKey(KeyCode.E))
                {
                    transform.Translate(0, -liftSpeed, 0);
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


        var flameStream = this.UpdateAsObservable()
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

        GameManager.Instance.GameEndStream.Subscribe(_ =>
        {
            moveStream.Dispose();
            flameStream.Dispose();
            animator.SetFloat("Move Y", 0);
            animator.SetBool("IsFlaming", false);
            flameParticle.Stop();
        });
    }

    float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360) angle += 360;
        if (angle > 360) angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }

}
