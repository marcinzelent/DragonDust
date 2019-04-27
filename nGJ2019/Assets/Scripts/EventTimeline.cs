using System;
using System.Collections;
using System.Collections.Generic;

public class EventTimeline
{
	public enum SpawnEventType {alfa, beta};
	
    public class SpawnEvent
	{
		public float time;
		public float height;
		public SpawnEventType type;
		
		public SpawnEvent(float time, float height, SpawnEventType type)
		{
			this.time = time;
			this.height = height;
			this.type = type;
		}
	}
	
	public event Action<SpawnEvent> OnSpawnEvent;
	
	private List<SpawnEvent> futureEvents;
	private float currentTime;
	
	public EventTimeline()
	{
		futureEvents = new List<SpawnEvent>();
		currentTime = 0;
	}
	
	public void Add(float time, float height, SpawnEventType type)
	{
		futureEvents.Add(new SpawnEvent(time, height, type));
		futureEvents.Sort((x,y) => x.time.CompareTo(y.time));
	}
	
	public void timeTick(float deltaTime)
	{
		currentTime += deltaTime;
		
		while(futureEvents.Count > 0 && currentTime > futureEvents[0].time)
		{
			SpawnEvent e = futureEvents[0];
			futureEvents.RemoveAt(0);
			if(OnSpawnEvent != null)
				OnSpawnEvent(e);
		}
	}
}
