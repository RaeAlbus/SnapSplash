using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperBehavior : MonoBehaviour
{
    public static GameObject player;
    private float distanceToPlayer;

    private float distanceToEnter = 5f;

    private bool inShop;
    private Animator anim;
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        anim.SetInteger("animState", 0);
    }

    // Update is called once per frame
    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= distanceToEnter)
        {
            anim.SetInteger("animState", 1);
            inShop = true;
        } else {
            inShop = false;
        }
    }
}
