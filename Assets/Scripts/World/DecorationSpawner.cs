using UnityEngine;


// class to randomize places of spawn on a tile

public abstract class DecorationSpawner : MonoBehaviour
{
    public abstract GameObject[] Places { get; }
    bool isInitializationDone = false;


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