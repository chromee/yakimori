using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class PlayerController : MonoBehaviour
{
    [SerializeField] protected GameObject dragon;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected ParticleSystem flameParticle;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameStartStream.Subscribe(_ => Init());
            GameManager.Instance.GameRestartStream.Subscribe(_ => Init());
        }
        else
        {
            Init();
        }
    }

    protected virtual void Init()
    {
    }

    public virtual void StartFireBreath()
    {
        animator.SetBool("IsFlaming", true);
    }

    public virtual void StopFireBreath()
    {
        animator.SetBool("IsFlaming", false);
        flameParticle.Stop();
    }

    public virtual void Fireball()
    {
        animator.SetTrigger("Fireball");
    }
}
