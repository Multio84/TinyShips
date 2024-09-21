using System.Collections.Generic;
using UnityEngine;


// class to randomize places of spawn on a tile

public abstract class DecorationSpawner : MonoBehaviour
{
    public abstract GameObject[] Places { get; }
    bool isInitializationDone = false;

    public float smallOffset = WorldGenerator.TileSize / 12;  // when decoration's place pos should be randomized slightly
    public float bigOffset = WorldGenerator.TileSize / 6;     // when decoration's place pos should be randomized a lot

    // class to set spawn chance of certain spawner for certain decoration type
    [System.Serializable]
    public class SpawnerSpawnChance
    {
        public DecorationType DecorationType;
        public float SpawnChance;
    }

    public SpawnerSpawnChance[] spawnerSpawnChance;


    void OnEnable()
    {
        Initialize();
    }

    public Transform[] GetPlacesTransform()
    {
        while (!isInitializationDone) {}
        Transform[] placesTransform = new Transform[Places.Length];
        for (int i = 0; i < Places.Length; i++) {
            placesTransform[i] = Places[i].transform;
        }

        return placesTransform;
    }

    public void Initialize()
    {
        RandomizePlacesOffset();
        RandomizePlacesRotation();
        RandomizeSpawnerRotation();
        isInitializationDone = true;
    }

    protected abstract void RandomizePlacesOffset();

    protected void RandomizePlacesRotation()
    {
        foreach(var place in Places) {
            float randomRotation = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, randomRotation, 0);
            place.transform.rotation = rotation;
        }
    }

    protected void RandomizeSpawnerRotation()
    {
        int[] rotations = new int[] {0, 90, 180, 270};
        int randomIndex = Random.Range(0, rotations.Length);

        transform.rotation = Quaternion.Euler(0, rotations[randomIndex], 0);
    }

}