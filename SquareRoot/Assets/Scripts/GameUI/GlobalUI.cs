using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalUI : MonoBehaviour
{
    // references to UI elements in scene
    public Text victoryText;
    public GameObject scoreScreen;
    public GameObject pauseMenu;

    void Start()
    {
        victoryText.gameObject.SetActive(false);
        scoreScreen.SetActive(false);

        HidePauseMenu();
    }

    public void ShowPauseMenu()
    {
        //EventSystem.current.SetSelectedGameObject(pauseMenu.GetComponentInChildren<Button>().gameObject);
        pauseMenu.SetActive(true);
    }

    public void HidePauseMenu()
    {
        pauseMenu.SetActive(false);
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

    public void SetDrawText()
    {
        victoryText.text = "Draw! Everyone loses!";
        victoryText.gameObject.SetActive(true);
    }
}
