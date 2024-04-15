using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{

    public float fishValue = 100.0f;

    public float minPauseDuration = 5.0f;

    private bool isMoving = false;

    public float fishSpeed = 10;

    private GameObject[] wanderPoints;

    private Vector3 nextDestination;

    private bool photographed = false;

    // Start is called before the first frame update
    void Start()
    {
        wanderPoints = GameObject.FindGameObjectsWithTag("FishMovementPoints");
        StartCoroutine(Swim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Swim()
    {
        while(true)
        {
            nextDestination = wanderPoints[Random.Range(0, wanderPoints.Length)].transform.position;

            transform.LookAt(nextDestination);

            float distance = Vector3.Distance(transform.position, nextDestination);
            float moveDuration = distance / fishSpeed;

            float elaspedTime = 0f;
            Vector3 startPos = transform.position;
            while(elaspedTime < moveDuration)
            {
                transform.position = Vector3.Lerp(startPos, nextDestination, elaspedTime / moveDuration);
                elaspedTime += Time.deltaTime;
                yield return null;
            }

            // Pause before selecting the next destination
            float pauseDuration = Random.Range(minPauseDuration, moveDuration);
            yield return new WaitForSeconds(pauseDuration);

        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        isMoving = false;
        StartCoroutine(Swim());
    }

    public void MarkAsPhotographed()
    {
        photographed = true;
    }

    public bool IsPhotographed()
    {
        return photographed;
    }
}
