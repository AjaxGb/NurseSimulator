using System;
using System.Collections.Generic;
using UnityEngine;

public enum Gender { male, female } // FOR THE PURPOSES OF THIS PROJECT

[Serializable]
public class PatientData : ISerializationCallbackReceiver {
	public string name;
	public string gender;
	public Color color;

	[NonSerialized]
	public Gender parsedGender;
	public void OnBeforeSerialize() {
		gender = Enum.GetName(typeof(Gender), parsedGender);
	}
	public void OnAfterDeserialize() { // Loaded topic |
		try {                          //              V
			parsedGender = (gender == null) ? default(Gender) : (Gender)Enum.Parse(typeof(Gender), gender, true);
		} catch (ArgumentException e) {
			throw new FormatException(gender + " is not a valid Gender", e); // LOADED TOPIC
		}
	}
}

public class Patient : MonoBehaviour, IDespawnEvents {

	public PatientData data { get; private set; }
	private NavMeshAgent navAgent;

	private ICollection<Action> _despawnActions = new List<Action>();
	public void AddDespawnAction(Action a) {
		_despawnActions.Add(a);
	}
	public bool RemoveDespawnAction(Action a) {
		return _despawnActions.Remove(a);
	}

	// NOT Start(), this is called manually on spawn
	public void Setup(PatientData data) {
		this.data = data;
		navAgent = GetComponent<NavMeshAgent>();
	}

	// Update is called once per frame
	void Update () {
		navAgent.destination = Player.inst.transform.position;
	}

	void OnDestroy() {
		// Move this if we do not destroy patients after they are done;
		foreach (Action a in _despawnActions) a();
	}
}
