using System;
using UnityEditor;
using UnityEngine;
using Random = System.Random;

enum TerrainType
{
    GRASS,
    ROCK
}

public class FieldGenerator : MonoBehaviour
{

    public int width = 256;
    public int height = 256;

    public int x = 1;
    public int z = 1;
    
    public int depth = 20;

    public float scale = 2.5f;

    public PhysicMaterial grassMaterial;
    public PhysicMaterial rockMaterial;

    public TerrainLayer grassLayer;
    public TerrainLayer rockLayer;

    public GameObject holeTemplate;
    
    private Vector2 _holePosition;
    private int _holeRadius = 2;

    private Terrain[,] _terrainTiles;
    private Random random = new Random(32);

    // Start is called before the first frame update
    void Start()
    {
        _holePosition = GenerateHolePosition();
        _terrainTiles = new Terrain[x, z];
        
        CreateTerrain(gameObject);
        CreateHoleWin(gameObject);
    }

    private GameObject CreateHoleWin(GameObject parent)
    {
        GameObject hole = Instantiate(
            holeTemplate, 
            new Vector3(_holePosition.x * x * width, -10, _holePosition.y * z * height), 
            Quaternion.identity
        );
        hole.transform.localScale = new Vector3(_holeRadius * 10, 0, _holeRadius * 10);
        hole.transform.parent = parent.transform;
        return hole;
    }

    private GameObject CreateTerrain(GameObject parent)
    {
        GameObject terrain = new GameObject("Terrain");
        for (int tileX = 0; tileX < x; tileX++)
        {
            for (int tileZ = 0; tileZ < z; tileZ++)
            {
                _terrainTiles[tileX, tileZ] = CreateTerrainTile(terrain, tileX, tileZ).GetComponent<Terrain>();
            }
        }

        // ConnectTerrain();
        terrain.transform.parent = parent.transform;
        return terrain;
    }

    private void ConnectTerrain()
    {
        for (int tileX = 0; tileX < x; tileX++)
        {
            for (int tileZ = 0; tileZ < z; tileZ++)
            {
                Terrain terrainTile = _terrainTiles[tileX, tileZ];
                terrainTile.SetNeighbors(
                    tileX == 0 ? null : _terrainTiles[tileX - 1, tileZ],
                    tileZ == z - 1 ? null : _terrainTiles[tileX, tileZ + 1],
                    tileX == x - 1 ? null : _terrainTiles[tileX + 1, tileZ],
                    tileZ == 0 ? null : _terrainTiles[tileX, tileZ - 1]
                );
            }
        }
    }

    private GameObject CreateTerrainTile(GameObject parent, int tileX, int tileZ)
    {
        string tileName = "tile_" + tileX + "-" + tileZ;
        TerrainData terrainData = GenerateTerrainData(tileX, tileZ);
        GameObject tile = Terrain.CreateTerrainGameObject(terrainData);
        SetupTerrainTile(tile, tileX, tileZ);

        tile.name = tileName;
        tile.transform.parent = parent.transform;
        tile.transform.position = new Vector3(tileX * width - x * width / 2, 0, tileZ * height - z * height / 2);
        return tile;
    }

    private void SetupTerrainTile(GameObject tile, int tileX, int tileY)
    {
        TerrainData terrainData = tile.GetComponent<Terrain>().terrainData;
        TerrainCollider collider = tile.GetComponent<TerrainCollider>();
        TerrainType terrainType = GenerateTerrainType(tileX, tileY);

        collider.material = GetPhysicsMaterial(terrainType);

        TerrainLayer[] terrainLayers = new TerrainLayer[1];
        terrainLayers[0] = CreateTerrainLayer(terrainType);
        if (terrainLayers[0] != null)
        {
            terrainData.terrainLayers = terrainLayers;
        }
    }

    private PhysicMaterial GetPhysicsMaterial(TerrainType terrainType)
    {
        switch (terrainType)
        {
            case TerrainType.GRASS:
                return grassMaterial;
            case TerrainType.ROCK:
                return rockMaterial;
        }

        return null;
    }
    
    private TerrainLayer CreateTerrainLayer(TerrainType terrainType)
    {
        switch (terrainType)
        {
            case TerrainType.GRASS:
                return grassLayer;
            case TerrainType.ROCK:
                return rockLayer;
        }

        return null;
    }
    
    private TerrainType GenerateTerrainType(int tileX, int tileY)
    {
        Array values = Enum.GetValues(typeof(TerrainType));
        return (TerrainType)values.GetValue(this.random.Next(values.Length));
    }
    
    private TerrainData GenerateTerrainData(int tileX, int tileZ)
    {
        TerrainData terrainData = new TerrainData 
        {
            name = GenerateTerrainTileName(tileX, tileZ),
            heightmapResolution = width + 1,
            size = new Vector3(width, depth, height)
        };
        terrainData.SetHoles(0, 0, GenerateHoles(tileX, tileZ));
        terrainData.SetHeights(0, 0, GenerateHeights(tileX, tileZ));
        return terrainData;
    }

    private bool[,] GenerateHoles(int tileX, int tileZ)
    {
        bool[,] holes = new bool[width, height];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                holes[i, j] = !CheckHole(tileX, tileZ, i, j);
                // holes[i, j] = !(i == width - 1 && j == 0);
            }
        }
        return holes;
    }

    
    private bool CheckHole(int tileX, int tileZ, int i, int j)
    {
        float xCoord = (float) (tileX * width + j) / (width * x) - 0.5f;
        float zCoord = (float) (tileZ * height + i) / (height * z) - 0.5f;
        return Vector2.Distance(new Vector2(xCoord, zCoord), _holePosition) * width * x < _holeRadius;
    }

    private Vector2 GenerateHolePosition()
    {
        return new Vector2(0.14f, 0.11f);
    }

    private float[,] GenerateHeights(int tileX, int tileZ)
    {
        float[,] heights = new float[width + 1, height + 1];
        for (int i = 0; i <= width; i++)
        {
            for (int j = 0; j <= height; j++)
            {
                heights[i, j] = CalculateHeight(tileX, tileZ, i, j);
            }
        }
        return heights;
    }

    private float CalculateHeight(int tileX, int tileZ, int i, int j)
    {
        float xCoord = (float) (tileX * width + j) / (width * x) * scale;
        float zCoord = (float) (tileZ * height + i) / (height * z) * scale;
        float target = Mathf.PerlinNoise(xCoord, zCoord) / 3;
        if (tileX == x - 1)
        {
            target += ((float) j / height) * ((float) j / height);
        }
        if (tileX == 0)
        {
            target += ((float) (height - j) / height) * ((float) (height - j) / height);
        }
        if (tileZ == z - 1)
        {
            target += ((float) i / width) * ((float) i / width);
        }
        if (tileZ == 0)
        {
            target += ((float) (width - i) / width) * ((float) (width - i) / width);
        }
        return target;
    }

    private string GenerateTerrainTileName(int tileX, int tileZ)
    {
        return "tile_" + tileX + "-" + tileZ;
    }
}