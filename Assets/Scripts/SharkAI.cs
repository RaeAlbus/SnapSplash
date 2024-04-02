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
        Attack
    }

    public FSMStates currentState;

    public float attackDistance = 5.0f;
    public float chaseDistance = 10.0f;
    public float sharkSpeed = 2.0f;
    public float chaseSpeed = 5.0f;
    public float biteRate = 1.0f;
    public float stunDuration = 10.0f;

    public GameObject player;

    GameObject[] wanderPoints;
    Vector3 nextDestination;
    float distanceToPlayer;
    Animator anim;
    float elapsedTime;

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
        }

        elapsedTime += Time.deltaTime;
    }

    void UpdatePatrolState()
    {
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
        nextDestination = player.transform.position;

        if (distanceToPlayer <= attackDistance)
        {
            currentState = FSMStates.Attack;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }
        
        transform.LookAt(nextDestination);
        transform.position = Vector3.MoveTowards(transform.position, nextDestination, chaseSpeed * Time.deltaTime);
    }

    void UpdateAttackState()
    {
        if (distanceToPlayer > attackDistance)
        {
            currentState = FSMStates.Chase;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            currentState = FSMStates.Patrol;
        }

        transform.LookAt(nextDestination);
        Bite();
    }

    void FindNextPoint()
    {
        nextDestination = wanderPoints[Random.Range(0, wanderPoints.Length)].transform.position;
    }

    void Bite()
    {
        if (elapsedTime >= biteRate)
        {
            int damageAmount = Random.Range(1, 4);
            LevelManager.Instance.LoseAir(damageAmount);
        }
    }

    public void StunShark()
    {
        StartCoroutine(Stun());
    }

    IEnumerator Stun()
    {
        currentState = FSMStates.Patrol;
        sharkSpeed = 0;
        yield return new WaitForSeconds(stunDuration);
        sharkSpeed = 2;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, chaseDistance);
    }

}
