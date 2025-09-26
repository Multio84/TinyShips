using UnityEngine;


// class for game start and initialization management

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    public GameMode GameMode {get; private set;}
    public WorldGenerator WorldGenerator {get; private set;}
    public TerrainDecorator TerrainDecorator {get; private set;}
    public CameraController CameraController {get; private set;}
    public PlayerController PlayerController {get; private set;}


    void Awake()
    {
        if (Instance is null) {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else {
            Destroy(gameObject);
            return;
        }

        InitializeComponents();
        WorldGenerator.GenerateWorld();
        CameraController.Initialize();  // camera goes after mapGenerator cause it uses field parameters

        GameMode.StartGame();
    }

    void InitializeComponents()
    {
        GameMode = FindAnyObjectByType<GameMode>();
        if (GameMode is null) {
            Debug.Log("GameMode not found!");
            return;
        }

        PlayerController = FindAnyObjectByType<PlayerController>();
        if (PlayerController is null) {
            Debug.Log("PlayerController not found!");
            return;
        }

        WorldGenerator = FindAnyObjectByType<WorldGenerator>();
        if (WorldGenerator is null) {
            Debug.Log("MapGenerator not found!");
            return;
        }
        WorldGenerator.Initialize();

        TerrainDecorator = FindAnyObjectByType<TerrainDecorator>();
        if (TerrainDecorator is null) {
            Debug.Log("TerrainDecorator not found!");
            return;
        }
        

        CameraController = FindAnyObjectByType<CameraController>();
        if (CameraController is null) {
            Debug.Log("CameraController not found!");
            return;
        }

        Debug.Log("Initialization done.");
    }



}
