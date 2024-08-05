using UnityEngine;


public class DecorationSpawnerDouble : DecorationSpawner
{
    [SerializeField] GameObject[] places;     // the third pos should the one, that uses bigOffset on X
    public override GameObject[] Places { 
        get {return places;}
    }

    float smallOffset = WorldGenerator.TileSize / 10;  // when decoration's place pos should be randomized slightly
    float bigOffset = WorldGenerator.TileSize / 5;     // when decoration's place pos should be randomized a lot


    protected override void RandomizePlacesOffset()
    {
        for (int i = 0; i < Places.Length; i++)
        {
            float xOffset, zOffset;

            xOffset = Random.Range(-bigOffset, bigOffset);
            zOffset = Random.Range(-smallOffset, smallOffset);

            Places[i].transform.localPosition += new Vector3(xOffset, 0, zOffset);
        }
    }

}
