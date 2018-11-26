using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using RootMotion.FinalIK;

public class DoragonController : MonoBehaviour
{

    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem flameParticle;
    [SerializeField] GameObject fireBall;
    [SerializeField] float fireBallSpeed;

    [SerializeField] FABRIK fABRIK;

    [SerializeField] AudioSource audioSource;
    [SerializeField] float SeInterval;

    void Start()
    {
        flameParticle.Stop();

        if (audioSource == null)
            audioSource = GetComponent<AudioSource>();

        if (fABRIK != null && fABRIK.solver.target == null)
        {
            var IKTarget = GameObject.FindGameObjectWithTag("IKTarget");
            if (IKTarget != null)
                fABRIK.solver.target = IKTarget.transform;
            else
                Debug.LogError("not found IKTarget");
        }

        Observable.Interval(System.TimeSpan.FromSeconds(SeInterval)).Subscribe(_ =>
        {
            audioSource.Play();
        });
    }

    public void StartFrame()
    {
        flameParticle.Play();
    }

    public void StopFrame()
    {
        flameParticle.Stop();
    }

    public void Fireball()
    {
        var fb = Instantiate(fireBall);
        fb.transform.position = muzzle.position;
        fb.transform.rotation = muzzle.rotation;
        var fbRigid = fb.GetComponent<Rigidbody>();
        fbRigid.AddForce(fb.transform.forward * fireBallSpeed, ForceMode.Impulse);
    }
}
