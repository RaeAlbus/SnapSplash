using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AnchorTravel : MonoBehaviour
{

    public float DISTANCE_TO_INTERACT;
    private GameObject player;

    void Start()
    {
        player = GameObject.FindWithTag("Player");
    }

    void Update()
    {
        // Switch To Surface if close to switch interactable
        if (Input.GetKeyDown(KeyCode.E))
        {
            float distanceToSwitch = Vector3.Distance(player.transform.position, transform.position);

            if (distanceToSwitch < DISTANCE_TO_INTERACT)
            {
                LevelManager.Instance.SwitchScene();
            }
        }
    }

}
