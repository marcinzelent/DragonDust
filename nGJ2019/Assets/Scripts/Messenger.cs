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

        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.name == "Tutorial")
        {
            timeline.Add(2, "USE WSAD KEYS OR GAMEPAD THUMB TO CONTROL THE DRAGON");
            timeline.Add(8, "AVOID OBSTACLES, YOU WILL LOSE HEALTH IF YOU HIT THEM");
            timeline.Add(31, "USE I, J, AND K KEYS TO CHANGE THE DRAGON FORM");
            timeline.Add(39, "PRESS J TO SHRINK AND FIT IN THE NARROW PASSAGE AHEAD");
            timeline.Add(43, "");
            timeline.Add(55, "PRESS K TO SPLIT INTO PARTICLES AND GO THROUGH THE NET");
            timeline.Add(59, "");
        }

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
