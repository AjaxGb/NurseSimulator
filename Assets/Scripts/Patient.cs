﻿using System;
using System.Collections.Generic;
using System.Linq;
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
    [NonSerialized]
    public List<ItemType> requiredItemTypes;
	public void OnBeforeSerialize() {
		gender = Enum.GetName(typeof(Gender), parsedGender);
        required_items = (from it in requiredItemTypes select it.name).ToArray();
	}
	public void OnAfterDeserialize() { // Loaded topic |
		try {                          //              V
			parsedGender = (gender == null) ? default(Gender) : (Gender)Enum.Parse(typeof(Gender), gender, true);
		} catch (ArgumentException e) {
			throw new FormatException(gender + " is not a valid Gender", e); // LOADED TOPIC
		}
        requiredItemTypes = new List<ItemType>();
        if (required_items != null)
        {
            foreach (var name in required_items)
            {
                requiredItemTypes.Add(ItemType.FromName(name));
            }
        }
	}
}

public class Patient : MonoBehaviour, IDespawnEvents, IMouseOverUI {
    public RectTransform popupPrefab;
    private RectTransform currPopup;

    public PatientData data { get; private set; }
	private NavMeshAgent navAgent;

    public bool follow = false;
	public Vector3 destination;

	private ICollection<Action> _despawnActions;
	public void AddDespawnAction(Action a) {
		if (_despawnActions == null) _despawnActions = new List<Action>();
		_despawnActions.Add(a);
	}
	public bool RemoveDespawnAction(Action a) {
		return _despawnActions != null && _despawnActions.Remove(a);
	}

	// NOT Start(), this is called manually on spawn
	public void Setup(PatientData data) {
		this.data = data;
		navAgent = GetComponent<NavMeshAgent>();
		destination = WaitingRoomOrganizer.inst.OccupyUnoccupied();
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
		if (_despawnActions != null) {
			foreach (Action a in _despawnActions) a();
		}
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

   

