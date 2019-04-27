using System;
using System.Collections;
using System.Collections.Generic;

public class EventTimeline
{
	public class SpawnEvent
	{
		public float time;
		public float height;
		public ObstacleType type;
		
		public SpawnEvent(float time, float height, ObstacleType type)
		{
			this.time = time;
			this.height = height;
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
	
	public void Add(float time, float height, ObstacleType type)
	{
		futureEvents.Add(new SpawnEvent(time, height, type));
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
		
		while(futureEvents.Count > 0 && currentTime > futureEvents[0].time && futureMessages.Count > 0 && currentTime > futureMessages[0].time)
		{
			SpawnEvent e = futureEvents[0];
			futureEvents.RemoveAt(0);
			if(OnSpawnEvent != null)
				OnSpawnEvent(e);

            MessageEvent m = futureMessages[0];
            futureMessages.RemoveAt(0);
            if(OnMessageEvent != null)
                OnMessageEvent(m);
		}
	}
}
