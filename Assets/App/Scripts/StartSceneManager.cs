using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class StartSceneManager : MonoBehaviour
{

    public void OnStart()
    {
        SceneManager.LoadScene("App/Scenes/1_PlayScene");
    }
}
