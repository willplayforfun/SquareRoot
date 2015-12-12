using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    private Color[] playerColors = { new Color(30  / 255f, 167 / 255f, 255 / 255f),
                                     new Color(232 / 255f, 106 / 255f, 23  / 255f),
                                     new Color(115 / 255f, 205 / 255f, 75  / 255f),
                                     new Color(255 / 255f, 204 / 255f, 0   / 255f) };

    // GameController used to get required resource count
    GameController gameController;

    public PlayerObject player;

    public Image needBar;
    public Image resourceBar;

    public float offsetFromEdge = 20;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>();

        resourceBar.color = playerColors[(int)player.number];
        //needBar.color = playerColors[(int)player.number];
    }

    void Update()
    {
        resourceBar.fillAmount = Mathf.Floor(player.resources) / PlayerObject.MaxResources;
        needBar.fillAmount = (float)gameController.requiredResources / (float)PlayerObject.MaxResources;
    }

    public void SetHUDPosition(PlayerNum num, int numPlayers)
    {
        switch (num)
        {
            case PlayerNum.First:
                if (numPlayers == 1)
                {
                    SetUpperLeft();
                }
                else if (numPlayers < 4)
                {
                    SetUpperLeft();
                }
                else
                {
                    SetUpperLeft();
                }
                break;
            case PlayerNum.Second:
                if (numPlayers == 2)
                {
                    SetLowerLeft();
                }
                else if (numPlayers == 3)
                {
                    SetLowerLeft();
                }
                else if (numPlayers == 4)
                {
                    SetUpperRight();
                }
                break;
            case PlayerNum.Third:
                if (numPlayers == 4)
                {
                    SetLowerLeft();
                }
                else
                {
                    SetLowerRight();
                }
                break;
            case PlayerNum.Fourth:
                SetLowerRight();
                break;
        }
    }

    void SetUpperLeft()
    {
        resourceBar.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
        resourceBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
        resourceBar.GetComponent<RectTransform>().pivot = new Vector2(0, 1);

        resourceBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetFromEdge, -offsetFromEdge);
        resourceBar.fillOrigin = 0;
        needBar.fillOrigin = 0;
    }

    void SetLowerLeft()
    {
        resourceBar.GetComponent<RectTransform>().anchorMin = new Vector2(0, 0);
        resourceBar.GetComponent<RectTransform>().anchorMax = new Vector2(0, 0);
        resourceBar.GetComponent<RectTransform>().pivot = new Vector2(0, 0);

        resourceBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(offsetFromEdge, offsetFromEdge);
        resourceBar.fillOrigin = 0;
        needBar.fillOrigin = 0;
    }

    void SetUpperRight()
    {
        resourceBar.GetComponent<RectTransform>().anchorMin = new Vector2(1, 1);
        resourceBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 1);
        resourceBar.GetComponent<RectTransform>().pivot = new Vector2(1, 1);

        resourceBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-offsetFromEdge, -offsetFromEdge);
        resourceBar.fillOrigin = 1;
        needBar.fillOrigin = 1;
    }

    void SetLowerRight()
    {
        resourceBar.GetComponent<RectTransform>().anchorMin = new Vector2(1, 0);
        resourceBar.GetComponent<RectTransform>().anchorMax = new Vector2(1, 0);
        resourceBar.GetComponent<RectTransform>().pivot = new Vector2(1, 0);

        resourceBar.GetComponent<RectTransform>().anchoredPosition = new Vector2(-offsetFromEdge, offsetFromEdge);
        resourceBar.fillOrigin = 1;
        needBar.fillOrigin = 1;
    }
}
