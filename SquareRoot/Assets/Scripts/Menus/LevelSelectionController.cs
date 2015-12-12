using UnityEngine;
using System.Collections.Generic;
using InControl;

[System.Serializable]
public class Level
{
    public int levelIndex;
    public Sprite thumbnail;
    public string levelName;
    public int numberOfPlayers;
}

public class LevelSelectionController : MonoBehaviour
{
    public float smoothing = 0.4f;
    public float spacing = 20;
    private float width;
    public GameObject levelPanelsParent;
    public GameObject levelPanelPrefab;

    public Level[] levels;
    private List<Level> validLevels;
    private int currentIndex;
    private Vector3 zeroPosition;

    private int numPlayers;

    void Start()
    {
        // get number of players
        numPlayers = 0;

        if (GameObject.FindGameObjectWithTag(Tags.DeviceBag) != null && GameObject.FindGameObjectWithTag(Tags.DeviceBag).GetComponent<JoinScreenController>() != null)
        {
            JoinScreenController jsc = GameObject.FindGameObjectWithTag(Tags.DeviceBag).GetComponent<JoinScreenController>();
            numPlayers = jsc.playerList.Count;
        }

        Debug.Log(numPlayers + " players");

        // get valid levels
        validLevels = new List<Level>();

        foreach (Level level in levels)
        {
            if (level.numberOfPlayers == numPlayers)
            {
                validLevels.Add(level);
            }
        }

        Debug.Log(validLevels.Count + " valid levels");

        // populate UI
        int index = 0;
        foreach (Level level in validLevels)
        {
            GameObject panel = Instantiate(levelPanelPrefab);
            panel.transform.SetParent(levelPanelsParent.transform, false);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(index * (panel.GetComponent<RectTransform>().rect.width + spacing), 0, 0);
            panel.GetComponent<LevelPanel>().SetAssets(level.levelName, level.thumbnail);

            width = panel.GetComponent<RectTransform>().rect.width;

            index++;
        }

        currentIndex = 0;
        zeroPosition = levelPanelsParent.transform.position;
    }

    void Update()
    {
        if(InputManager.ActiveDevice.LeftStickRight.WasPressed)
        {
            currentIndex = Mathf.Clamp(currentIndex + 1, 0, validLevels.Count - 1);
        }
        if (InputManager.ActiveDevice.LeftStickLeft.WasPressed)
        {
            currentIndex = Mathf.Clamp(currentIndex - 1, 0, validLevels.Count - 1);
        }

        if (InputManager.ActiveDevice.Action1.WasPressed || InputManager.ActiveDevice.Command.WasPressed)
        {
            LoadLevel(currentIndex);
        }

        float targetX = currentIndex * (width - spacing);
        levelPanelsParent.transform.position = Vector3.Lerp(levelPanelsParent.transform.position, zeroPosition + targetX * Vector3.left, Time.deltaTime / smoothing );
    }

    void LoadLevel(int index)
    {
        Application.LoadLevel(validLevels[index].levelIndex);
    }
}
