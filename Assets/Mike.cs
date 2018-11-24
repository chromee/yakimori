using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mike : MonoBehaviour
{
    public DD_DataDiagram diagram;
    public int sampleLength;

    private GameObject line;
    private float time;

    AudioSource audioSource;

    void Start()
    {
        Debug.Log(Microphone.devices[1]);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 3, 24000);
        audioSource.Play();

        line = diagram.AddLine("mic", Color.yellow);
    }

    void Update()
    {
        if (audioSource != null)
        {
            var data = new float[sampleLength];
            audioSource.GetSpectrumData(data, 0, FFTWindow.Hamming);
            // string s = "";
            // for (int i = 0; i > 8192; i += 100)
            //     s += $"{data[i]}, ";
            // Debug.Log(data.Average());
            diagram.InputPoint(line, new Vector2(0.1f, data.Average() * 1000));
        }
    }
}
