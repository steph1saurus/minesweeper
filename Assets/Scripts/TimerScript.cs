using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [Header("Private variables")]
    public TMP_Text timerText;
    public float timer = 0f;
    public bool timerActive = false;

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
            DisplayTime();
        }


    }


    void DisplayTime()
    {
        int minutes = Mathf.FloorToInt(timer / 60.0f);
        int seconds = Mathf.FloorToInt(timer - minutes * 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);

    }

    public void StartTimer()
    {
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void ResetTimer()
    {
        timer = 0.0f;
    }
}
