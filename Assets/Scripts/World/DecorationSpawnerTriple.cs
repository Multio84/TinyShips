using UnityEngine;


public class DecorationSpawnerTriple : DecorationSpawner
{
    [SerializeField] GameObject[] places;     // the third pos should the one, that uses bigOffset on X
    public override GameObject[] Places { 
        get {return places;}
    }


    protected override void RandomizePlacesOffset()
    {
        for (int i = 0; i < Places.Length; i++)
        {
            float xOffset, zOffset;

            if (i < 2) {
                xOffset = Random.Range(-smallOffset, smallOffset);
            }
            else {
                xOffset = Random.Range(-bigOffset, bigOffset);
            }

            zOffset = Random.Range(-smallOffset, smallOffset);

            Places[i].transform.localPosition += new Vector3(xOffset, 0, zOffset);
        }
    }



    //bool isInitializationDone = false;

    // void OnEnable()
    // {
    //     Initialize();
    // }

    // public Transform[] GetPlacesTransform()
    // {
    //     while (!isInitializationDone) {}
    //     Transform[] placesTransform = new Transform[Places.Length];
    //     for (int i = 0; i < Places.Length; i++) {
    //         placesTransform[i] = Places[i].transform;
    //     }

    //     return placesTransform;
    // }

    // void Initialize()
    // {
    //     RandomizePlacesOffset();
    //     RandomizePlacesRotation();
    //     RandomizeSpawnerRotation();
    //     isInitializationDone = true;
    // }



    // void RandomizePlacesRotation()
    // {
    //     foreach(var place in Places) {
    //         float randomRotation = Random.Range(0, 360);
    //         Quaternion rotation = Quaternion.Euler(0, randomRotation, 0);
    //         place.transform.rotation = rotation;
    //     }
    // }

    // void RandomizeSpawnerRotation()
    // {
    //     int[] rotations = new int[] {0, 90, 180, 270};
    //     int randomIndex = Random.Range(0, rotations.Length);

    //     transform.rotation = Quaternion.Euler(0, rotations[randomIndex], 0);
    // }
}
