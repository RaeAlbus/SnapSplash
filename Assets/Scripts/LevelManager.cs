using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    // Singleton instance of the LevelManager
    private static LevelManager _instance;

    // Read-only instant of LevelManger to access from other scripts
    public static LevelManager Instance => _instance;

    //Whether the player is currently on a dive
    public static bool isDiving;

    // The total amount of air the player has for this level at the start
    public float totalAir = 20f;

    // Amount of air left at this second in level
    private float airLeft;

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
    public Slider airUI;
    public Text storageUI;
    public Text storageUICamera;
    public Text storageUISurface;
    public Text depthUI;
    public Text moneyUI;

    // Canvases: One for when camera is not equipped and one for when it is
    public Canvas playerCanvas;
    public Canvas cameraCanvas;
    public Canvas surfaceCanvas;

    public static GameObject player;
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

    void InitOceanLevel()
    {
        isDiving = true;
        airLeft = totalAir;
        isLevelLost = false;
        player.transform.position = OceanSpawnPos;
        SoundManager.Instance.PlaySplashSFX();
        SoundManager.Instance.PlayBreathingSFX();

        UsePlayerUI();
    }

    void InitSurfaceLevel()
    {
        isDiving = false;
        airLeft = totalAir;
        isLevelLost = false;
        //storageLeft = totalStorage; TODO: if passed out, then reset storage
        player.transform.position = SurfaceSpawnPos;
        player.transform.rotation = Quaternion.Euler(SurfaceSpawnRot);
        SoundManager.Instance.StopBreathingSFX();

        UseSurfaceUI();
    }

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

    void LevelOver()
    {
        // Go back for air and switch scenes
        airLeft = totalAir;
        totalFishValue = 0;
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

    public void addFishValue(float value)
    {
        totalFishValue += value;
        //Debug.Log("Total fish value: " + totalFishValue);
    }

    void FindFish()
    {
        fishInScene = GameObject.FindGameObjectsWithTag("Fish");
    }
}