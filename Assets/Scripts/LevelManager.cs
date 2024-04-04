using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{
    // Singleton instance of the LevelManager
    private static LevelManager _instance;

    // Read-only instant of LevelManger to access from other scripts
    public static LevelManager Instance => _instance;

    //Whether the player is currently on a dive
    public static bool isDiving;

    // The total amount of air the player has for this level at the start
    [Header("Level Info")]
    public static float totalAir = 30f;

    // Amount of air left at this second in level
    public static float airLeft;

    // Whether or not level is over
    public static bool isLevelLost;

    // Total amount of pics the player can hold at once
    public static int totalStorage = 16;

    // Amount of pics player is currently holding
    public static int storageLeft;

    // Money the player has earned from selling fish
    public static float money;

    // Current Maximum depth player can go to
    public float maxDepth;

    // Current depth of player at this second 
    private float currentDepth;

    // References to UI Elements
    [Header("UI Elements")]
    public Slider airUI;
    public Text warningText;
    public Text storageUI;
    public Text storageUICamera;
    public Text storageUISurface;
    public Text depthUI;
    public Text moneyUI;

    // Canvases: One for when camera is not equipped and one for when it is
    [Header("Canvases")]
    public Canvas playerCanvas;
    public Canvas cameraCanvas;
    public Canvas surfaceCanvas;

    public static GameObject player;
    [Header("Spawnpoints")]
    public Transform shallowTopSpawn;
    public Transform shallowBotSpawn;
    public Transform midTopSpawn;
    public Transform midBotSpawn;
    public Transform deepTopSpawn;
    public Transform surfaceAnchorSpawn;

    // the amount of money player will receive from selling fish
    public static float totalFishValue;

    // fish in current scene
    public static GameObject[] fishInScene;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        InitGame();

        // Conditions for surface level only set at start of game
        _instance = this;
        storageLeft = totalStorage;
        maxDepth = -18f;
    }

    void Update()
    {
        if (isDiving && !isLevelLost)
        {
            UpdateAir();
            UpdateDepth();
        }

        UpdateUI();
    }

    //----------------------------------------------------SCENE SWITCHING FUNCTIONALITY----------------------------------------------

    void TeleportPlayer(Transform loc)
    {
        CharacterController c = player.GetComponent<CharacterController>();
        c.enabled = false;
        player.transform.position = loc.position;
        player.transform.rotation = loc.rotation;
        Physics.SyncTransforms();
        c.enabled = true;
    }

    void InitOceanLevel()
    {
        isDiving = true;
        airLeft = totalAir;
        isLevelLost = false;
        TeleportPlayer(shallowTopSpawn);
        SoundManager.Instance.PlaySplashSFX();
        SoundManager.Instance.StopWalkingSFX();
        SoundManager.Instance.PlayBreathingSFX();

        UsePlayerUI();
    }

    void InitGame()
    {

        isDiving = false;
        airLeft = totalAir;
        isLevelLost = false;
        SoundManager.Instance.PlayWalkingSFX();
        UseSurfaceUI();
    }

    void InitSurfaceLevel()
    {
        // Plays splash sfx only if going to surface from
        // previously diving
        if (isDiving)
        {
            SoundManager.Instance.PlaySplashSFX();
        }

        isDiving = false;
        airLeft = totalAir;
        isLevelLost = false;
        TeleportPlayer(surfaceAnchorSpawn);
        SoundManager.Instance.StopBreathingSFX();
        SoundManager.Instance.PlayWalkingSFX();

        UseSurfaceUI();
    }

    void InitMidLevelOcean(bool topSpawn)
    {
        if (topSpawn)
        {
            TeleportPlayer(midTopSpawn);
        } else
        {
            TeleportPlayer(midBotSpawn);
        }
        LoadMidLevelOcean();
        Invoke("FindFish", 0.5f);
    }

    void InitShallowOcean(bool topSpawn)
    {
        if (topSpawn)
        {
            TeleportPlayer(shallowTopSpawn);
        } else
        {
            TeleportPlayer(shallowBotSpawn);
        }
        LoadShallowOcean();
        Invoke("FindFish", 0.5f);
    }

    void InitDeepOcean()
    {
        TeleportPlayer(shallowTopSpawn);
        LoadDeepOcean();
        Invoke("FindFish", 0.5f);
    }

    public void LoadUpLevelOcean()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "MidLevelOcean")
        {
            InitShallowOcean(false);
        }
        else if (sceneName == "DeepOcean")
        {
            InitMidLevelOcean(false);
        }
    }

    public void LoadDownLevelOcean()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName == "ShallowOcean")
        {
            InitMidLevelOcean(true);
        }
        else if (sceneName == "MidLevelOcean")
        {
            InitDeepOcean();
        }
    }

    void LevelOver()
    {
        // Go back for air and switch scenes
        airLeft = totalAir;
        totalFishValue = 0;
        storageLeft = totalStorage;
        Invoke("SwitchScene", 1);
    }

    public void SwitchScene()
    {
        if (isDiving)
        {
            LoadSurface();
            InitSurfaceLevel();
        }
        else
        {
            LoadShallowOcean();
            InitOceanLevel();
            Invoke("FindFish", 0.5f);
        }
    }

    private void LoadSurface()
    {
        SceneManager.LoadScene("Surface");
    }

    private void LoadShallowOcean()
    {
        SceneManager.LoadScene("ShallowOcean");
    }

    private void LoadMidLevelOcean()
    {
        SceneManager.LoadScene("MidLevelOcean");
    }

    private void LoadDeepOcean()
    {
        SceneManager.LoadScene("DeepOcean");
    }


    //-------------------------------------------------------------UI FUNCTIONALITY---------------------------------------------------------

    void UpdateUI()
    {
        depthUI.text = "" + Mathf.Abs(Mathf.Floor(currentDepth));
        airUI.value = airLeft / totalAir;
        storageUI.text = "" + storageLeft;
        storageUICamera.text = "" + storageLeft;
        storageUISurface.text = "" + storageLeft;
        moneyUI.text = "" + money;
    }

    void UpdateAir()
    {
        if (airLeft > 0)
        {
            airLeft -= Time.deltaTime;
        }
        else
        {
            airLeft = 0;
            playerCanvas.GetComponent<Canvas>().enabled = false;
            isLevelLost = true;
            LevelOver();
        }
    }

    void UpdateDepth()
    {
        currentDepth = player.transform.position.y - shallowTopSpawn.transform.position.y;
        if (currentDepth < maxDepth)
        {
            playerCanvas.GetComponent<Canvas>().enabled = false;
            isLevelLost = true;
            LevelOver();
        }
        else if (Mathf.Abs(maxDepth) - Mathf.Abs(currentDepth) <= 5)
        {
            warningText.gameObject.SetActive(true);
            warningText.text = "Youre too deep! Go back up!";
        }
        else
        {
           warningText.gameObject.SetActive(false);
        }
    }

    public void UseCameraUI()
    {
        cameraCanvas.GetComponent<Canvas>().enabled = true;

        playerCanvas.GetComponent<Canvas>().enabled = false;
        surfaceCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void UsePlayerUI()
    {
        playerCanvas.GetComponent<Canvas>().enabled = true;

        cameraCanvas.GetComponent<Canvas>().enabled = false;
        surfaceCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void UseSurfaceUI()
    {
        surfaceCanvas.GetComponent<Canvas>().enabled = true;

        cameraCanvas.GetComponent<Canvas>().enabled = false;
        playerCanvas.GetComponent<Canvas>().enabled = false;
    }

    //----------------------------------------------------PLAYER VALUES FUNCTIONALITY----------------------------------------------

    public void addFishValue(float value)
    {
        totalFishValue += value;
    }

    void FindFish()
    {
        fishInScene = GameObject.FindGameObjectsWithTag("Fish");
        GameObject[] sharksInScene = GameObject.FindGameObjectsWithTag("Shark");
        fishInScene = fishInScene.Concat(sharksInScene).ToArray();
    }

    public void LoseAir(float airLoss)
    {
        airLeft -= airLoss;
    }

}