using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class PlayerController : MonoBehaviour
{
    [SerializeField] protected GameObject dragon;
    [SerializeField] protected Animator animator;
    [SerializeField] protected Rigidbody rigid;
    [SerializeField] protected ParticleSystem flameParticle;

    [SerializeField] protected float liftSpeed;
    [SerializeField] protected float flyingHeight;
    [SerializeField] protected float flyingRange;


    [SerializeField] protected Collider flameTrigger;
    [SerializeField] protected float flameDelay;
    [SerializeField] protected AudioClip flameSoundClip;
    [SerializeField] protected AudioSource audioSource;

    protected bool isFlaming = false;

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
        isFlaming = false;

        RaycastHit hit;
        int layerMask = 1 << 9;
        var transformFixStream = this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                if (Physics.Raycast(transform.position, Vector3.down, out hit, 500f, layerMask))
                {
                    var distance = Vector3.Distance(transform.position, hit.point);
                    var diff = Mathf.Abs(flyingHeight - distance);
                    if (distance < flyingHeight - flyingRange)
                        transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.up * diff, Time.deltaTime * liftSpeed);
                    else if (distance > flyingHeight + flyingRange)
                        transform.position = Vector3.Lerp(transform.position, transform.position + Vector3.down * diff, Time.deltaTime * liftSpeed);
                }
                else
                    Debug.LogError("範囲外に出たかTerrainのレイヤーがGroundになってない");

                if (transform.eulerAngles.x > 0 || transform.eulerAngles.z > 0)
                {
                    transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                }

            });

        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameEndStream.Subscribe(_ =>
            {
                transformFixStream.Dispose();
                animator.SetFloat("Move Y", 0);
                animator.SetBool("IsFlaming", false);
                flameParticle.Stop();
            });
        }
    }

    public virtual void StartFireBreath()
    {
        animator.SetBool("IsFlaming", true);
        flameTrigger.enabled = true;
    }

    public virtual void StopFireBreath()
    {
        animator.SetBool("IsFlaming", false);
        flameTrigger.enabled = false;
        flameParticle.Stop();
    }

    public virtual void Fireball()
    {
        animator.SetTrigger("Fireball");
    }

    float audioVolume = 0;
    protected IEnumerator FireBreathSEFadein()
    {
        audioSource.clip = flameSoundClip;
        audioVolume = 0;
        for (; audioVolume < 100; audioVolume += 10)
        {
            if (!isFlaming)
            {
                StartCoroutine(FireBreathSEFadeout());
                yield break;
            }
            audioSource.volume = audioVolume / 100;
            yield return null;
        }
        audioSource.volume = 1;
    }

    protected IEnumerator FireBreathSEFadeout()
    {
        for (; audioVolume > 0; audioVolume--)
        {
            audioSource.volume = audioVolume / 100;
            yield return null;
        }
        audioVolume = 0;
        audioSource.Stop();
    }
}
