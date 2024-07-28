using UnityEngine;


public class DecorationSpawnerTriple : MonoBehaviour
{

    public GameObject[] _places;     // the third pos should the one, that uses bigOffset on X

    float smallOffset = WorldGenerator._tileSize / 10;  // when decoration's place pos should be randomized slightly
    float bigOffset = WorldGenerator._tileSize / 5;     // when decoration's place pos should be randomized a lot



    public GameObject[] GetPlaces()
    {
        RandomizePlacesOffset();
        RandomizePlacesRotation();
        RandomizeSpawnerRotation();
        return _places;
    }

    void RandomizePlacesOffset()
    {
        for (int i = 0; i < _places.Length; i++)
        {
            float xOffset, zOffset;

            if (i < 2) {
                xOffset = Random.Range(-smallOffset, smallOffset);
            }
            else {
                xOffset = Random.Range(-bigOffset, bigOffset);
            }

            zOffset = Random.Range(-smallOffset, smallOffset);

            _places[i].transform.localPosition += new Vector3(xOffset, 0, zOffset);
        }
    }

    void RandomizePlacesRotation()
    {
        foreach(var place in _places) {
            float randomRotation = Random.Range(0, 360);
            Quaternion rotation = Quaternion.Euler(0, randomRotation, 0);
            place.transform.rotation = rotation;
        }
    }

    void RandomizeSpawnerRotation()
    {
        int[] rotations = new int[] {0, 90, 180, 270};
        int randomIndex = Random.Range(0, rotations.Length);

        transform.rotation = Quaternion.Euler(0, rotations[randomIndex], 0);
    }
}
