using UnityEngine;


public class DecprationSpawnerQuadro : DecorationSpawner
{
    [SerializeField] GameObject[] places;     // the third pos should the one, that uses bigOffset on X
    public override GameObject[] Places
    {
        get => places;//{ return places; }
    }

    float smallOffset = WorldGenerator.TileSize / 10;  // when decoration's place pos should be randomized slightly


    protected override void RandomizePlacesOffset()
    {
        for (int i = 0; i < Places.Length; i++)
        {
            float xOffset, zOffset;

            xOffset = Random.Range(-smallOffset, smallOffset);
            zOffset = Random.Range(-smallOffset, smallOffset);

            Places[i].transform.localPosition += new Vector3(xOffset, 0, zOffset);
        }
    }
}
