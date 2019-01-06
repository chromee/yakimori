using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BurningTree : MonoBehaviour
{
    [SerializeField] GameObject firePrefab;
    [SerializeField] SphereCollider collider;
    [SerializeField] AudioSource audioSource;
    [SerializeField] Transform firePostion;
    [SerializeField] int burnPeriod;
    [SerializeField] Vector3 fireScale;

    bool isBurned = false;
    Renderer[] renderers;
    GameObject fire;

    void Start()
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.GameRestartStream.Subscribe(_ =>
        {
            if (renderers == null) return;
            foreach (var render in renderers)
                foreach (var material in render.materials)
                    material.SetColor("_Color", new Color(1, 1, 1, 1));
        });
    }

    void OnTriggerEnter(Collider col)
    {
        if (isBurned) return;

        isBurned = true;
        StartCoroutine(BurnLeave());

        fire = Instantiate(firePrefab);
        fire.transform.position = firePostion.position;
        fire.transform.rotation = firePostion.rotation;
        fire.transform.localScale = fireScale;

        if (GameManager.Instance != null)
            GameManager.Instance.Score.Value += 1;
        else
            Debug.LogError("GameManager is null");
    }

    IEnumerator BurnLeave()
    {
        audioSource.Play();

        renderers = this.GetComponentsInChildren<Renderer>();
        var c = 1.0f;
        var d = c / 100f;
        while (c > 0)
        {
            c -= d;
            foreach (var render in renderers)
                foreach (var material in render.materials)
                    if (material.name.Contains("Leave"))
                        material.SetColor("_Color", new Color(c, c, c, c));
                    else
                        material.SetColor("_Color", new Color(c, c, c, 1));

            yield return new WaitForSeconds(d * burnPeriod);
        }
        collider.enabled = false;

        audioSource.Stop();
        Destroy(fire);
    }
}
