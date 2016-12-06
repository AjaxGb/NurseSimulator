using UnityEngine;

public class Spawnpoint : MonoBehaviour, IWeighted {

	public string id;
	public int weight;

	public int Weight { get { return weight; } }
}
