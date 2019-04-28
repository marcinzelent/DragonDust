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
        var topJawHeight= TopJaw.GetComponent<MeshRenderer>().bounds.size.y;
        var bottomJawHeight= BottomJaw.GetComponent<MeshRenderer>().bounds.size.y;

        if (TopJaw.localPosition.y - topJawHeight / 2 < BottomJaw.localPosition.y + bottomJawHeight / 2) reverse = true;
        else if(TopJaw.localPosition.y > initialTopJawY) reverse = false;   

        if (!reverse) {
            TopJaw.Translate(0, -0.01f * Speed, 0, Space.World);
            BottomJaw.Translate(0, 0.01f * Speed, 0, Space.World);
        } else {
            TopJaw.Translate(0, 0.01f * Speed, 0, Space.World);
            BottomJaw.Translate(0, -0.01f * Speed, 0, Space.World);
        }
    }
}
