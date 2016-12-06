using System;
using System.Collections.Generic;
using UnityEngine;

public enum Gender { male, female } // FOR THE PURPOSES OF THIS PROJECT

[Serializable]
public class PatientData : ISerializationCallbackReceiver {
    public string name;
	public string gender;
    public string condition;
    public float deathChance;
    public string[] required_items;
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

public class Patient : MonoBehaviour, IDespawnEvents, IMouseOverUI {
    public RectTransform popupPrefab;
    private RectTransform currPopup;

    public PatientData data { get; private set; }
	private NavMeshAgent navAgent;

    public bool follow = false;
    public Vector3 destination = GameObject.FindWithTag("destination").transform.position;

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
        if (follow)
        {
            navAgent.destination = Player.inst.transform.position;
        }
        else
        {
            navAgent.destination = destination;
        }
	}

	void OnDestroy() {
		// Move this if we do not destroy patients after they are done;
		foreach (Action a in _despawnActions) a();
	}

    public void ShowUI(Transform parent, Camera camera)
    {
        if (currPopup != null)
        {
            HideUI();
        }
        if (popupPrefab != null)
        {
            currPopup = Instantiate(popupPrefab);
            currPopup.transform.SetParent(parent);
        }
    }
    public void UpdateUI(Camera camera, Vector3 point)
    {
        if (currPopup != null)
        {
            currPopup.transform.position = RectTransformUtility.WorldToScreenPoint(camera, point);
        }
    }

    public void HideUI()
    {
        if (currPopup != null)
        {
            Destroy(currPopup.gameObject);
        }
    }

   


    public void OnClick(int button)
    {
        follow = true;
        Player.inst.escortee = this;
    }

    
}

   

