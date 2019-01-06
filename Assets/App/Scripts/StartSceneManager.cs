using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UniRx;
using UniRx.Triggers;
using System.Linq;


public class StartSceneManager : MonoBehaviour
{
    [SerializeField] string playSceneName;
    [SerializeField] Slider slider;
    [SerializeField] AudioSource audioSource;
    [SerializeField] int sampleLength;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        audioSource.clip = Microphone.Start(Microphone.devices[0], true, 3, 24000);
        audioSource.Play();
        var spectrunData = new float[sampleLength];
        this.UpdateAsObservable()
            .Subscribe(_ =>
            {
                audioSource.GetSpectrumData(spectrunData, 0, FFTWindow.Hamming);
                var ave = spectrunData.Average() * 1000;
                slider.value = ave;
            });
    }

    public void OnDesktopModeStart()
    {
        GameManager.GameMode = GameMode.DesktopMode;
        SceneManager.LoadScene(playSceneName);
    }
    public void OnVRModeStart()
    {
        GameManager.GameMode = GameMode.VRMode;
        SceneManager.LoadScene(playSceneName);
    }
}
