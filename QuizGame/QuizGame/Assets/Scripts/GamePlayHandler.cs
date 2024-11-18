using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GamePlayHandler : MonoBehaviour
{
    public static GamePlayHandler Instance;
    public UIHandler uiHandler;
    public MultiPlayHandler multiPlayHandler;
    public GameData gameData;


    [Header("Game")]
    public int questionIndex;
    public int answered;
    public int correctAnswered;
    [HideInInspector] public Questions currentQuestion;
    [HideInInspector] public List<int> selectedAnswerIndices = new List<int>(); // List to store multiple selected answers
    [HideInInspector] public float currentTime;
    [HideInInspector] public float totalTime;
    [HideInInspector] public bool isAnswered;
    [HideInInspector] public bool allowMultipleSelections = false; // Flag to toggle multiple selection mode
    [HideInInspector] public int score;


    [Header("MultiPlayer")]
    public string otherPlayerName;
    public string otherPlayerRank;
    public int otherPlayerScore;
    public bool otherPlayerAnswered;

    private void Awake()
    {
        Instance = this;
        otherPlayerScore=0;
        otherPlayerAnswered=false;
    }

    private void Start()
    {
        if (GameManager.Instance.isMultiPlayer)
        {
            multiPlayHandler.gameObject.SetActive(true);
            // Multiplayer setup
         
            uiHandler.UpdatePlayer2Score(otherPlayerScore);
            uiHandler.player2Obj.SetActive(true);
        }
        else
        {
            multiPlayHandler.gameObject.SetActive(false);
            uiHandler.player2Obj.SetActive(false);
        }
        score = 0;
        answered = 0;
        correctAnswered = 0;
        questionIndex = -1;
        uiHandler.UpdatePlayer1Data();
        uiHandler.UpdatePlayer1Score(score);
        uiHandler.UpdateAnsweredFill(GetCorrectAnswerPercentage(), GetWrongAnswerPercentage());
        LoadRandomQuestion();
    }

    private void Update()
    {
        if (!isAnswered)
        {
            HandleTimer();
        }
    }

    public void LoadRandomQuestion(bool isRandom = false)
    {
        // Choose a random question from the questionsData array
        if (isRandom) questionIndex = Random.Range(0, GameManager.Instance.questions.Length);
        else
        {
            questionIndex++;
            if (questionIndex >= GameManager.Instance.questions.Length)
            {
                if(GameManager.Instance.isMultiPlayer)
                {
                    uiHandler.FinishMultiPlayer(gameData.userName, score.ToString(), otherPlayerName, otherPlayerScore.ToString(), GetWinStatus(score, otherPlayerScore));
                }
                else
                {
                    uiHandler.FinishLocal(gameData.userName, score.ToString());
                }
                questionIndex = 0;
                return;
            }
        }
        currentQuestion = GameManager.Instance.questions[questionIndex];

        currentTime = GameManager.Instance.useDefaultTime ? GameManager.Instance.defaultTime : currentQuestion.questionTime;
        totalTime = currentTime;
        selectedAnswerIndices.Clear(); // Clear selected answers
        allowMultipleSelections = false;
        isAnswered = false;
        uiHandler.DisplayQuestion(currentQuestion);
    }

    // Call this when an answer button is pressed
    public void OnAnswerSelected(int index)
    {
        if (isAnswered)
        {
            return;
        }

        // Add the selected answer to the list if multiple selections are allowed
        if (allowMultipleSelections)
        {
            if (!selectedAnswerIndices.Contains(index))
            {
                selectedAnswerIndices.Add(index);
                if (currentQuestion.questionImg == null)
                {
                    uiHandler.answerTxtOnlyBtnTxt[index].color = Color.blue;
                }
                else
                {
                    uiHandler.answerMultiModalBtnTxt[index].color = Color.blue;
                }
                // If 2 answers have been selected, check if either is correct
                if (selectedAnswerIndices.Count == 2)
                {
                    isAnswered = true;
                    CheckAnswer();
                }
            }
        }
        else
        {
            // Single selection
            selectedAnswerIndices.Clear();
            selectedAnswerIndices.Add(index);
            isAnswered = true;
            CheckAnswer();
        }
    }

    // Modified CheckAnswer to handle multiple selections
    private void CheckAnswer()
    {
        bool isCorrect = false;

        foreach (int index in selectedAnswerIndices)
        {
            if (index == currentQuestion.correctAnswer)
            {
                isCorrect = true;
                // Highlight the correct answer in green
                if (currentQuestion.questionImg == null)
                {
                    uiHandler.answerTxtOnlyBtnTxt[index].color = Color.green;
                }
                else
                {
                    uiHandler.answerMultiModalBtnTxt[index].color = Color.green;
                }
            }
            else
            {
                // Highlight incorrect answers in red
                if (currentQuestion.questionImg == null)
                {
                    uiHandler.answerTxtOnlyBtnTxt[index].color = Color.red;
                }
                else
                {
                    uiHandler.answerMultiModalBtnTxt[index].color = Color.red;
                }
            }
        }

        if (isCorrect)
        {
            // Optionally increment correct answers if at least one correct answer was selected
            correctAnswered++;
            gameData.correctAnswered++;
            score += 2;
            uiHandler.UpdatePlayer1Score(score);
        }
        answered++;
        gameData.questionsAttempted++;

        uiHandler.UpdateAnsweredFill(GetCorrectAnswerPercentage(), GetWrongAnswerPercentage());
        if(GameManager.Instance.isMultiPlayer)
        {
            multiPlayHandler.pv.RPC("Answered", Photon.Pun.RpcTarget.All, multiPlayHandler.playerId, score);
        }
        // Load another question after a short delay
        // Delay of 2 seconds before loading a new question
        StartCoroutine(LoadNewQuestion());
    }
    IEnumerator LoadNewQuestion()
    {
        if(GameManager.Instance.isMultiPlayer)
        {
            yield return new WaitUntil(predicate: () => otherPlayerAnswered == true);
            otherPlayerAnswered = false;
            yield return new WaitForSeconds(1f);
        }
        else
        {
            yield return new WaitForSeconds(2f);
        }
 
        LoadRandomQuestion(false);

    }
    public void GetDoubleChoice()
    {
        if ((gameData.freeDoubleChoices > 0 || gameData.gems >= gameData.doubleChoicePrice) && !allowMultipleSelections)
        {
            if (gameData.freeDoubleChoices > 0)
            {
                gameData.freeDoubleChoices--;
            }
            else
            {
                gameData.gems -= gameData.doubleChoicePrice;
                uiHandler.UpdateGems();
            }

            allowMultipleSelections = true;
            uiHandler.UpdateDateDoubeChoiceData();
        }
    }
    private void HandleTimer()
    {
        if (currentTime > 0)
        {
            currentTime -= Time.deltaTime;
            uiHandler.timerTxt.text = currentTime.ToString("F0");
            if (totalTime > 0) uiHandler.timerImg.fillAmount = currentTime / totalTime;
        }
        else if (!isAnswered)
        {
            isAnswered = true;
            uiHandler.timerTxt.text = "0";

            // Automatically highlight all buttons as wrong when time is up
            if (currentQuestion.questionImg == null)
            {
                foreach (var v in uiHandler.answerTxtOnlyBtnTxt)
                {
                    v.color = Color.red;
                }
            }
            else
            {
                foreach (var v in uiHandler.answerMultiModalBtnTxt)
                {
                    v.color = Color.red;
                }
            }
            if (GameManager.Instance.isMultiPlayer)
            {
                multiPlayHandler.pv.RPC("Answered", Photon.Pun.RpcTarget.All, multiPlayHandler.playerId, score);
            }
            // Load a new question after a short delay
            StartCoroutine(LoadNewQuestion());
        }
    }
    public float GetCorrectAnswerPercentage()
    {
        if (answered == 0)
            return 1f; // To avoid division by zero

        return (float)correctAnswered / answered; // Correct answer percentage between 0 and 1
    }

    public float GetWrongAnswerPercentage()
    {
        if (answered == 0)
            return 0f; // To avoid division by zero

        return (float)(answered - correctAnswered) / answered; // Wrong answer percentage between 0 and 1
    }
    public string GetWinStatus(int scoreYour, int scoreOther)
    {
        if(scoreYour == scoreOther)
        {
            return "Draw";
        }
        if (scoreYour > scoreOther)
        {
            return "Won";
        }
        if (scoreYour < scoreOther)
        {
            return "Lost";
        }
        return "Finish";
    }
    public void Home()
    {
        if(!GameManager.Instance.isMultiPlayer)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            multiPlayHandler.Home();
        }
    }
}
