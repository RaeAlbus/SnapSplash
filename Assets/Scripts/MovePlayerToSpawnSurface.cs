using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePlayerToSpawnSurface : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    private void Update()
    {
        if (LevelManager.player.transform.position.x > -100)
        {
            LevelManager.player.transform.position = new Vector3(-218.1f, 3.84f, 135.3f);
        }
    }
}
