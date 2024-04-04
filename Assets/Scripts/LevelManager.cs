using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

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
    public static float totalAir = 20f;

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

    // Maximum depth for this level
    public float maxDepth;

    // Current depth of player at this second 
    private float currentDepth;

    // References to UI Elements
    [Header("UI Elements")]
    public Slider airUI;
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
    public Vector3 OceanSpawnPos;
    public Vector3 SurfaceSpawnPos;
    public Vector3 SurfaceSpawnRot;

    // the amount of money player will receive from selling fish
    public static float totalFishValue;

    // fish in current scene
    public static GameObject[] fishInScene;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        OceanSpawnPos = new Vector3(3.2f, -1.3f, 2.2f);
        SurfaceSpawnPos = new Vector3(-218.1f, 3.84f, 135.3f);
        SurfaceSpawnRot = new Vector3(0f, -335.4f, 0f);

        InitSurfaceLevel();

        // Conditions for surface level only set at start of game
        _instance = this;
        storageLeft = totalStorage;
        money = 1000;
    }

    void Update()
    {
        if(isDiving && !isLevelLost)
        {
            UpdateAir();
            //TODO: UpdateDepth();
        }
        
        UpdateUI();
    }

    //----------------------------------------------------SCENE SWITCHING FUNCTIONALITY----------------------------------------------

    void InitOceanLevel()
    {
        isDiving = true;
        airLeft = totalAir;
        isLevelLost = false;
        player.transform.position = OceanSpawnPos;
        SoundManager.Instance.PlaySplashSFX();
        SoundManager.Instance.StopWalkingSFX();
        SoundManager.Instance.PlayBreathingSFX();

        UsePlayerUI();
    }

    void InitSurfaceLevel()
    {
        // Plays splash sfx only if going to surface from
        // previously diving
        if(isDiving)
        {
            SoundManager.Instance.PlaySplashSFX();
        }
        
        isDiving = false;
        airLeft = totalAir;
        isLevelLost = false;
        player.transform.position = SurfaceSpawnPos;
        player.transform.rotation = Quaternion.Euler(SurfaceSpawnRot);
        SoundManager.Instance.StopBreathingSFX();
        SoundManager.Instance.PlayWalkingSFX();

        UseSurfaceUI();
    }

    void LevelOver()
    {
        // Go back for air and switch scenes
        airLeft = totalAir;
        totalFishValue = 0;
        storageLeft = totalStorage;
        Invoke("SwitchScene", 2);
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
            LoadOcean();
            InitOceanLevel();
            Invoke("FindFish", 1f);
        }
    }

    private void LoadSurface()
    {
        SceneManager.LoadScene("Surface");
    }

    private void LoadOcean()
    {
        SceneManager.LoadScene("ShallowOcean");
    }

    //-------------------------------------------------------------UI FUNCTIONALITY---------------------------------------------------------

    void UpdateUI()
    {
        depthUI.text = "" + currentDepth;
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