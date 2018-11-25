using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using System.Linq;

public class MicrophoneFireBreath : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] float fireBreathThreshold;
    [SerializeField] int sampleLength;

    bool isBreathing = false;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 3, 24000);
        audioSource.Play();


        var spectrunData = new float[sampleLength];
        this.UpdateAsObservable()
            .Where(_ => audioSource != null)
            .Subscribe(_ =>
            {
                audioSource.GetSpectrumData(spectrunData, 0, FFTWindow.Hamming);
                var ave = spectrunData.Average() * 1000;
                if (!isBreathing)
                {
                    if (ave > fireBreathThreshold)
                    {
                        playerController.StartFireBreath();
                        isBreathing = true;
                    }
                }
                else
                {
                    if (ave < fireBreathThreshold)
                    {
                        playerController.StopFireBreath();
                        isBreathing = false;
                    }
                }
            });
    }
}
