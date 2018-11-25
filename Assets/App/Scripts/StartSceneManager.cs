using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartSceneManager : MonoBehaviour
{
    [SerializeField] string DesktopModeSceneName;
    [SerializeField] string VRModeSceneName;

    void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnDesktopModeStart()
    {
        SceneManager.LoadScene(DesktopModeSceneName);
    }
    public void OnVRModeStart()
    {
        SceneManager.LoadScene(VRModeSceneName);
    }
}
