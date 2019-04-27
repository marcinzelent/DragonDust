using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Messenger : MonoBehaviour
{
    public Text MainText;
    public ObstacleSpawner Spawner;
    private EventTimeline timeline;

    // Start is called before the first frame update
    void Start()
    {
        timeline = Spawner.GetEventTimeline();

        timeline.Add(1, "Test");
        timeline.Add(3, "");

        timeline.OnMessageEvent += MessageOnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void MessageOnEvent(EventTimeline.MessageEvent e)
    {
        MainText.text = e.message;
    }
}
