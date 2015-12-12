using UnityEngine;
using UnityEngine.UI;

public class LevelPanel : MonoBehaviour
{
    public Image thumbnail;
    public Text levelname;

    public void SetAssets(string name, Sprite image)
    {
        levelname.text = name;
        thumbnail.sprite = image;
    }
}
