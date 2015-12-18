using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class GlobalUI : MonoBehaviour
{
    // references to UI elements in scene
    public GameObject victoryPanel;
    public Text victoryText;
    public GameObject scoreScreen;
    public GameObject pauseMenu;
    public GameObject gameOverScreen;
    public GameObject gameOverFirstButton;

    public GameObject minimap_2p;
    public GameObject minimap_3p;
    public GameObject minimap_4p;

    public void SetMinimap(int numPlayers)
    {
        minimap_2p.SetActive(false);
        minimap_3p.SetActive(false);
        minimap_4p.SetActive(false);

        switch (numPlayers)
        {
            case 1:
            case 2:
                minimap_2p.SetActive(true);
                break;
            case 3:
                minimap_3p.SetActive(true);
                break;
            case 4:
                minimap_4p.SetActive(true);
                break;
        }
    }

    void Start()
    {
        victoryPanel.SetActive(false);
        scoreScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        HidePauseMenu();
    }
    public void ShowGameOverScreen(){

        gameOverScreen.SetActive(true);
        EventSystem.current.SetSelectedGameObject(gameOverFirstButton);
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
        victoryPanel.SetActive(true);
    }

    public void SetDrawText()
    {
        victoryText.text = "Draw! Everyone loses!";
        victoryPanel.SetActive(true);
    }
}
