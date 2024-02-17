using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : MonoBehaviour
{

    public float fishValue = 100.0f;

    public float moveSpeed = 1.0f;
    public float rotateSpeed = 1.0f;

    public float minPauseDuration = 1.0f;

    public float minMoveDuration = 1.0f;
    public float maxMoveDuration = 5.0f;

    private bool isMoving = false;
    private Vector3 targetPosition;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Swim());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator Swim()
    {
        while (true)
        {
            if (!isMoving)
            {
                isMoving = true;
                yield return new WaitForSeconds(minPauseDuration);
                targetPosition = new Vector3(Random.Range(-10.0f, 10.0f), 0.0f, Random.Range(-10.0f, 10.0f));
                float moveDuration = Random.Range(minMoveDuration, maxMoveDuration);
                float elapsedTime = 0.0f;
                Vector3 startPosition = transform.position;
                Quaternion startRotation = transform.rotation;
                while (elapsedTime < moveDuration)
                {
                    transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / moveDuration);
                    transform.rotation = Quaternion.Slerp(startRotation, Quaternion.LookRotation(targetPosition - transform.position), elapsedTime / moveDuration);
                    elapsedTime += Time.deltaTime;
                    yield return null;
                }
                transform.position = targetPosition;
                isMoving = false;
            }
            yield return null;
        }
    }
}
