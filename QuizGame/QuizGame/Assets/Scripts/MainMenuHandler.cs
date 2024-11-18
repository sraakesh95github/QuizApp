using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{

    public string jsonInput; // JSON is passed by reference here

    private QnAData qnaData;
    public string gamePlaySceneName;

    public GameObject loadingPanel;
    public GameObject mainPanel;
    public GameObject multiPlayerModePanel;
    public GameObject multiPlayerPrivateModePanel;
    public GameObject multiPlayerJoinPanel;
    public GameObject multiPlayerCreatePanel;
    public GameObject multiPlayerPublicPanel;
    public GameObject multiPlayerWaitingPanel;
    public GameObject noNetworkPanel;
    public List<GameObject> allPanels;


    void Start()
    {
        // Parse the JSON input into the QnAData class
        qnaData = JsonUtility.FromJson<QnAData>(jsonInput);

        // Display the parsed data for testing
        DisplayQuestions();
    }
    public void Play()
    {
        SceneManager.LoadScene(1);
    }
    void DisplayQuestions()
    {
       GameManager.Instance.questions = new Questions[qnaData.QnASets.Count];

        int i = 0;
        foreach (QnASet qna in qnaData.QnASets)
        {
            GameManager.Instance.questions[i] = new Questions();
            GameManager.Instance.questions[i].question = qna.question.text;
            if (qna.question.useImage)
            {
                StartCoroutine(DownloadImage(qna.question.imageURL, i));
            }
            GameManager.Instance.questions[i].correctAnswer = qna.question.correctAnswer;
            GameManager.Instance.questions[i].answers = new string[4];
            Debug.Log("Question: " + qna.question.text);
            Debug.Log("Image URL: " + qna.question.imageURL);
            Debug.Log("Use Image: " + qna.question.useImage);
            Debug.Log("Correct Ans: " + qna.question.correctAnswer);
            Debug.Log("Answer Choices:");
            for (int j = 0; j < qna.answerChoices.Count && j < 4; j++)
            {
                GameManager.Instance.questions[i].answers[j] = qna.answerChoices[j];
                Debug.Log(qna.answerChoices[j]);
            }

            i++;
        }
    }
   
    // Function to download the image and return a Sprite
    public IEnumerator DownloadImage(string url, int ind)
    {
      //StartCoroutine(DownloadImage(url));
        yield return null;
        using (UnityWebRequest webRequest = UnityWebRequestTexture.GetTexture(url))
        {
            // Send the request and wait for the response
            yield return webRequest.SendWebRequest();

            // Check if the request has any errors
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Failed to download image: " + webRequest.error);
                // Return null in case of an error
            }
            else
            {
                // Get the downloaded texture
                Texture2D texture = DownloadHandlerTexture.GetContent(webRequest);

                if (texture != null)
                {

                   
                   
                    Rect rect = new Rect(0, 0, texture.width, texture.height);
                    Vector2 pivot = new Vector2(0.5f, 0.5f); // Pivot at the center
                    GameManager.Instance.questions[ind].questionImg = Sprite.Create(texture, rect, pivot);
             
                }
                else
                {
                    Debug.LogError("Failed to process texture.");

                }
            }
        }
    }


    public void StartGame()
    {
        GameManager.Instance.isMultiPlayer = false;
        SceneManager.LoadScene(gamePlaySceneName);
    }

    public void ActivatePanel(GameObject panel)
    {
        foreach (GameObject g in allPanels)
        {
            g.SetActive(false);
        }
        allPanels.Find(pred => pred.gameObject == panel).SetActive(true);
    }
    public void SelectPlayer(int plrs)
    {
        GameManager.Instance.totalPlayers = plrs;
    }

}
[Serializable]
public class Question
{
    public string text;
    public string imageURL;
    public bool useImage;
    public int correctAnswer;
}

[Serializable]
public class QnASet
{
    public Question question;
    public List<string> answerChoices;
}

[Serializable]
public class QnAData
{
    public List<QnASet> QnASets;
}
