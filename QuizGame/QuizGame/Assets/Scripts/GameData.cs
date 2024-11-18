using UnityEngine;

[CreateAssetMenu(fileName = "GameData", menuName = "Settings/GameData", order = 0)]
public class GameData : ScriptableObject
{
    public string userName;
    public string userRank;
    public string userScore;
    public int gems;
    public int freeDoubleChoices;

    public int questionsAttempted;
    public int correctAnswered;

    public int doubleChoicePrice;
    public int fiftyFiftyPrice;

}
[System.Serializable]
public class Questions
{
    public string question;
    public Sprite questionImg;

    public string[] answers;
    public int correctAnswer;
    public int questionTime;
}