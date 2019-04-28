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
        for (int i = 0; i < 120; i += 5)
        {
            timeline.Add(i, new Vector3(0, 0, 0), ObstacleType.caveWalls);
        }
        timeline.Add(0, new Vector3(0, 2.75f, 0), ObstacleType.rockTop);
        timeline.Add(6, new Vector3(0, -2, 0), ObstacleType.rockBottom);
        timeline.Add(12, new Vector3(0, 0, 0), ObstacleType.narrowPassage);
        timeline.Add(20, new Vector3(0, -2.5f, 2), ObstacleType.rockJaws);
        timeline.Add(25, new Vector3(0, 0.5f, 3), ObstacleType.net);

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
