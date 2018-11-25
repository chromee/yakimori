using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UniRx;

public class GuiManager : MonoBehaviour
{

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI timeText;

    [SerializeField] GameObject resultPanel;
    [SerializeField] TextMeshProUGUI resultScoreText;
    [SerializeField] TextMeshProUGUI resultCommentText;

    void Init()
    {
        GameManager.Instance.Score.Subscribe(v => scoreText.text = v.ToString());
        GameManager.Instance.Time.Subscribe(v => timeText.text = v.ToString());
        scoreText.transform.parent.gameObject.SetActive(true);
        timeText.transform.parent.gameObject.SetActive(true);
        resultPanel.gameObject.SetActive(false);
    }

    void Start()
    {
        GameManager.Instance.GameStartStream.Subscribe(_ => Init());
        GameManager.Instance.GameRestartStream.Subscribe(_ => Init());

        GameManager.Instance.GameEndStream.Subscribe(_ =>
        {
            scoreText.transform.parent.gameObject.SetActive(false);
            timeText.transform.parent.gameObject.SetActive(false);

            var score = GameManager.Instance.Score.Value;
            resultScoreText.text = score.ToString();
            if (0 == score)
                resultCommentText.text = "little dragooon";
            else if (0 < score && score <= 10)
                resultCommentText.text = "middle dragooon";
            else if (10 < score && score <= 20)
                resultCommentText.text = "ultra big";
            else if (20 < score && score <= 30)
                resultCommentText.text = "ngaaaaaaaaaaa";
            else if (30 < score && score <= 40)
                resultCommentText.text = "ieeeeeeeeeeee";
            else if (40 < score && score <= 50)
                resultCommentText.text = "uoooooooooooo";

            resultPanel.gameObject.SetActive(true);
        });
    }

    public void OnRestartBtn()
    {
        GameManager.Instance.GameRestartStream.OnNext(Unit.Default);
    }

}
