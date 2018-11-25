using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartSceneManager : MonoBehaviour
{
    [SerializeField] string PlaySceneName;
    public void OnStart()
    {
        SceneManager.LoadScene(PlaySceneName);
    }
}
