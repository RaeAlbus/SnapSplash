using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkAI : MonoBehaviour
{

    public int sharkValue = 200;

    public enum FSMStates
    {
        Patrol,
        Chase,
        Attack,
        Stunned
    }

    public FSMStates currentState;

    public float attackDistance = 4.0f;
    public float chaseDistance = 15.0f;
    public float chaseAfterAttackDistance = 7.0f;
    public float sharkSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float stunDuration = 5.0f;

    public GameObject player;

    GameObject[] wanderPoints;
    Vector3 nextDestination;
    float distanceToPlayer;
    Animator anim;
    float elapsedTime;
    float biteRate;

    // Start is called before the first frame update
    void Start()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("FishMovementPoints");
        anim = GetComponent<Animator>();
        player = GameObject.FindGameObjectWithTag("Player");
        currentState = FSMStates.Patrol;
        FindNextPoint();
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        switch (currentState)
        {
            case FSMStates.Patrol:
                UpdatePatrolState();
                break;
            case FSMStates.Chase:
                UpdateChaseState();
                break;
            case FSMStates.Attack:
                UpdateAttackState();
                break;
            case FSMStates.Stunned:
                UpdateStunnedState();
                break;
        }

        elapsedTime += Time.deltaTime;
    }

    void UpdatePatrolState()
    {
        anim.SetInteger("animState", 0);

        if (Vector3.Distance(transform.position, nextDestination) < 1.0f)
        {
            FindNextPoint();
        }
        else if (distanceToPlayer <= chaseDistance)
        {
            currentState = FSMStates.Chase;
        }
        
        transform.LookAt(nextDestination);
        transform.position = Vector3.MoveTowards(transform.position, nextDestination, sharkSpeed * Time.deltaTime);
    }

    void UpdateChaseState()
    {
        anim.SetInteger("animState", 0);

        nextDestination = player.transform.position;
        transform.LookAt(nextDestination);

        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }
    
        transform.position = Vector3.MoveTowards(transform.position, nextDestination, chaseSpeed * Time.deltaTime);
    }

    void UpdateAttackState()
    {
        anim.SetInteger("animState", 1);

        nextDestination = player.transform.position;
        transform.LookAt(nextDestination);

        if (distanceToPlayer > chaseAfterAttackDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        Attack();
    }

    void UpdateStunnedState()
    {
        if (elapsedTime >= stunDuration)
        {
            currentState = FSMStates.Patrol;
            elapsedTime = 0.0f;
        }

        anim.SetInteger("animState", 2);
    }

    void FindNextPoint()
    {
        nextDestination = wanderPoints[Random.Range(0, wanderPoints.Length)].transform.position;
    }

    void Attack()
    {
        biteRate = anim.GetCurrentAnimatorStateInfo(0).length;
        if (elapsedTime >= biteRate)
        {
            var animDuration = anim.GetCurrentAnimatorStateInfo(0).length;
            Invoke("Bite", animDuration);
            elapsedTime = 0.0f;
        }
    }

    void Bite()
    {
        if (currentState == FSMStates.Attack)
        {
            int damageAmount = Random.Range(1, 4);
            LevelManager.Instance.LoseAir(damageAmount);
        }
    }

    public void StunShark()
    {
        if (distanceToPlayer <= chaseDistance && currentState != FSMStates.Stunned)
        {
            currentState = FSMStates.Stunned;
            elapsedTime = 0.0f;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

}
