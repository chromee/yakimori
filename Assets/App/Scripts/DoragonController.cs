using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoragonController : MonoBehaviour
{

    [SerializeField] Transform muzzle;
    [SerializeField] ParticleSystem flameParticle;
    [SerializeField] GameObject fireBall;
    [SerializeField] float fireBallSpeed;

    void Start()
    {
        flameParticle.Stop();
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
