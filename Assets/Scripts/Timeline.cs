using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Timeline : MonoBehaviour {

	public Patient patientPrefab;
	public Spawnpoint[] spawnpoints;
	public TextAsset timelineJSON;
	
	public enum EventStatus { registered, started, ended }

	[Serializable]
	public abstract class Event : ISerializationCallbackReceiver {
		public Event() { }
		public Event(SerializeTime delay) : this(null, delay) { }
		public Event(EventPrereq[] after, SerializeTime delay) : this(null, after, delay) { }
		public Event(string id, EventPrereq[] after, SerializeTime delay) {
			this.id = id;
			this.after = after;
			this.delay = delay;
			OnAfterDeserialize();
		}
		[Serializable]
		public class EventPrereq : ISerializationCallbackReceiver {
			public EventPrereq() : this(null) { }
			public EventPrereq(string event_id) : this(event_id, default(EventStatus)) { }
			public EventPrereq(string event_id, EventStatus targetStatus) {
				this.event_id = event_id;
				this.targetStatus = targetStatus;
				OnBeforeSerialize();
			}
			public string event_id;
			public string has;

			[NonSerialized]
			public EventStatus targetStatus;
			public void OnBeforeSerialize() {
				has = Enum.GetName(typeof(EventStatus), targetStatus);
			}
			public void OnAfterDeserialize() {
				try {
					targetStatus = (has == null) ? default(EventStatus) : (EventStatus)Enum.Parse(typeof(EventStatus), has, true);
				} catch (ArgumentException e) {
					throw new FormatException(has + " is not a valid EventStatus", e);
				}
			}
		}
		public string id;
		public EventPrereq[] after;
		public SerializeTime delay;
		public abstract void Execute(Timeline timeline);

		[NonSerialized]
		public EventStatus status;
		// Using last value of enum to get enum.Count - 1 (Because C#)
		private List<Event>[] _afterIHave = Utilities.ArrayOfLists<Event>((int)EventStatus.ended);
		public List<Event> AfterIHave(EventStatus status) {
			return _afterIHave[(int)status - 1];
		}

		// While running, keep prereqs in dictionary for easy access
		[NonSerialized]
		public Dictionary<string, EventStatus> prereqs = new Dictionary<string, EventStatus>();
		public void OnBeforeSerialize() {
			after = new EventPrereq[prereqs.Count];
			int i = 0;
			foreach (var kv in prereqs) {
				after[i] = new EventPrereq(kv.Key, kv.Value);
				i++;
			}
		}
		public void OnAfterDeserialize() {
			prereqs.Clear();
			if (after == null) return;
			foreach (var p in after) {
				prereqs.Add(p.event_id, p.targetStatus);
			}
		}
	}

	[Serializable]
	public class TimelineData {
		[Serializable]
		public class SpawnEvent : Event {
			public string spawnpoint;
			public PatientData patient;

			public override void Execute(Timeline timeline) {
				Spawnpoint spawn = timeline.GetSpawnpoint(spawnpoint);
				Debug.Log(timeline.patientPrefab);
				Patient p = Instantiate(timeline.patientPrefab);
				p.transform.position = spawn.transform.position;
				p.Setup(patient);
				p.AddDespawnAction(() => timeline.EventEnded(this));
			}
		}
		public SpawnEvent[] spawns;
	}

	private SortedList<float, Event> eventQueue = new SortedList<float, Event>();
	private Dictionary<string, Event> eventIdMap = new Dictionary<string, Event>();
	private Dictionary<string, ICollection<Event>> waitingOnId = new Dictionary<string, ICollection<Event>>();
	private Dictionary<string, Spawnpoint> spawnpointIds = new Dictionary<string, Spawnpoint>();

	public int TotalSpawnpointWeight { get; private set; }

	// Use this for initialization
	void Start () {
		if (spawnpoints == null || spawnpoints.Length == 0) {
			spawnpoints = GetComponentsInChildren<Spawnpoint>();
			if (spawnpoints == null || spawnpoints.Length == 0) {
				Debug.LogWarning("No spawnpoints found!");
			}
		}
		TotalSpawnpointWeight = 0;
		foreach (var sp in spawnpoints) {
			spawnpointIds.Add(sp.id, sp);
			TotalSpawnpointWeight += sp.weight;
		}

		TimelineData data = JsonUtility.FromJson<TimelineData>(timelineJSON.text);
		print(JsonUtility.ToJson(data));
		foreach (var spawn in data.spawns) {
			AddEvent(spawn);
		}
	}

	public void AddEvent(Event e) {
		if (e.id != null) RegisterId(e);
		e.status = EventStatus.registered;
		if (e.prereqs.Count == 0) {
			EnqueEvent(e);
		} else {
			var ids = e.prereqs.Keys.ToArray();
			foreach (var id in ids) {
				Event target;
				if (eventIdMap.TryGetValue(id, out target)) {
					// Already registered
					CheckPrereqAgainstRegistered(e, target);
				} else {
					waitingOnId.GetOrInsertDefault(id, () => new List<Event>()).Add(e);
				}
			}
		}
	}

	private void RegisterId(Event e) {
		eventIdMap.Add(e.id, e);
		// Were any other events waiting for this event to be registered?
		ICollection<Event> waiting;
		if (waitingOnId.TryGetValue(e.id, out waiting)) {
			foreach (var w in waiting) {
				CheckPrereqAgainstRegistered(w, e);
			}
			waitingOnId.Remove(e.id);
		}
	}

	private void CheckPrereqAgainstRegistered(Event checking, Event registered) {
		EventStatus desiredStatus = checking.prereqs[registered.id];
		if (registered.status >= desiredStatus) {
			PrereqSatisfied(checking, registered.id);
		} else {
			registered.AfterIHave(desiredStatus).Add(checking);
		}
	}

	private void EnqueEvent(Event e) {
		Debug.Log("Enqueue Event " + e.id + " for " + (Time.time + e.delay));
		eventQueue.Add(Time.time + e.delay, e);
	}

	public void EventEnded(Event e) {
		e.status = EventStatus.ended;
		foreach (Event w in e.AfterIHave(EventStatus.ended)) {
			PrereqSatisfied(w, e.id);
		}
	}

	public Spawnpoint GetSpawnpoint(string id = null) {
		Spawnpoint spawn;
		if (id == null) {
			spawn = spawnpoints.WeightedChoice(TotalSpawnpointWeight);
		} else if (!spawnpointIds.TryGetValue(id, out spawn)) {
			throw new ArgumentException(id + " is not a known spawnpoint");
		}
		return spawn;
	}

	private void PrereqSatisfied(Event e, string prereq_id) {
		e.prereqs.Remove(prereq_id);
		if (e.prereqs.Count == 0) {
			EnqueEvent(e);
		}
	}
	
	// Update is called once per frame
	void Update () {
		while (eventQueue.Count > 0 && eventQueue.Keys[0] <= Time.time) {
			Event e = eventQueue.Values[0];
			eventQueue.RemoveAt(0);
			e.Execute(this);
			e.status = EventStatus.started;
			foreach (Event w in e.AfterIHave(EventStatus.started)) {
				PrereqSatisfied(w, e.id);
			}
			Debug.Log("Started " + e.id);
		}
	}
}
