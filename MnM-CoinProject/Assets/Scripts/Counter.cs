using UnityEngine;
using UnityEngine.UI;

public class Counter : MonoBehaviour
{
    [SerializeField]
    private Canvas canvas;
    [SerializeField]
    private GameObject LaundryCounter;
    [SerializeField]
    private GameObject MnMCounter;
    [SerializeField]
    private Video VideoScript;

    private float VideoStartTime;
    // TimeFor1stAction = Time since video start til first Action - TimeForNextAction
    private float TimeFor1stAction;
    private float ActionStartTime;
    // Duration between each Action
    private float TimeForNextAction;

    private int CounterNumber = 0;
    private Text CounterText;
    private bool StartedAction = false;
    private bool EndedAction = false;
    
    private void Start ()
    {
        CounterText = canvas.transform.GetChild(0).GetChild(0).GetComponent<Text>();

        // Set Counter timings for different video
        if (VideoScript.Gender == "Female")
        {
            switch(VideoScript.VideoIndex)
            {
                case 1:
                    TimeFor1stAction = 3;
                    TimeForNextAction = 2.5f;
                    break;
                case 2:
                    TimeFor1stAction = 2.3f;
                    TimeForNextAction = 2.85f;
                    break;
                case 3:
                    TimeFor1stAction = 6;
                    TimeForNextAction = 4.5f;
                    break;
                case 4:
                    TimeFor1stAction = 5.85f;
                    TimeForNextAction = 4.45f;
                    break;
            }
        }
        else if (VideoScript.Gender == "Male")
        {
            switch (VideoScript.VideoIndex)
            {
                case 1:
                    TimeFor1stAction = 4;
                    TimeForNextAction = 4;
                    break;
                case 2:
                    TimeFor1stAction = 3.9f;
                    TimeForNextAction = 4.4f;
                    break;
                case 3:
                    TimeFor1stAction = 3.3f;
                    TimeForNextAction = 8;
                    break;
                case 4:
                    TimeFor1stAction = 5.2f;
                    TimeForNextAction = 5.7f;
                    break;
            }
        }

        // Set Counter position for different video
        if (VideoScript.VideoIndex == 1 || VideoScript.VideoIndex == 2)
        {
            canvas.transform.position = LaundryCounter.transform.position;
        }
        else if (VideoScript.VideoIndex == 3 || VideoScript.VideoIndex == 4)
        {
            canvas.transform.position = MnMCounter.transform.position;
        }
        
        VideoStartTime = Time.time;
	}
    
    private void CountIncrement()
    {
        CounterNumber += 1;

        // Convert to string for visual feedback
        if (CounterNumber <= 9)
        {
            CounterText.text = "0" + CounterNumber.ToString();
        }
        else
        {
            CounterText.text = CounterNumber.ToString();
        }

        // Stop updating Counter when reach last action
        if (VideoScript.VideoIndex == 1 || VideoScript.VideoIndex == 3)
        {
            if (CounterNumber > 2 && !EndedAction)
            {
                EndedAction = true;
            }
        }
        else if(VideoScript.VideoIndex == 2 || VideoScript.VideoIndex == 4)
        {
            if (CounterNumber > 29 && !EndedAction)
            {
                EndedAction = true;
            }
        }

        ActionStartTime = Time.time;
    }
    
	private void Update ()
    {
        // Do nothing when not playing video
        if (!VideoScript.videoPlayer.isPlaying)
            return;

        if (Input.GetKeyUp(KeyCode.Space))
        {
            Debug.Log("Enter: " + Time.time);
        }

        if (!StartedAction)
        {
            if (Time.time - VideoStartTime > TimeFor1stAction)
            {
                StartedAction = true;
                ActionStartTime = Time.time;
            }
        }
        else
        {
            if (!EndedAction)
            {
                if(Time.time - ActionStartTime > TimeForNextAction)
                {
                    CountIncrement();
                }
            }
        }
    }
}
