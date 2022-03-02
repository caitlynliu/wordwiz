using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.SceneManagement;
using System;
using System.IO;

public class GameManager : MonoBehaviour
{
    private List<Question> questions = new List<Question>();
    private static List<Question> unansweredQuestions;

    private Question currentQuestion;

    [SerializeField] private Text questionText;
    [SerializeField] private Text option1;
    [SerializeField] private Text option2;
    [SerializeField] private Text option3;
    [SerializeField] private Text option4;
    [SerializeField] private Text option5;
    [SerializeField] private Text option6;
    [SerializeField] private Text score;
    [SerializeField] private Text wordMistake1;
    [SerializeField] private Text wordMistake2;
    [SerializeField] private Text wordMistake3;
    [SerializeField] private Text correct1;
    [SerializeField] private Text correct2;
    [SerializeField] private Text correct3;

    [SerializeField] private Animator buttonAni;
    [SerializeField] private Animator wizardAni;
    [SerializeField]
    private float timeBetweenQuestions = 1-2f;
    private static string correct = "True";
    private static string incorrect = "False";
    private int scoreNum;
    private int numWrong;

    /* Scene restarts after each question */

    void Start()
    {
        // Wizard Animation
        wizardAni.SetBool("isRun", true);
        
        // Store questions from csv file into list
        ReadQuestions();

        // Scene Setup - First launched
        if (unansweredQuestions == null) 
        {
            // Debug.Log("if 1");
            scoreNum = 0;
            score.text = scoreNum.ToString();
            numWrong = 0;
        }
        else 
        // Scene Setup - After each question refresh
        {
            // Debug.Log("if 2");
            scoreNum = PlayerPrefs.GetInt("ScoreNumPP");
            score.text = scoreNum.ToString();
            Debug.Log("score num: " + scoreNum);

            numWrong = PlayerPrefs.GetInt("numWrongPP");
            // Debug.Log("num wrong: " + numWrong);
            wordMistake1.text = PlayerPrefs.GetString("wordMistake1PP");
            wordMistake2.text = PlayerPrefs.GetString("wordMistake2PP");
            wordMistake3.text = PlayerPrefs.GetString("wordMistake3PP");
            correct1.text = PlayerPrefs.GetString("correct1PP");
            correct2.text = PlayerPrefs.GetString("correct2PP");
            correct3.text = PlayerPrefs.GetString("correct3PP");
        }
        
        // Load unasnwered questions
        if (unansweredQuestions == null || unansweredQuestions.Count == 0)
        {
            // Debug.Log("if 3");
            // Debug.Log(questions[0].question);
            // Debug.Log(questions[0].options[0].optionText);
            unansweredQuestions = questions;
        }
        SetCurrentQuestion();
        //Debug.Log(currentQuestion.question + " answer: " + currentQuestion.answer);

    }

    /* Retrieve, store, and display question from question bank */

    void SetCurrentQuestion()
    {
        // Get random question
        int randomQIndex = UnityEngine.Random.Range(0, unansweredQuestions.Count);
        // Debug.Log(randomQIndex);
        currentQuestion = unansweredQuestions[randomQIndex];

        //Debug.Log(currentQuestion.question);
        //Debug.Log(currentQuestion.options[0].optionText);

        // Display questions
        questionText.text = currentQuestion.question;
        option1.text = currentQuestion.options[0].optionText;
        option2.text = currentQuestion.options[1].optionText;
        option3.text = currentQuestion.options[2].optionText;
        option4.text = currentQuestion.options[3].optionText;
        option5.text = currentQuestion.options[4].optionText;
        option6.text = currentQuestion.options[5].optionText;
 
    }

