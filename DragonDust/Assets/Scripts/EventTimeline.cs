using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTimeline
{
	public class SpawnEvent
	{
		public float time;
		public Vector3 position;
		public ObstacleType type;
		
		public SpawnEvent(float time, Vector3 position, ObstacleType type)
		{
			this.time = time;
			this.position = position;
			this.type = type;
		}
	}

    public class MessageEvent
    {
        public float time;
        public String message;

        public MessageEvent(float time, String message)
        {
            this.time = time;
            this.message = message;
        }
    }
	
	public event Action<SpawnEvent> OnSpawnEvent;
    public event Action<MessageEvent> OnMessageEvent;
	
	private List<SpawnEvent> futureEvents;
    private List<MessageEvent> futureMessages;
	private float currentTime;
	
	public EventTimeline()
	{
		futureEvents = new List<SpawnEvent>();
        futureMessages = new List<MessageEvent>();
		currentTime = 0;
	}
	
	public void Add(float time, Vector3 position, ObstacleType type)
	{
		futureEvents.Add(new SpawnEvent(time, position, type));
		futureEvents.Sort((x,y) => x.time.CompareTo(y.time));
	}

    public void Add(float time, String message)
    {
        futureMessages.Add(new MessageEvent(time, message));
        futureMessages.Sort((x,y) => x.time.CompareTo(y.time));
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

        while(futureMessages.Count > 0 && currentTime > futureMessages[0].time)
        {
            MessageEvent m = futureMessages[0];
            futureMessages.RemoveAt(0);
            if(OnMessageEvent != null)
                OnMessageEvent(m);
        }
	}
}
