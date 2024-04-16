using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopKeeperBehavior : MonoBehaviour
{
    // Inspector vars
    public static GameObject player;

    // How far away player must be to trigger "shopping state"
    public float distanceToEnter = 2f;

    // Current distance from shopkeeper to player
    private float distanceToPlayer;

    // Triggered when player is within distanceToEnter
    public static bool inShop;
    
    // ShopKepper animator
    private Animator anim;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        anim = GetComponent<Animator>();

        anim.SetInteger("animState", 0);
        inShop = false;
    }

    void Update()
    {
        distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

        if(distanceToPlayer <= distanceToEnter && Input.GetKeyDown(KeyCode.E) && !inShop)
        {
            // Initializes waving animation
            anim.SetInteger("animState", 1);
            inShop = true;

            // Starts the introduction dialouge
            ShopKeeperUI.Instance.InitDialouge();
        } 
        else if(!inShop)
        {
            // Reverts back to idle animation
            anim.SetInteger("animState", 0);
            inShop = false;

            // Starts outro dialouge
            ShopKeeperUI.Instance.ExitDialouge();
        }
    }

    public void SellFishPics()
    {
        if(LevelManager.storageLeft != LevelManager.totalStorage)
        {
            ShopKeeperUI.Instance.SoldFishDialouge(LevelManager.totalFishValue);

            SoundManager.Instance.PlayCashSFX();
            LevelManager.storageLeft = LevelManager.totalStorage;
            LevelManager.money = LevelManager.money + LevelManager.totalFishValue;
            PlayerPrefs.SetFloat("CoinsCollected", PlayerPrefs.GetFloat("CoinsCollected", 0) + LevelManager.totalFishValue);
            LevelManager.totalFishValue = 0;
        } 
        else 
        {
            ShopKeeperUI.Instance.NoFishDialouge();
            SoundManager.Instance.PlayNoStorageSFX();
        }
    }

    public void BuyItems()
    {
        ShopKeeperUI.Instance.BuyItemDialouge();
    }
}