    /* Next question */ 
    IEnumerator TransitionToNextQ()
    {
        unansweredQuestions.Remove(currentQuestion);
        yield return new WaitForSeconds(timeBetweenQuestions);

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    /* Retrieve and store question bank from csv file into question list */
    public void ReadQuestions()
    {
        StringReader reader = null;
        TextAsset csvFile = Resources.Load<TextAsset>("data");
        Debug.Log(csvFile.text);
        reader = new StringReader(csvFile.text);
        bool EOF = false;
        int qNum = 0;
        {
            while (!EOF)
            {
                Debug.Log("qNum " + qNum);
                string line = reader.ReadLine();
                if (line == null)
                {
                    EOF = true;
                    break;
                }
                var values = line.Split(',');
                Debug.Log("values 0 " + values[0].ToString());
                questions.Add(new Question());

                // Store question
                questions[qNum].question = values[0].ToString();

                // Store options
                for (int i = 0; i < 6; i++)
                {
                    Debug.Log("option num " + i);
                    Debug.Log("values " + values[i + 1].ToString());
                    questions[qNum].options[i] = new Option();
                    questions[qNum].options[i].optionText = values[i + 1].ToString();
                    questions[qNum].options[i].isTrue = false;
                }

                // Store true answer
                int correctAnswer = Int16.Parse(values[7].ToString());
                Debug.Log("correctAnswer: " + correctAnswer);
                questions[qNum].options[correctAnswer - 1].isTrue = true;
                qNum++;
            }

        }

    }

    /* User selects question */
    public void Selection(int select)
    {
        if (buttonAni.GetInteger("Selection") == 0)
        {
            // Animate
            buttonAni.SetInteger("Selection", select);
            //Debug.Log("select");

            // If user is correct
            if (currentQuestion.options[select - 1].isTrue)
            {
                wizardAni.SetTrigger("attack");
                // Debug.Log("CORRECT");
                // Animations
                buttonAni.SetTrigger(correct); // Button animation

                // Score increase
                scoreNum++;
                score.text = scoreNum.ToString();
            }
            // If user if incorrect
            else
            {
                wizardAni.SetTrigger("hurt");
                int correctI = 0;
                for (int i = 0; i < 6; i++)
                {
                    if (currentQuestion.options[i].isTrue)
                    {
                        int correct = i + 1;
                        correctI = i;
                        //Debug.Log("correction: "+i);
                        buttonAni.SetInteger("Correction", correct);
                        break;
                    }
                }
                //Debug.Log("INCORRECT!");

                // Animations
                buttonAni.SetTrigger(incorrect);


                // Display wrong words

                if (numWrong == 0)
                {
                    wordMistake1.text = "X     " + currentQuestion.options[select - 1].optionText;
                    correct1.text = "->     " + currentQuestion.options[correctI].optionText;
                    //Debug.Log("mistake 1 animation test");
                    // mistakeAni.SetInteger("numWrongAni", 1);
                }
                if (numWrong == 1)
                {
                    wordMistake2.text = "X     " + currentQuestion.options[select - 1].optionText;
                    correct2.text = "->     " + currentQuestion.options[correctI].optionText;
                    //mistakeAni.SetInteger("numWrongAni", 2);
                }
                if (numWrong == 2)
                {
                    wordMistake3.text = "X     " + currentQuestion.options[select - 1].optionText;
                    correct3.text = "->     " + currentQuestion.options[correctI].optionText;
                    //mistakeAni.SetInteger("numWrongAni", 3);
                    PlayerPrefs.SetInt("ScoreNumPP", scoreNum);
                    PlayerPrefs.SetString("wordMistake1PP", wordMistake1.text);
                    PlayerPrefs.SetString("wordMistake2PP", wordMistake2.text);
                    PlayerPrefs.SetString("wordMistake3PP", wordMistake3.text);
                    PlayerPrefs.SetString("correct1PP", correct1.text);
                    PlayerPrefs.SetString("correct2PP", correct2.text);
                    PlayerPrefs.SetString("correct3PP", correct3.text);
                    Debug.Log("Game Over");
                    StartCoroutine(DelaySceneLoad());

                }
                numWrong++;
            }
        }

        // Store scene
        PlayerPrefs.SetInt("ScoreNumPP", scoreNum);
        PlayerPrefs.SetString("wordMistake1PP", wordMistake1.text);
        PlayerPrefs.SetString("wordMistake2PP", wordMistake2.text);
        PlayerPrefs.SetString("wordMistake3PP", wordMistake3.text);
        PlayerPrefs.SetString("correct1PP", correct1.text);
        PlayerPrefs.SetString("correct2PP", correct2.text);
        PlayerPrefs.SetString("correct3PP", correct3.text);
        PlayerPrefs.SetInt("numWrongPP", numWrong);
        StartCoroutine(TransitionToNextQ());

    }

    /* Load End Game */
    IEnumerator DelaySceneLoad()
    {
        yield return new WaitForSeconds(0.2f);
        SceneManager.LoadScene("EndGame");

    }
 }

