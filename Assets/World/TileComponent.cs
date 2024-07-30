using UnityEngine;
using System.Collections.Generic;


public class TileComponent : MonoBehaviour
{
    public TileType TileType { get; private set; }
    public BiomeType BiomeType { get; private set; }
    public List <DecorationTypeSpawnChance> SpawnChances { get; private set; }

    public void Initialize(Tile tile)
    {
        TileType = tile.Type;
        BiomeType = tile.BiomeType;
        SpawnChances = tile.SpawnChances;
    }
}
