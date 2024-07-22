using UnityEngine;


// class for game start and initialization management

public class GameManager : MonoBehaviour
{
    public static GameManager Instance {get; private set;}

    public GameMode GameMode {get; private set;}
    public MapGenerator MapGenerator {get; private set;}
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
        MapGenerator.GenerateMap();

        GameMode.StartGame();
    }

    void InitializeComponents()
    {
        GameMode = FindObjectOfType<GameMode>();
        if (GameMode is null) {
            Debug.Log("GameMode not found!");
            return;
        }

        PlayerController = FindObjectOfType<PlayerController>();
        if (PlayerController is null) {
            Debug.Log("PlayerController not found!");
            return;
        }

        MapGenerator = FindObjectOfType<MapGenerator>();
        if (MapGenerator is null) {
            Debug.Log("MapGenerator not found!");
            return;
        }
        MapGenerator.Initialize();

        CameraController = FindObjectOfType<CameraController>();
        if (CameraController is null) {
            Debug.Log("CameraController not found!");
            return;
        }
        CameraController.Initialize();


        Debug.Log("Initialization done.");
    }



}
