using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    private EventTimeline timeline = new EventTimeline();

    public GameObject caveWallsPrefab, rockTopPrefab, rockBottomPrefab, narrowPassagePrefab, rockJawsPrefab, netPrefab;
    public LevelScrolling scrolling;

    public float spawnLine;

    private void spawnOnEvent(EventTimeline.SpawnEvent e)
    {
        GameObject prefab = null;

        switch (e.type)
        {
            case ObstacleType.caveWalls:
                prefab = caveWallsPrefab;
                break;
            case ObstacleType.rockTop:
                prefab = rockTopPrefab;
                break;
            case ObstacleType.rockBottom:
                prefab = rockBottomPrefab;
                break;
            case ObstacleType.narrowPassage:
                prefab = narrowPassagePrefab;
                break;
            case ObstacleType.rockJaws:
                prefab = rockJawsPrefab;
                break;
            case ObstacleType.net:
                prefab = netPrefab;
                break;
        }

        var o = Instantiate(prefab, new Vector3(spawnLine, e.position.y, e.position.z), Quaternion.identity);
        scrolling.Obstacles.Add(o);
    }

    void Start()
    {
        var scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        if (scene.name == "TitleScreen")
        {
            for (int i = 0; i < 1000; i++)
            {
                timeline.Add(i * 2.5f, new Vector3(0, 0, 0), ObstacleType.caveWalls);
                timeline.Add(i * 12.5f + 3, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);
                timeline.Add(i * 12.5f + 6, new Vector3(0, -2, 0), ObstacleType.rockBottom);
                timeline.Add(i * 12.5f + 9, new Vector3(0, 0, 0), ObstacleType.narrowPassage);
                timeline.Add(i * 12.5f + 12.5f, new Vector3(0, -2.5f, 2), ObstacleType.rockJaws);
                timeline.Add(i * 12.5f + 15f, new Vector3(0, 0.5f, 3), ObstacleType.net);
            }
        }
        else if (scene.name == "Level1")
        {
            for (int i = 0; i < 1000; i++)
            {
                timeline.Add(i * 2.5f, new Vector3(0, 0, 0), ObstacleType.caveWalls);
                timeline.Add(i * 12.5f + 3, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);
                timeline.Add(i * 12.5f + 6, new Vector3(0, -2, 0), ObstacleType.rockBottom);
                timeline.Add(i * 12.5f + 9, new Vector3(0, 0, 0), ObstacleType.narrowPassage);
                timeline.Add(i * 12.5f + 12.5f, new Vector3(0, -2.5f, 2), ObstacleType.rockJaws);
                timeline.Add(i * 12.5f + 15f, new Vector3(0, 0.5f, 3), ObstacleType.net);
            }
        }

        timeline.OnSpawnEvent += spawnOnEvent;
    }

    // Update is called once per frame
    void Update()
    {
        timeline.timeTick(Time.deltaTime);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(new Vector3(spawnLine, -10, 0), new Vector3(spawnLine, 10, 0));
    }

    public EventTimeline GetEventTimeline()
    {
        return timeline;
    }
}
