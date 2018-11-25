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

    public ReactiveProperty<int> Score;
    public ReactiveProperty<int> Time;
    public ReactiveProperty<bool> isGameContinue;

    [SerializeField] int GameTime = 60;
    [SerializeField] Transform playerStartPos;

    System.IDisposable timerStream;

    void Start()
    {
        Instance = this;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameStartStream.Subscribe(_ =>
        {
            Init();
        });

        GameEndStream.Subscribe(_ =>
        {
            timerStream.Dispose();
            isGameContinue.Value = false;
        });

        GameRestartStream.Subscribe(_ =>
        {
            Init();
        });

        Observable.Timer(System.TimeSpan.FromMilliseconds(1))
            .Subscribe(_ => GameStartStream.OnNext(Unit.Default));
    }

    void Init()
    {
        Score = new ReactiveProperty<int>(0);
        Time = new ReactiveProperty<int>(GameTime);
        isGameContinue = new ReactiveProperty<bool>(false);

        timerStream = Observable.Interval(System.TimeSpan.FromSeconds(1))
            .Where(_ => isGameContinue.Value)
            .Subscribe(_ =>
            {
                Time.Value -= 1;
                if (Time.Value == 0)
                    GameEndStream.OnNext(Unit.Default);
            });

        if (playerStartPos != null)
            GameObject.FindGameObjectWithTag("Player").transform.position = playerStartPos.position;
        isGameContinue.Value = true;
    }
}
