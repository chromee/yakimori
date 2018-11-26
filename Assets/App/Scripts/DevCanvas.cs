using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class DevCanvas : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI text;

    void Start()
    {
        var pc = GameObject.Find("Player_VR").GetComponent<VRPlayerController>();
        pc.hmdMoveAmount.Subscribe(v =>
        {
            text.text = $"{v}";
        });
    }

    // Update is called once per frame
    void Update()
    {

    }
}
