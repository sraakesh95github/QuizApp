using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIHandler : MonoBehaviour
{
    public GamePlayHandler gamePlayHandler;
    public GameData gameData;
 

    [Header("Gems")]
    public TextMeshProUGUI[] gemsTxt;

    [Header("Players")]
    public GameObject player1Obj;
    public Image player1Img;
    public TextMeshProUGUI player1Name;
    public TextMeshProUGUI player1Rank;
    public TextMeshProUGUI player1Score;
    [Space]
    public GameObject player2Obj;
    public Image player2Img;
    public TextMeshProUGUI player2Name;
    public TextMeshProUGUI player2Rank;
    public TextMeshProUGUI player2Score;


    [Header("Timer")]
    public TextMeshProUGUI timerTxt;
    public Image timerImg;

    [Header("Answered Image Fills")]
    public Image correctOldFillImg;
    public Image correctNewFillImg;
    public Image wrongOldFillImg;
    public Image wrongNewFillImg;

    [Header("Questions Text Only")]
    public GameObject txtOnlyObj;
    public TextMeshProUGUI questionTxtOnlyTxt;
    public TextMeshProUGUI[] answerTxtOnlyBtnTxt;
    public Button[] answerTxtOnlyBtn;

    [Header("Questions Multi Modal")]
    public GameObject multiModalObj;
    public TextMeshProUGUI questionMultiModalTxt;
    public Image questionMultiModalImg;
    public TextMeshProUGUI[] answerMultiModalBtnTxt;
    public Button[] answerMultiModalBtn;

    [Header("PowerUps")]
    public TextMeshProUGUI fiftyFiftyPriceTxt;
    public TextMeshProUGUI doubleChoicePriceTxt;
    public TextMeshProUGUI doubleChoiceFreeTxt;
    public GameObject doubleChoiceFreeObj;


    [Header("Finish")]
    public GameObject localFinishPanel;
    public TextMeshProUGUI localFinishScore;
    public TextMeshProUGUI localFinishName;

    public GameObject multipPlayerFinishPanel;
    public TextMeshProUGUI multipPlayerWinStatus;
    
    public TextMeshProUGUI multipPlayerYourFinishScore;
    public TextMeshProUGUI multipPlayerYourFinishName;

    public TextMeshProUGUI multipPlayerOtherFinishScore;
    public TextMeshProUGUI multipPlayerOtherFinishName;


   

    private void Awake()
    {
        UpdateGems();
        UpdateDateDoubeChoiceData();
    }


    public void UpdateGems()
    {
        foreach (TextMeshProUGUI g in gemsTxt)
        {
            g.text = gameData.gems.ToString();
        }
    }
    public void UpdatePlayer1Data()
    {
        player1Name.text = gameData.userName;
        player1Rank.text = gameData.userRank;

    }
    public void UpdatePlayer1Score(int score = 0)
    {

        player1Score.text = score.ToString();
    }
    public void UpdatePlayer2Data(string nm,string rnk)
    {
        player2Name.text = nm;
        player2Rank.text = rnk;

    }
    public void UpdatePlayer2Score(int score = 0)
    {

        player2Score.text = score.ToString();
    }
    public void DisplayQuestion(Questions question)
    {
        // Check if the question has an image or not
        if (question.questionImg != null)
        {

            // Use Multi-Modal UI (Image + Text)
            foreach (var v in answerMultiModalBtnTxt)
            {
                v.color = Color.white;
            }
            multiModalObj.SetActive(true);
            txtOnlyObj.SetActive(false);

            questionMultiModalTxt.text = question.question;
            questionMultiModalImg.sprite = question.questionImg;

            for (int i = 0; i < question.answers.Length; i++)
            {
                answerMultiModalBtnTxt[i].text = question.answers[i];
                answerMultiModalBtn[i].gameObject.SetActive(true);
            }

            // Disable unused answer buttons
            for (int i = question.answers.Length; i < answerMultiModalBtnTxt.Length; i++)
            {
                answerMultiModalBtn[i].gameObject.SetActive(false);
            }
        }
        else
        {
            // Use Text-Only UI
            foreach (var v in answerTxtOnlyBtnTxt)
            {
                v.color = Color.white;
            }
            txtOnlyObj.SetActive(true);
            multiModalObj.SetActive(false);

            questionTxtOnlyTxt.text = question.question;

            for (int i = 0; i < question.answers.Length; i++)
            {
                answerTxtOnlyBtnTxt[i].text = question.answers[i];
                answerTxtOnlyBtn[i].gameObject.SetActive(true);
            }

            // Disable unused answer buttons
            for (int i = question.answers.Length; i < answerTxtOnlyBtnTxt.Length; i++)
            {
                answerTxtOnlyBtn[i].gameObject.SetActive(false);
            }
        }
    }

    public void UpdateAnsweredFill(float cAns, float wAns)
    {
        correctOldFillImg.fillAmount = correctNewFillImg.fillAmount;
        wrongOldFillImg.fillAmount = wrongNewFillImg.fillAmount;


        correctNewFillImg.fillAmount = cAns;
        wrongNewFillImg.fillAmount = wAns;
    }
    public void UpdateDateDoubeChoiceData()
    {
        if (gameData.freeDoubleChoices > 0)
        {
            doubleChoiceFreeObj.SetActive(true);
            doubleChoiceFreeTxt.text = gameData.freeDoubleChoices + " FREE";
        }
        else
        {
            doubleChoicePriceTxt.text = gameData.doubleChoicePrice.ToString();
            doubleChoiceFreeObj.SetActive(false);
        }
    }

    public void FinishLocal(string pName, string pScore)
    {
        localFinishScore.text = pScore;
        localFinishName.text = pName;
        localFinishPanel.SetActive(true);
    }
    public void FinishMultiPlayer(string p1Name, string p1Score, string p2Name, string p2Score,string wStatus)
    {
        multipPlayerWinStatus.text = wStatus;
        multipPlayerYourFinishName.text = p1Name;
        multipPlayerYourFinishScore.text = p1Score;
        multipPlayerOtherFinishName.text = p2Name;
        multipPlayerOtherFinishScore.text = p2Score;
        multipPlayerFinishPanel.SetActive(true);
    }
}
