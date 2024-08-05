using UnityEngine;


public class DecorationSpawnerSingle : DecorationSpawner
{
    [SerializeField] GameObject[] places;     // the third pos should the one, that uses bigOffset on X
    public override GameObject[] Places { 
        get {return places;}
    }

    float bigOffset = WorldGenerator.TileSize / 5;     // when decoration's place pos should be randomized a lot


    protected override void RandomizePlacesOffset()
    {
        float xOffset = Random.Range(-bigOffset, bigOffset);
        float zOffset = Random.Range(-bigOffset, bigOffset);

        Places[0].transform.localPosition += new Vector3(xOffset, 0, zOffset);
    }
}
