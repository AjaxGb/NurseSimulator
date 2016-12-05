using UnityEngine;
using System.Collections;

public class SpawnPatient : MonoBehaviour {
    public GameObject NPC;
    public Transform SpawnPoint;
    public float SpawnTime = 3f;
    public int MaxPatients = 10;
    public GameObject[] patients;
    private int i = 0;

    void Start() {
        patients = new GameObject[MaxPatients];
        InvokeRepeating("Spawn", SpawnTime, SpawnTime);
    }
    
    void Spawn () {
        if (i < MaxPatients)
        {
            patients[i] = (GameObject)Instantiate(NPC, SpawnPoint.position, SpawnPoint.rotation);
            ++i;
        }
    }
}
