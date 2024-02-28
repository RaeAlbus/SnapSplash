using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //Whether the player is currently on a dive
    public static bool isDiving;

    // The total amount of air the player has for this level at the start
    public float totalAir = 20f;

    // Amount of air left at this second in level
    private float airLeft;

    // Whether or not level is over
    public static bool isLevelLost;

    // Total amount of pics the player can hold at once
    public int totalStorage = 16;

    // Amount of pics player is currently holding
    public static int storageLeft;

    // Maximum depth for this level
    public float maxDepth;

    // Current depth of player at this second 
    private float currentDepth;

    /*// References to UI Elements
    public Slider airUI;
    public Text storageUI;
    public Text storageUICamera;
    public Text depthUI;

    // Canvases: One for when camera is not equipped and one for when it is
    public Canvas playerCanvas;
    public Canvas cameraCanvas;*/

    void Start()
    {
    //    UsePlayerUI();
        storageLeft = totalStorage;
        airLeft = totalAir;

        DontDestroyOnLoad(gameObject);
    }

    void Update()
    {
     //   UpdateUI();

        if(isDiving)
        {
            UpdateAir();
            //TODO: UpdateDepth();
        }

        // TODO: LOGIC WHEN PLAYER REACHES ANCHORPOINT
        // LevelSuccess();
    }

  /*  void UpdateUI()
    {
        depthUI.text = "" + currentDepth;
        airUI.value = airLeft / totalAir;
        storageUI.text = "" + storageLeft;
        storageUICamera.text = "" + storageLeft;
    }*/

    void UpdateAir()
    {
        if (airLeft > 0)
        {
            airLeft -= Time.deltaTime;
        }
        else
        {
            airLeft = 0;
            LevelLost();
        }
    }
/*
    public void UseCameraUI()
    {
        playerCanvas.gameObject.SetActive(false);
        cameraCanvas.gameObject.SetActive(true);
    }

    public void UsePlayerUI()
    {
        cameraCanvas.gameObject.SetActive(false);
        playerCanvas.gameObject.SetActive(true);
    }
*/
    void LevelSuccess()
    {
        // Load the surface level after reaching anchorpoint
        Invoke("SwitchScene", 2);
    }

    void LevelLost()
    {
        isLevelLost = true;
        // Switch back to Surface after passing out
        Invoke("SwitchScene", 2);

    }

    public void SwitchScene()
    {
        if (isLevelLost)
        {
            storageLeft = totalStorage;
        }
        if (isDiving)
        {
            SceneManager.LoadScene("Surface");
        }
        else
        {
            SceneManager.LoadScene("ShallowOcean");
        }
        isDiving = !isDiving;
    }
}
  