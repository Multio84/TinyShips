using UnityEngine;
using System.Collections.Generic;


public class TileComponent : MonoBehaviour
{
    public GameObject TileObject;
    public TileType TileType { get; private set; }
    public BiomeType BiomeType { get; private set; }
    public List<DecorationTypeSpawnChance> SpawnChances { get; private set; }

    // tile X & Z size
    public float Size { get => WorldGenerator.TileSize; }

    // tile Y size
    public float Height {
        get { 
            return TileObject.transform.localScale.y * WorldGenerator.TileHeightModifier;
        }
    }


    public void Initialize(Tile tile)
    {
        TileType = tile.Type;
        BiomeType = tile.BiomeType;
        SpawnChances = tile.SpawnChances;
    }
}
