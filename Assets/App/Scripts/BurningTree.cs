using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BurningTree : MonoBehaviour
{
    [SerializeField] GameObject firePrefab;
    [SerializeField] SphereCollider collider;
    [SerializeField] AudioSource audioSource;
    [SerializeField] List<Transform> firePostions;
    [SerializeField] int burnPeriod;

    bool isBurned = false;
    Renderer[] renderers;

    void Start()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.GameRestartStream.Subscribe(_ =>
            {
                if (renderers == null) return;
                foreach (var render in renderers)
                {
                    foreach (var material in render.materials)
                    {
                        material.SetColor("_Color", new Color(1, 1, 1, 1));
                    }
                }
            });
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (!isBurned)
        {
            isBurned = true;
            Burn();
            if (GameManager.Instance != null)
                GameManager.Instance.Score.Value += 1;
            else
                Debug.LogError("GameManager is null");
        }
    }

    void Burn()
    {
        StartCoroutine(BurnLeave());
        StartCoroutine(BlackoutTree());
        audioSource.Play();
    }

    IEnumerator BurnLeave()
    {
        List<GameObject> fires = new List<GameObject>();
        foreach (var firePos in firePostions)
        {
            var fire = Instantiate(firePrefab);
            fire.transform.position = firePos.position;
            fire.transform.rotation = firePos.rotation;
            fires.Add(fire);
        }

        yield return new WaitForSeconds(burnPeriod - 2f);

        fires.ForEach(v => Destroy(v));
    }

    IEnumerator BlackoutTree()
    {
        renderers = this.GetComponentsInChildren<Renderer>();
        var c = 1.0f;
        var d = c / 100f;

        StartCoroutine(Fadeout());
        while (c > 0)
        {
            c -= d;
            foreach (var render in renderers)
            {
                foreach (var material in render.materials)
                {
                    if (material.name.Contains("Leave"))
                    {
                        material.SetColor("_Color", new Color(c, c, c, c));
                    }
                    else
                    {
                        material.SetColor("_Color", new Color(c, c, c, 1));
                    }
                }
            }
            yield return new WaitForSeconds(d * burnPeriod);
        }
        collider.enabled = false;
    }

    int volume = 100;
    IEnumerator Fadeout()
    {
        for (; volume > 0; volume--)
        {
            audioSource.volume = (float)volume / 100;
            yield return new WaitForSeconds(0.005f * burnPeriod);
        }
        volume = 0;
        audioSource.Stop();
    }
}
