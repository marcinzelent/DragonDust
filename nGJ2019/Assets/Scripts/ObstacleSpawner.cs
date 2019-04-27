using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
	private EventTimeline timeline = new EventTimeline();
	
	public GameObject alfaPrefab;
	public GameObject betaPrefab;
    public LevelScrolling scrolling;
	
	public float spawnLine;
	
	private void spawnOnEvent(EventTimeline.SpawnEvent e)
	{
		GameObject prefab = null;
		
		if(e.type == ObstacleType.alfa)
			prefab = alfaPrefab;
		
		if(e.type == ObstacleType.beta)
			prefab = betaPrefab;
		
		var transformT = ((GameObject) Instantiate(prefab, new Vector3(spawnLine, e.height, 0), Quaternion.identity)).transform;
        scrolling.Obstacles.Add(transformT);
	}
	
    void Start()
    {
        for(int i = 0; i < 120; i += 5)
        {
            timeline.Add(i, 0, ObstacleType.alfa);
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
		Gizmos.DrawLine(new Vector3(spawnLine,-10,0), new Vector3(spawnLine,10,0));
	}
}
