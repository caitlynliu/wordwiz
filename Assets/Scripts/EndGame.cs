using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;

public class EndGame : MonoBehaviour
{
    [SerializeField] private Text score;
    [SerializeField] private Text scoreShadow;

    void Start()
    {
        Debug.Log("score "+PlayerPrefs.GetInt("ScoreNumPP"));
        score.text = PlayerPrefs.GetInt("ScoreNumPP").ToString();
        scoreShadow.text = PlayerPrefs.GetInt("ScoreNumPP").ToString();

    }
        public void restartGame()
    {
        PlayerPrefs.SetInt("ScoreNumPP", 0);
        PlayerPrefs.SetInt("numWrongPP", 0);
        PlayerPrefs.SetString("wordMistake1PP", "");
        PlayerPrefs.SetString("wordMistake2PP", "");
        PlayerPrefs.SetString("wordMistake3PP", "");
        PlayerPrefs.SetString("correct1PP", "");
        PlayerPrefs.SetString("correct2PP", "");
        PlayerPrefs.SetString("correct3PP", "");
        SceneManager.LoadScene("Game");
        

    }

}
