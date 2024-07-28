using UnityEngine;


public enum BiomType
{
    Sand,
    Grass1,
    Grass2,
    Grass3,
    Rock
}

public class Decoration : MonoBehaviour
{
    public GameObject _prefab;
    public BiomType _type;
    public float _spawnChance;
}
