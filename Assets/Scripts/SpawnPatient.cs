using UnityEngine;
using System.Collections;

public class SpawnPatient : MonoBehaviour {
    public GameObject NPC;
    public Transform[] SpawnPoints;
    public float SpawnTime = 3f;
    public GameObject[] patients;

    void Start() {
        InvokeRepeating("Spawn", SpawnTime, SpawnTime);
    }
    
    void Spawn () {
        int SpawnPointIndex = Random.Range(0, SpawnPoints.Length);
        Instantiate(NPC, SpawnPoints[SpawnPointIndex].position, SpawnPoints[SpawnPointIndex].rotation);
    }
}
