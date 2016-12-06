using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class WaitingRoomOrganizer : MonoBehaviour {

	public Transform[] destinations;

	public static WaitingRoomOrganizer inst { get; private set; }
	private Dictionary<Vector3, bool> occupiedMap;
	
	// Use this for initialization
	void Start () {
		if (inst != null) Debug.LogWarning("More than one WaitingRoomOrganizer!");
		foreach (var t in destinations) {
			occupiedMap.Add(t.position, false);
		}
	}
	
	public Vector3 GetUnoccupied() {
		return (from kvp in occupiedMap where !kvp.Value select kvp.Key).FirstOrDefault();
	}

	public void SetOccupied(Vector3 position, bool occupied) {
		if (occupiedMap.ContainsKey(position)) {
			occupiedMap[position] = occupied;
		}
	}
}
