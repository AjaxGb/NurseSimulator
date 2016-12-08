using UnityEngine;
using System.Collections;

//[RequireComponent(typeof(NavMeshAgent))]
public class PatFollow : MonoBehaviour {
    public bool escort = false;
    Transform target; //the patient's target
    float moveSpeed = 3; //move speed
    float rotationSpeed = 3; //speed of turning
    float range = 30f;
    float range2 = 30f;
    float stop = 5;
    Transform myTransform; //current transform data of this enemy
    void Awake()
    {
        myTransform = transform; //cache transform data for easy access/preformance
    }

    void Start()
    {
        target = GameObject.FindWithTag("Player").transform; //target the player

    }

    void Update()
    {
        if (escort == true)
        {
            //rotate to look at the player
            var distance = Vector3.Distance(myTransform.position, target.position);
            if (distance <= range2 && distance >= range)
            {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
            }


            else if (distance <= range && distance > stop)
            {

                //move towards the player
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
                myTransform.position += myTransform.forward * moveSpeed * Time.deltaTime;
            }
            else if (distance <= stop)
            {
                myTransform.rotation = Quaternion.Slerp(myTransform.rotation,
                Quaternion.LookRotation(target.position - myTransform.position), rotationSpeed * Time.deltaTime);
            }


        }
    }
}
