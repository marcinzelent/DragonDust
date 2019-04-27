using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelScrolling : MonoBehaviour
{
    public int UpdateRate = -5;
    public List<GameObject> Obstacles;
    public Transform Background;

    private Vector3 initialBgPos;
    private float[] backgroundSize;

    // Start is called before the first frame update
    void Start()
    {
        ResizeBackground();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        MoveBackground();
        MoveObstacles();
        DestroyObstacles();
    }

    void ResizeBackground()
    {
        var sr = Background.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        Background.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var finalHeight = (float)(worldScreenHeight / height);
        var finalWidth = (float)(worldScreenWidth / width);

        Background.localScale = new Vector3(finalHeight, finalHeight, 1);

        var viewportX = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0)).x;
        Background.position = new Vector3(viewportX * -1, 0, 0);
        initialBgPos = Background.position;
    }

    void MoveObstacles()
    {
        for (int i = 0; i < Obstacles.Count; i++)
        {
            Obstacles[i].transform.Translate(0.01f * UpdateRate, 0, 0);
        }
    }

    void DestroyObstacles()
    {
        if (Obstacles[0].transform.position.x < -30)
        {
            Destroy(Obstacles[0]);
            Obstacles.RemoveAt(0);
        }
    }

    void MoveBackground()
    {
        if (Background.position.x > -initialBgPos.x)
            Background.Translate(0.01f * UpdateRate, 0, 0);
        else Background.position = initialBgPos;
    }
}
