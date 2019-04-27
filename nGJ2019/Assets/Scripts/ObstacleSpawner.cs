using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
	private EventTimeline timeline = new EventTimeline();
	
	public GameObject alfaPrefab;
	public GameObject betaPrefab;
	
	public float spawnLine;
	
	private void spawnOnEvent(EventTimeline.SpawnEvent e)
	{
		GameObject prefab = null;
		
		if(e.type == EventTimeline.SpawnEventType.alfa)
			prefab = alfaPrefab;
		
		if(e.type == EventTimeline.SpawnEventType.beta)
			prefab = betaPrefab;
		
		Instantiate(prefab, new Vector3(spawnLine, e.height, 0), Quaternion.identity);
	}
	
    void Start()
    {
        timeline.Add(2, 2, EventTimeline.SpawnEventType.alfa);
		timeline.Add(4, 2, EventTimeline.SpawnEventType.beta);
		timeline.Add(5, -2, EventTimeline.SpawnEventType.beta);
		timeline.Add(7, -3, EventTimeline.SpawnEventType.alfa);
		
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
