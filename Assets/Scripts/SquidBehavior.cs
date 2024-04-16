using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquidBehavior : MonoBehaviour
{
    private bool chasingPlayer = false;
    private Transform player;
    private Vector3 spawnPoint;
    private SkinnedMeshRenderer[] r;

    public float moveSpeed = 5f;
    public bool squidEnabled = false;

    private void Start()
    {
        spawnPoint = transform.position;
        r = GetComponentsInChildren<SkinnedMeshRenderer>();
        foreach (SkinnedMeshRenderer renderer in r)
        {
            renderer.enabled = false;
        }
        chasingPlayer = false;
        squidEnabled = false;
    }

    void Update()
    {
        if (player == null)
        {
            player = LevelManager.player.transform;
        }
        if (chasingPlayer)
        {
            if (player ==  null)
            {
                player = LevelManager.player.transform;
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, player.position, moveSpeed * Time.deltaTime);
                Vector3 direction = player.position - transform.position;
                transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void StartChase()
    {
        chasingPlayer = true;
        foreach (SkinnedMeshRenderer renderer in r)
        {
            renderer.enabled = true;
        }
        squidEnabled = true;
    }

    public void EnterDeepOcean()
    {
        if (!chasingPlayer)
        {
            foreach (SkinnedMeshRenderer renderer in r)
            {
                renderer.enabled = true;
            }
            transform.position = spawnPoint;
            squidEnabled = true;
        }
    }

    public void EnterSurface()
    {
        foreach (SkinnedMeshRenderer renderer in r)
        {
            renderer.enabled = false;
        }
        transform.position = spawnPoint;
        chasingPlayer = false;
        squidEnabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (squidEnabled && other.CompareTag("Player"))
        {
            LevelManager.Instance.LevelLost();
        }
    }
}
