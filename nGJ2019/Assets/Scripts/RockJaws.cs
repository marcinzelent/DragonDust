using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RockJaws : MonoBehaviour
{
    public Transform TopJaw;
    public Transform BottomJaw;
    public int Speed = 10;
    private bool reverse = false;
    private float initialTopJawY;
    // Start is called before the first frame update
    void Start()
    {
        initialTopJawY = TopJaw.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (TopJaw.localPosition.y < BottomJaw.localPosition.y) reverse = true;
        else if(TopJaw.localPosition.y > initialTopJawY) reverse = false;

        if (!reverse) {
            TopJaw.Translate(0, -0.01f * Speed, 0);
            BottomJaw.Translate(0, 0.01f * Speed, 0);
        } else {
            TopJaw.Translate(0, 0.01f * Speed, 0);
            BottomJaw.Translate(0, -0.01f * Speed, 0);
        }
    }
}
