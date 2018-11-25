﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class BurningTree : MonoBehaviour
{
    [SerializeField] GameObject firePrefab;
    [SerializeField] SphereCollider collider;
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

    void OnParticleCollision(GameObject obj)
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
}