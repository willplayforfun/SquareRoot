using UnityEngine;
using UnityEngine.UI;

public class GlobalUI : MonoBehaviour
{
    public Text victoryText;
    public GameObject scoreScreen;

    void Start()
    {
        victoryText.gameObject.SetActive(false);
        scoreScreen.SetActive(false);
    }

    public void SetVictoryText(PlayerNum playerNumber)
    {
        string text = "Player ";

        switch(playerNumber)
        {
            case PlayerNum.First:
                text += "1";
                break;
            case PlayerNum.Second:
                text += "2";
                break;
            case PlayerNum.Third:
                text += "3";
                break;
            case PlayerNum.Fourth:
                text += "4";
                break;
        }

        text += " Wins!";

        victoryText.text = text;
        victoryText.gameObject.SetActive(true);
    }
}
