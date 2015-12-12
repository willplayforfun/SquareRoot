using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{
    public static Color[] playerColors = { new Color(30, 167, 255),
                                           new Color(232, 106, 23),
                                           new Color(115, 205, 75),
                                           new Color(255, 204, 0) };

    // GameController used to get required resource count
    GameController gameController;

    public PlayerObject player;

    public Image needBar;
    public Image resourceBar;

    void Start()
    {
        gameController = GameObject.FindGameObjectWithTag(Tags.GameController).GetComponent<GameController>();

        resourceBar.color = playerColors[(int)player.number];
        needBar.color = playerColors[(int)player.number];
    }

    void Update()
    {
        resourceBar.fillAmount = player.resources / PlayerObject.MaxResources;
        needBar.fillAmount = gameController.requiredResources / PlayerObject.MaxResources;
    }
}
