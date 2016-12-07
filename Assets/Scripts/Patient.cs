using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[Serializable]
public class PatientData : ISerializationCallbackReceiver {
    public string name;
    public string condition;
    public float death_time;
	public float treatment_time;
    public string[] required_items;

	[NonSerialized]
	public Material skinMaterial;
    [NonSerialized]
    public List<ItemType> requiredItemTypes;
	public void OnBeforeSerialize() {
        required_items = (from it in requiredItemTypes select it.name).ToArray();
	}
	public void OnAfterDeserialize() {
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
	public CheckAvailable room;
	private NavMeshAgent navAgent;
	private Animator anim;

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
		anim = GetComponent<Animator>();
		if (data.skinMaterial != null) GetComponentInChildren<Renderer>().material = data.skinMaterial;
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

		if (navAgent.desiredVelocity.sqrMagnitude > 0.01) {
			anim.SetBool("Walking", true);
		} else {
			anim.SetBool("Walking", false);
			Vector3 lookPos = navAgent.destination - transform.position;
			lookPos.y = 0;
			transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookPos), Time.deltaTime * 2);
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
		if (room != null) return;
		if (button == 0) {
			follow = true;
			WaitingRoomOrganizer.inst.SetOccupied(destination, false);
			Player.inst.escortee = this;
		} else {
			follow = false;
			destination = WaitingRoomOrganizer.inst.OccupyUnoccupied();
			if (Player.inst.escortee == this) {
				Player.inst.escortee = null;
			}
		}
    }
}

   

