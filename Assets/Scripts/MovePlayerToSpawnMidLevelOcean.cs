using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToSpawnMidLevelOcean : MonoBehaviour
{
    void Start()
    {
        LevelManager.player.transform.position = new Vector3(-2.27f, 17.62f, 1.67f);
    }

    private void Update()
    {
        if (LevelManager.player.transform.position.x > 0)
        {
            LevelManager.player.transform.position = new Vector3(-2.27f, 17.62f, 1.67f);
        }
    }
}
