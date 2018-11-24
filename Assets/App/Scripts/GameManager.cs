using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public Subject<Unit> GameStartStream = new Subject<Unit>();
    public Subject<Unit> GameEndStream = new Subject<Unit>();
    public Subject<Unit> GameRestartStream = new Subject<Unit>();

    public int Score;
    public int Time;

    void Start()
    {
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }
}
