using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    
    // The total amount of air the player has for this level at the start
    public float totalAir = 20;

    // Amount of air left at this second in level
    private float airLeft;

    // Whether or not level is over
    private bool isLevelOver;

    // Total amount of pics the player can hold at once
    public int totalStorage = 16;

    // Amount of pics player is currently holding
    public static int storageLeft;

    // Maximum depth for this level
    public float maxDepth;

    // Current depth of player at this second 
    private float currentDepth;

    // References to UI Elements
    public Slider airUI;
    public Text storageUI;
    public Text storageUICamera;
    public Text depthUI;

    // Canvases: One for when camera is not equipped and one for when it is
    public Canvas playerCanvas;
    public Canvas cameraCanvas;

    // Surface scene to load when player leaves the water
    public string surfaceScene;

    void Start()
    {
        UsePlayerUI();
        storageLeft = totalStorage;
        airLeft = totalAir;
    }

    void Update()
    {
        UpdateUI();

        if(airLeft > 0)
        {
            airLeft -= Time.deltaTime;
        } else {
            airLeft = 0;
            LevelLost();
        }

        if(currentDepth < 0 && airLeft != 0)
        {
            LevelSuccess();
        }


    }

    void UpdateUI()
    {
        depthUI.text = "" + currentDepth;
        airUI.value = airLeft / totalAir;
        storageUI.text = "" + storageLeft;
        storageUICamera.text = "" + storageLeft;
    }

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

    void LevelSuccess()
    {
        isLevelOver = true;

        // Load the surface level after reaching anchorpoint
        Invoke("LoadSurfaceScene", 2);
    }

    void LevelLost()
    {
        isLevelOver = true;

        // Load the surface level after losing air
        Invoke("LoadSurfaceScene", 2);

    }

    void LoadSurfaceScene()
    {
        SceneManager.LoadScene(surfaceScene);
    }
}
  