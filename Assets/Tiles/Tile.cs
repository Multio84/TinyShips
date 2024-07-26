using UnityEngine;


public enum TileType {
    Water,
    Land,
    Reef
}

public enum TileTerrainType {
    Water,
    Shallow,
    Sand,
    Grass1,
    Grass2,
    Grass3,
    Rock
}


// square object, spawned in the world to create terrain
public class Tile : MonoBehaviour
{
    // set these manually:
    public GameObject _tile;
    public TileType Type;
    public TileTerrainType TerrainType;
    
    // tile X & Z size
    public float Size {get => WorldGenerator._tileSize;}

    // tile Y size
    public float Height {
        get {
            if (_tile is null) {
                Debug.Log("There's no tile object assigned to Tile");
                return 0;
            }
            else {
                return _tile.transform.localScale.x / 10;
            }
        }
    }



    
}
