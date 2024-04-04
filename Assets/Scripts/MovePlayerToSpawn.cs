using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToSpawn : MonoBehaviour
{
    void Start()
    {
        LevelManager.player.transform.position = new Vector3(3.2f, -1.3f, 2.2f);
    }

    private void Update()
    {
        if (LevelManager.player.transform.position.x < -200 || LevelManager.player.transform.position.y > 3f)
        {
            LevelManager.player.transform.position = new Vector3(3.2f, -1.3f, 2.2f);
        }
    }
}
