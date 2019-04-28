using System;
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
        else if (scene.name == "Tutorial")
        {
            for (int i = 0; i < 60; i++)
                timeline.Add(i * 2.5f, new Vector3(0, 0, 0), ObstacleType.caveWalls);

            timeline.Add(10, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);
            timeline.Add(15, new Vector3(0, -2, 0), ObstacleType.rockBottom);
            timeline.Add(20, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);
            timeline.Add(23, new Vector3(0, -2, 0), ObstacleType.rockBottom);
            timeline.Add(26, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);

            timeline.Add(40, new Vector3(0, 0, 0), ObstacleType.narrowPassage);
            timeline.Add(48, new Vector3(0, -2.5f, 2), ObstacleType.rockJaws);
            timeline.Add(56, new Vector3(0, 0.5f, 3), ObstacleType.net);
        }
        else if (scene.name == "Level1")
        {
            System.Random random = new System.Random();
            for (int i = 0; i < 1000; i++)
            {
                timeline.Add(i * 2.5f, new Vector3(0,0,0), ObstacleType.caveWalls);

                Array values = Enum.GetValues(typeof(ObstacleType));
                ObstacleType randomObstacle = (ObstacleType)values.GetValue(random.Next(values.Length));
                Vector3 position;

                switch (randomObstacle)
                {
                    case ObstacleType.rockTop:
                        position = new Vector3(0, 2.75f, 0);
                        break;
                    case ObstacleType.rockBottom:
                        position = new Vector3(0, -2, 0);
                        break;
                    case ObstacleType.rockJaws:
                        position = new Vector3(0, -2.5f, 2);
                        break;
                    case ObstacleType.net:
                        position = new Vector3(0, 0.5f, 3);
                        break;
                    default:
                        position = new Vector3(0,0,0);
                        break;
                }
                timeline.Add(i * 4, position, randomObstacle);
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
