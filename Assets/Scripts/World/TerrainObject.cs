using UnityEngine;


public enum BiomeType
{
    Water,
    Shallow,
    Sand,
    Grass1,
    Grass2,
    Grass3,
    Rock
}


// class for all terrain objects (which are NOT interactable)

public abstract class TerrainObject : ScriptableObject
{
    public abstract GameObject Prefab { get; set; }
}