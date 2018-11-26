using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
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
                resultCommentText.text = "do you best";
            else if (0 < score && score <= 10)
                resultCommentText.text = "good";
            else if (10 < score && score <= 30)
                resultCommentText.text = "very good";
            else if (30 < score && score <= 50)
                resultCommentText.text = "special";
            else if (50 < score && score <= 80)
                resultCommentText.text = "great";
            else if (80 < score)
                resultCommentText.text = "excellent";

            resultPanel.gameObject.SetActive(true);
        });
    }

    public void OnRestartBtn()
    {
        SceneManager.LoadScene("App/Scenes/0_StartScene");
    }

}
