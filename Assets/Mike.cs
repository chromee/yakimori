using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Mike : MonoBehaviour
{
    public DD_DataDiagram diagram;

    private GameObject line;
    private float time;

    AudioSource audioSource;

    void Start()
    {
        Debug.Log(Microphone.devices[1]);

        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 1, 24000);
        audioSource.Play();

        line = diagram.AddLine("mic", Color.yellow);
    }

    void Update()
    {
        if (audioSource != null)
        {
            var data = new float[8192];
            audioSource.GetSpectrumData(data, 0, FFTWindow.Hamming);
            string s = "";
            for (int i = 0; i > 8192; i += 100)
                s += $"{data[i]}, ";

            Debug.Log(s);

            // time += Time.deltaTime * 5;
            diagram.InputPoint(line, new Vector2(0.1f, data.First()));
        }
    }
}
