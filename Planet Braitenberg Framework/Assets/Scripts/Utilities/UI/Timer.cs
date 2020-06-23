using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Timer : MonoBehaviour {

	public bool manualStart = false;
	public string prefix = "Time: ";

    internal bool started = true;
    internal bool handled = false;
    internal bool stop = false;

	internal int seconds = 0;
	internal int minutes = 0;

	private Text timerText;

	void Awake()
	{
		timerText = this.GetComponent<Text> ();
	}

    void Start()
    {
		if (manualStart)
			started = false;
    }

    void Update()
    {
		if (started && (handled == false))
        {
            InvokeRepeating("UpdateTime", 0f, 1.0f);
            handled = true;
        }
        if (stop)
        {
            CancelInvoke("UpdateTime");
        }
    }

	internal void SetTextColor(Color c)
	{
		//change the timer color.
		this.timerText.color = c;
	}

    void UpdateTime()
    {
        timerText.text = prefix + minutes.ToString("D2") + ":" + seconds.ToString("D2");
        seconds++;
        if (seconds == 60)
        {
            seconds = 0;
            minutes++;
        }
    }

}