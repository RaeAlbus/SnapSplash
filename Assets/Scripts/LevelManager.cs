using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class LevelManager : MonoBehaviour
{
    private enum Level // Added 'enum' keyword to declare an enumeration
    {
        ShallowOcean,
        MidLevelOcean,
        DeepOcean,
        Surface
    }

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

    //What level the player is currently on
    private Level currentLevel;

    // Total amount of pics the player can hold at once
    public static int totalStorage = 16;

    // Amount of pics player is currently holding
    public static int storageLeft;

    // Money the player has earned from selling fish
    public static float money;

    // Current Maximum depth player can go to
    public static float maxDepth;

    // Whether or not the player has bought the flashlight
    public static bool hasFlashlight = false;

    // Whether or not the player is using flashlight
    public bool flashlightOn;

    // Current depth of player at this second 
    private float currentDepth;

    // Whether the game is paused
    public static bool isPaused;

    // References to UI Elements
    [Header("UI Elements")]
    public Slider airUI;
    public Text warningText;
    public Text underwaterTooltip;
    public Text surfaceTooltip;
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
    public Canvas pauseCanvas;

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

    // squid
    public static SquidBehavior squidBehavior;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        squidBehavior = transform.parent.gameObject.GetComponentInChildren<SquidBehavior>();

        InitGame();

        // Conditions for surface level only set at start of game
        _instance = this;
        storageLeft = totalStorage;
        maxDepth = -20f;
        currentLevel = Level.Surface;
    }

    void Update()
    {
        if (isDiving && !isLevelLost)
        {
            UpdateAir();
            UpdateDepth();
        }

        UpdateInteractableTooltip();
        UpdateUI();

        if(Input.GetKeyDown(KeyCode.P))
        {
            PauseGame();
        }
    }

    //----------------------------------------------------SCENE SWITCHING FUNCTIONALITY----------------------------------------------

    void TeleportPlayer(Transform loc)
    {
        if (squidBehavior)
        {
            Vector3 offset = squidBehavior.transform.position - transform.position;
            squidBehavior.transform.position = loc.position + offset;
        }
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
        currentLevel = Level.ShallowOcean;
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
        currentLevel = Level.Surface;
        SoundManager.Instance.StopBreathingSFX();
        SoundManager.Instance.PlayWalkingSFX();

        UseSurfaceUI();

        Vector3 currentRotation = player.transform.rotation.eulerAngles;
        // Set the Z rotation component to 0
        currentRotation.z = 0f;
        // Set the object's rotation to the modified Euler angles
        player.transform.rotation = Quaternion.Euler(currentRotation);

        squidBehavior.EnterSurface();
    }

    void InitMidLevelOcean(bool topSpawn)
    {
        if (topSpawn)
        {
            TeleportPlayer(midTopSpawn);
        }
        else
        {
            TeleportPlayer(midBotSpawn);
        }
        LoadMidLevelOcean();
        currentLevel = Level.MidLevelOcean;
        Invoke("FindFish", 0.5f);
    }

    void InitShallowOcean(bool topSpawn)
    {
        if (topSpawn)
        {
            TeleportPlayer(shallowTopSpawn);
        }
        else
        {
            TeleportPlayer(shallowBotSpawn);
        }
        LoadShallowOcean();
        currentLevel = Level.ShallowOcean;
        Invoke("FindFish", 0.5f);
    }

    void InitDeepOcean()
    {
        TeleportPlayer(deepTopSpawn);
        LoadDeepOcean();
        currentLevel = Level.DeepOcean;
        Invoke("FindFish", 0.5f);
        squidBehavior.EnterDeepOcean();
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

    public void LevelLost()
    {
        airLeft = 0;
        playerCanvas.GetComponent<Canvas>().enabled = false;
        isLevelLost = true;
        LevelOver();
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
        //offset by for how deep the water is
        if (currentLevel == Level.ShallowOcean)
        {
            currentDepth = player.transform.position.y - shallowTopSpawn.transform.position.y;
        }
        else if (currentLevel == Level.MidLevelOcean)
        {
            currentDepth = player.transform.position.y - midTopSpawn.transform.position.y - 20f;
        }
        else if (currentLevel == Level.DeepOcean)
        {
            currentDepth = player.transform.position.y - deepTopSpawn.transform.position.y - 60f;
        }
        else
        {
            currentDepth = 0;
        }

        // If player is too deep, they lose
        if (currentDepth < maxDepth)
        {
            playerCanvas.GetComponent<Canvas>().enabled = false;
            isLevelLost = true;
            LevelOver();
        }
        else if (currentDepth <= maxDepth + 5f)
        {
            // Warn player that they are getting too deep
            warningText.gameObject.SetActive(true);
            warningText.text = "Youre too deep! Go back up!";
        }
        else
        {
            warningText.gameObject.SetActive(false);
        }
    }

    public void UpdateInteractableTooltip()
    {
        GameObject[] interactables = GameObject.FindGameObjectsWithTag("Interactable");
        bool showTooltip = false;

        foreach (GameObject interactable in interactables)
        {
            if (Vector3.Distance(player.transform.position, interactable.transform.position) < 5f)
            {
                showTooltip = true;
            }
        }
        
        if (showTooltip)
        {
            if (currentLevel == Level.Surface && !ShopKeeperBehavior.inShop)
            {
                surfaceTooltip.gameObject.SetActive(true);
                surfaceTooltip.text = "Press 'E' to interact";
            }
            else
            {
                underwaterTooltip.gameObject.SetActive(true);
                underwaterTooltip.text = "Press 'E' to interact";
            }
        }
        else
        {
            surfaceTooltip.gameObject.SetActive(false);
            underwaterTooltip.gameObject.SetActive(false);
        }
    }

    public void UseCameraUI()
    {
        cameraCanvas.GetComponent<Canvas>().enabled = true;

        playerCanvas.GetComponent<Canvas>().enabled = false;
        surfaceCanvas.GetComponent<Canvas>().enabled = false;
        pauseCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void UsePlayerUI()
    {
        playerCanvas.GetComponent<Canvas>().enabled = true;

        cameraCanvas.GetComponent<Canvas>().enabled = false;
        surfaceCanvas.GetComponent<Canvas>().enabled = false;
        pauseCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void UseSurfaceUI()
    {
        surfaceCanvas.GetComponent<Canvas>().enabled = true;

        cameraCanvas.GetComponent<Canvas>().enabled = false;
        playerCanvas.GetComponent<Canvas>().enabled = false;
        pauseCanvas.GetComponent<Canvas>().enabled = false;
    }

    public void SwitchPauseMenu(bool isPaused)
    {   

        if(isPaused)
        {
            pauseCanvas.GetComponent<Canvas>().enabled = true;

            surfaceCanvas.GetComponent<Canvas>().enabled = false;
            cameraCanvas.GetComponent<Canvas>().enabled = false;
            playerCanvas.GetComponent<Canvas>().enabled = false;
        } 
        else if(!isDiving)
        {
            UseSurfaceUI();
        }
        else
        {
            UsePlayerUI();
        }

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

    void PauseGame()
    {
        if(LevelManager.isPaused)
        {
            isPaused = false;
            Time.timeScale = 1f;

            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        } 
        else
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            isPaused = true;
            Time.timeScale = 0f;
        }

        SwitchPauseMenu(isPaused);
    }
}