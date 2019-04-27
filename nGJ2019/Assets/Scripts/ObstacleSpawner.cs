using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
  private EventTimeline timeline = new EventTimeline();

  public GameObject caveWallsPrefab, rockTopPrefab, rockBottomPrefab, narrowPassagePrefab, rockJawsPrefab;
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
    }

    var transformT = ((GameObject)Instantiate(prefab, new Vector3(spawnLine, e.height, 0), Quaternion.identity)).transform;
    scrolling.Obstacles.Add(transformT);
  }

  void Start()
  {
    for (int i = 0; i < 120; i += 5)
    {
      timeline.Add(i, 0, ObstacleType.caveWalls);
    }
    timeline.Add(2, 2.75f, ObstacleType.rockTop);
    timeline.Add(8, -2, ObstacleType.rockBottom);
    timeline.Add(20, 0, ObstacleType.narrowPassage);
    timeline.Add(30, 0, ObstacleType.rockJaws);

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
}
