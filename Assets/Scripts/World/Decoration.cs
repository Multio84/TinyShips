using System.Collections.Generic;
using UnityEngine;


public enum DecorationType
{
    Tree,   // trees & bushes
    Grass,  // grass, flowers, and other small flora
    Stone   // stones and other small objects
}

// class to set spawn chance of decoration on a biome
[System.Serializable]
public class BiomeSpawnChance
{
    public BiomeType BiomeType;
    public float SpawnChance;
}

[CreateAssetMenu(fileName = "Decoration", menuName = "Terrain/Decoration")]
public class Decoration : TerrainObject
{
    [SerializeField] private DecorationType _type;
    [SerializeField] private GameObject _prefab;
    [SerializeField] private List <BiomeSpawnChance> _biomeSpawnChances = new List <BiomeSpawnChance>();

    public DecorationType Type => _type;
    public override GameObject Prefab => _prefab;
    public List <BiomeSpawnChance> BiomeSpawnChances => _biomeSpawnChances;   // spawn chances of this decoration for each biome

    private void OnEnable()
    {
        InitAllBiomesChance();
    }

    // inits spawn chance on each biom as 0, if it's not set yet
    private void InitAllBiomesChance()
    {
        foreach (BiomeType biomeType in System.Enum.GetValues(typeof(BiomeType)))
        {
            bool chanceIsSet = false;
            foreach (var spawnChance in _biomeSpawnChances) {
                if (spawnChance.BiomeType == biomeType) {
                    chanceIsSet = true;
                    break;
                }
            }

            if (!chanceIsSet) {
                _biomeSpawnChances.Add(new BiomeSpawnChance { BiomeType = biomeType, SpawnChance = 0f });
            }
        }
    }

}
