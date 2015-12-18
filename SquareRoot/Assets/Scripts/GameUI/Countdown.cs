using UnityEngine;
using UnityEngine.UI;

public class Countdown : MonoBehaviour {

    private bool running;
    private float countdownStart;
    private float countdownLength;

    public Image fill;
    public Text number;

    public void StartCountdown(float length)
    {
        countdownLength = length;
        countdownStart = Time.time;
        running = true;
    }

	void Update ()
    {
        if (running)
        {
            float aboveSecond = Mathf.Repeat(Time.time - countdownStart, 1);
            float seconds = (Time.time - countdownStart) - aboveSecond;

            fill.fillAmount = 1f - aboveSecond;
            number.text = (countdownLength - seconds).ToString();

            if (Time.time - countdownStart > countdownLength)
            {
                running = false;
                gameObject.SetActive(false);
            }
        }
	}
}
