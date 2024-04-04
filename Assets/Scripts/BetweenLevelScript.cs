using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetweenLevelScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.collider.CompareTag("UpwardsBarrier"))
        {
            LevelManager.Instance.LoadUpLevelOcean();
        }
        else if (hit.collider.CompareTag("DownwardsBarrier"))
        {
            LevelManager.Instance.LoadDownLevelOcean();
        }
    }
}
