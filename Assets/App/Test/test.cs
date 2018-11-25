using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class test : MonoBehaviour
{

    public float v;
    void Update()
    {
        transform.Rotate(Vector3.up, v);
    }
}
