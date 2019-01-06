using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

public enum GameMode { DesktopMode, VRMode };
public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static GameMode GameMode = GameMode.VRMode;

    public Subject<Unit> GameStartStream = new Subject<Unit>();
    public Subject<Unit> GameEndStream = new Subject<Unit>();
    public Subject<Unit> GameRestartStream = new Subject<Unit>();

    public ReactiveProperty<int> Score;
    public ReactiveProperty<int> Time;
    public ReactiveProperty<bool> isGameContinue;

    [SerializeField] GameObject desktopModeParent;
    [SerializeField] GameObject vrModeParent;
    [SerializeField] int GameTime = 60;
    [SerializeField] bool IsHideCursor = false;

    [SerializeField] GameMode devGameMode;

    System.IDisposable timerStream;

    void Start()
    {
        Instance = this;

#if UNITY_EDITOR
        GameManager.GameMode = devGameMode;
#endif

        if (IsHideCursor)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        GameStartStream.Subscribe(_ =>
        {
            Init();
        });

        GameEndStream.Subscribe(_ =>
        {
            timerStream.Dispose();
            isGameContinue.Value = false;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        });

        GameRestartStream.Subscribe(_ =>
        {
            Init();
        });

        if (GameManager.GameMode == GameMode.DesktopMode)
            desktopModeParent.SetActive(true);
        else if (GameManager.GameMode == GameMode.VRMode)
            vrModeParent.SetActive(true);
        else
        {
            Debug.LogError("not found game mode.");
            return;
        }
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

        var startPositions = GameObject.FindGameObjectsWithTag("StartPosition");
        if (startPositions.Length > 0)
        {
            var startPos = startPositions[Random.Range(0, startPositions.Length)].transform;
            var player = GameObject.FindGameObjectWithTag("Player").transform;
            player.position = startPos.position;
            player.rotation = startPos.rotation;
        }
        isGameContinue.Value = true;
    }
}
