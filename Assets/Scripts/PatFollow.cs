using UnityEngine;
using System.Collections;

[RequireComponent(typeof(NavMeshAgent))]
public class PatFollow : MonoBehaviour {

    public enum State { Idle, Chasing, Attacking };
    State currentState;

    public float hp = 3;
    public ParticleSystem deathEffect;

    NavMeshAgent pathfinder;
    Transform target;
    Life targetLife;
    Material skinMaterial;
    Color originColor;

    float attackDistanceThreshold = .5f;
    float timeBetweenAttacks = 1;
    float damage = 1;

    float nextAttackTime;
    //float myCollisionRadius;
    float targetCollisionRadius;

    bool hasTarget;

    // Use this for initialization
    protected override void Start()
    {
        base.Start();
        skinMaterial = GetComponent<Renderer>().material;
        originColor = skinMaterial.color;
        pathfinder = GetComponent<NavMeshAgent>();

        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            currentState = State.Chasing;
            hasTarget = true;
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetLife = target.GetComponent<Life>();
            targetLife.OnDeath += onTargetDeath;


            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;
            StartCoroutine(UpdatePath());
        }



    }



    void onTargetDeath()
    {
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (hasTarget)
        {
            if (Time.time > nextAttackTime)
            {
                float sqrDistToTarget = (target.position - transform.position).sqrMagnitude;
                if (sqrDistToTarget < Mathf.Pow(attackDistanceThreshold + targetCollisionRadius + targetCollisionRadius, 2))
                {
                    nextAttackTime = Time.time + timeBetweenAttacks;
                    StartCoroutine(Attack());
                }
            }
        }
    }

    IEnumerator Attack()
    {
        currentState = State.Attacking;
        pathfinder.enabled = false;
        Vector3 originPos = transform.position;
        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (targetCollisionRadius);

        float attackSpeed = 3;
        float percent = 0;
        skinMaterial.color = Color.cyan;
        bool hasAppDamage = false;
        while (percent <= 1)
        {
            if (percent <= .5f && !hasAppDamage)
            {
                hasAppDamage = true;
                targetLife.TakeDamage(damage);
            }
            percent += Time.deltaTime * attackSpeed;
            float interpolation = 4 * (-percent * percent + percent);
            transform.position = Vector3.Lerp(originPos, attackPosition, interpolation);
            yield return null;
        }
        skinMaterial.color = originColor;
        currentState = State.Chasing;
        pathfinder.enabled = true;
    }

    IEnumerator UpdatePath()
    {

        float refreshRate = .25f;
        while (hasTarget)
        {
            if (currentState == State.Chasing)
            {
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (targetCollisionRadius * 2 + attackDistanceThreshold / 2);
                pathfinder.SetDestination(targetPosition);

            }
            yield return new WaitForSeconds(refreshRate);
        }
    }

    public void Damage(float amount)
    {
        hp -= amount;
        if (hp <= 0f)
        {
            Destroy(gameObject);
            Destroy(Instantiate(deathEffect.gameObject, transform.position, Quaternion.FromToRotation(Vector3.forward, -transform.forward)) as GameObject, deathEffect.startLifetime);
            Player.enemiesKilled += 1;
        }
    }

}
