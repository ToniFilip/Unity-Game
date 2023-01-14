using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif

[System.Serializable]
public class MapGenerator : MonoBehaviour
{
    public string seed;
    public Texture2D heightMap;

    public List<MapTile> iTiles;
    public List<MapTile> lTiles;
    public List<MapTile> tTiles;
    public List<MapTile> xTiles;
    public List<MapTile> emptyTiles;

    public MapTile HillTile;
    public MapTile SpawnTile;
    public MapTile PlayerBaseTile;
    public GameObject MapBorder;

    public int size = 10;
    public int spawns = 4;
    public int accesses = 4;

    private MapTile[,] map;
    private const float TileSpacing = 30;

    /// <summary>
    /// A list of the tiles where the enemies spawn from.
    /// </summary>
    public List<MapTile> spawnTiles;
    public List<MapTile> spawnTilesNorth;
    public List<MapTile> spawnTilesEast;
    public List<MapTile> spawnTilesSouth;
    public List<MapTile> spawnTilesWest;
    public Dictionary<CardinalPoint, List<MapTile>> spawnTilesGrouped;

    public MapTile playerBase;

    [HideInInspector] public int hillCount;
    public float mapDifficulty;

    public bool generate;
    public bool clear;
    public bool mapBorder;

    private void OnEnable()
    {
        LoadMapVariables();
    }

    public void ComputeMapDifficulty(float sizeMax, float spawnsMax)
    {
        mapDifficulty = 0.2f * Mathf.Min(accesses, spawns)
            + 0.4f * hillCount / ((float)size * size)
            - 0.4f * (size / sizeMax)
            + 0.3f * (spawns / spawnsMax);
    }

    public void LoadMapVariables()
    {
        // Spawn tile list needs to be defined again, because references are not kept from edit into play mode
        spawnTiles = new List<MapTile>();
        for (int k = 0; k < transform.childCount; k++)
        {
            MapTile tile = transform.GetChild(k).GetComponent<MapTile>();
            if (tile == null) continue;
            if (tile.type == MapTileType.SPAWN)
            {
                if (tile.transform.position.x == 0)
                {
                    tile.cardinalPoint = CardinalPoint.West;
                }
                else if (tile.transform.position.z == 0)
                {
                    tile.cardinalPoint = CardinalPoint.South;
                }
                else if (tile.transform.position.x == (size - 1) * TileSpacing + transform.position.x)
                {
                    tile.cardinalPoint = CardinalPoint.East;
                }
                else if (tile.transform.position.z == (size - 1) * TileSpacing + transform.position.z)
                {
                    tile.cardinalPoint = CardinalPoint.North;
                }

                spawnTiles.Add(tile);
            }
        }


        // Group spawns for easier use in game manager
        spawnTilesNorth = new List<MapTile>();
        spawnTilesEast = new List<MapTile>();
        spawnTilesSouth = new List<MapTile>();
        spawnTilesWest = new List<MapTile>();

        for (int k = 0; k < spawnTiles.Count; k++)
        {
            if (spawnTiles[k].cardinalPoint == CardinalPoint.North)
            {
                spawnTilesNorth.Add(spawnTiles[k]);
            }
            else if (spawnTiles[k].cardinalPoint == CardinalPoint.East)
            {
                spawnTilesEast.Add(spawnTiles[k]);
            }
            else if (spawnTiles[k].cardinalPoint == CardinalPoint.South)
            {
                spawnTilesSouth.Add(spawnTiles[k]);
            }
            else
            {
                spawnTilesWest.Add(spawnTiles[k]);
            }
        }

        // Add group to list of lists
        spawnTilesGrouped = new Dictionary<CardinalPoint, List<MapTile>>();
        if (spawnTilesNorth.Count > 0) spawnTilesGrouped.Add(CardinalPoint.North, spawnTilesNorth);
        if (spawnTilesEast.Count > 0) spawnTilesGrouped.Add(CardinalPoint.East, spawnTilesEast);
        if (spawnTilesSouth.Count > 0) spawnTilesGrouped.Add(CardinalPoint.South, spawnTilesSouth);
        if (spawnTilesWest.Count > 0) spawnTilesGrouped.Add(CardinalPoint.West, spawnTilesWest);
    }

    /// <summary>
    /// Creates invisible walls on the border of the map.
    /// </summary>
    public void CreateMapBorder()
    {
        int xPosBase = size / 2;
        int yPosBase = size / 2;
        Vector3 offset = new Vector3(xPosBase * TileSpacing, 0, yPosBase * TileSpacing);

        Transform northBorder = Instantiate(MapBorder, new Vector3(0, 0, (size / 2 + 0.5f) * TileSpacing) + offset, Quaternion.identity, transform).transform;
        northBorder.localScale = new Vector3(size * TileSpacing, northBorder.localScale.y, northBorder.localScale.z);

        Transform eastBorder = Instantiate(MapBorder, new Vector3((size / 2 + 0.5f) * TileSpacing, 0, 0) + offset, Quaternion.identity, transform).transform;
        eastBorder.localScale = new Vector3(eastBorder.localScale.x, eastBorder.localScale.y, size * TileSpacing);

        Transform southBorder = Instantiate(MapBorder, new Vector3(0, 0, (-size / 2 - 0.5f) * TileSpacing) + offset, Quaternion.identity, transform).transform;
        southBorder.localScale = new Vector3(size * TileSpacing, southBorder.localScale.y, southBorder.localScale.z);

        Transform westBorder = Instantiate(MapBorder, new Vector3((-size / 2 - 0.5f) * TileSpacing, 0, 0) + offset, Quaternion.identity, transform).transform;
        westBorder.localScale = new Vector3(westBorder.localScale.x, westBorder.localScale.y, size * TileSpacing);
    }

    private void Update()
    {
        if (generate)
        {
            generate = false;
            GenerateMap();
        }
        if (clear)
        {
            clear = false;
            ClearMap();
        }
        if (mapBorder)
        {
            mapBorder = false;
            CreateMapBorder();
        }
    }

    public void ClearMap()
    {
        for (int k = transform.childCount - 1; k >= 0; k--)
        {
            DestroyImmediate(transform.GetChild(k).gameObject);
        }
        map = new MapTile[size, size];
        hillCount = 0;
    }

    public bool GenerateMap()
    {
        Random.InitState(seed.GetHashCode());
        ClearMap();

        if (size <= spawns / 4f)
        {
            Debug.LogError("Map is not big enough for number of spawns.");
            return false;
        }

        if (accesses > 4)
        {
            Debug.LogError("No more than four accesses to the base are allowed.");
            return false;
        } else if (accesses < 1)
        {
            Debug.LogError("At least one access is required.");
            return false;
        }

        // Place the spawn tiles randomly but uniformly distributed along all four sides
        spawnTiles = new List<MapTile>();
        for (int k = 0; k < spawns; k++)
        {
            bool foundPosition = false;
            int xPos = 0;
            int yPos = 0;
            CardinalPoint cardinalPoint = CardinalPoint.North;

            while (!foundPosition)
            {
                if (k % 4 == 0) // North
                {
                    cardinalPoint = CardinalPoint.North;
                    xPos = Random.Range(1, size - 1);
                    yPos = size - 1;
                    if (map[xPos, yPos] == null) break;
                } else if (k % 4 == 1) // East
                {
                    cardinalPoint = CardinalPoint.East;
                    xPos = size - 1;
                    yPos = Random.Range(1, size - 1);
                    if (map[xPos, yPos] == null) break;
                } else if (k % 4 == 2) // South
                {
                    cardinalPoint = CardinalPoint.South;
                    xPos = Random.Range(1, size - 1);
                    yPos = 0;
                    if (map[xPos, yPos] == null) break;
                } else if (k % 4 == 3) // West
                {
                    cardinalPoint = CardinalPoint.West;
                    xPos = 0;
                    yPos = Random.Range(1, size - 1);
                    if (map[xPos, yPos] == null) break;
                }
            }

            // Create spawn tile and add to list
            MapTile newSpawn = PlaceTile(SpawnTile, xPos, yPos);
            Debug.Log(cardinalPoint);
            newSpawn.cardinalPoint = cardinalPoint;

            spawnTiles.Add(newSpawn);
        }

        // Place player base
        int xPosBase = size / 2;
        int yPosBase = size / 2;
        playerBase = PlaceTile(PlayerBaseTile, xPosBase, yPosBase);


        // Fill the rest of the map
        // Add randomized costs
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                if (map[x, y] == null)
                {
                    PlaceTile(RandomTile(emptyTiles), x, y);

                    // Randomize costs or use the height map
                    if (heightMap != null)
                    {
                        int a = Mathf.FloorToInt(x / ((float) size) * heightMap.width);
                        int b = Mathf.FloorToInt(y / ((float) size) * heightMap.height);
                        map[x, y].costs = heightMap.GetPixel(a, b).grayscale * 100f;

                        // Randomize grass costs
                        if (map[x,y].costs == 0)
                        {
                            map[x, y].costs = Random.Range(0, 1f);
                        }
                    } else
                    {
                        if (Random.Range(0, 1f) < 0.5f) map[x, y].costs = Random.Range(0f, 1f);
                        else map[x, y].costs = Random.Range(0f, 100f);
                    }

                    // If high costs, place hills
                    if (map[x, y].costs > 1f)
                    {
                        HillTile.transform.localScale = new Vector3(HillTile.transform.localScale.x, map[x,y].costs * 0.05f, HillTile.transform.localScale.z);
                        PlaceTile(HillTile, x, y);
                        hillCount++;
                    }
                } else
                {
                    // Bases and spawn tiles are not passable
                    map[x, y].costs = float.MaxValue;
                }
            }
        }

        // Generate target tiles
        List<MapTile> targetTiles = new List<MapTile>
        {
            map[playerBase.x + 1, playerBase.y],
            map[playerBase.x - 1, playerBase.y],
            map[playerBase.x, playerBase.y + 1],
            map[playerBase.x, playerBase.y - 1]
        };

        // Randomly remove not used accesses
        for (int k = 0; k < 4 - accesses; k++)
        {
            targetTiles.RemoveAt(Random.Range(0, targetTiles.Count));
        }

        // Assign path targets
        for (int k = 0; k < spawnTiles.Count; k++)
        {
            float minDistance = float.MaxValue;
            int targetIndex = 0;

            for (int l = 0; l < targetTiles.Count; l++)
            {
                float distance = Mathf.Pow(spawnTiles[k].x - targetTiles[l].x, 2) + Mathf.Pow(spawnTiles[k].y - targetTiles[l].y, 2);
                if (distance < minDistance)
                {
                    targetIndex = l;
                    minDistance = distance;
                }
            }

            spawnTiles[k].target = targetTiles[targetIndex];
        }

        // --- Generate paths --- //
        foreach (MapTile spawnTile in spawnTiles)
        {
            CreatePath(spawnTile, spawnTile.target);

            // Save Path
            List<MapTile> path = new List<MapTile>();
            MapTile currentCell = spawnTile.target;
            while (currentCell != null)
            {
                path.Insert(0, currentCell);
                if (currentCell.predecessor != null) currentCell.predecessor.successor = currentCell;
                currentCell = currentCell.predecessor;
            }

            // Replace path tiles //
            PlaceTile(SpawnTile, path[0].x, path[0].y);
            bool lastTileWasEmpty = true;
            for (int k = 1; k < path.Count; k++)
            {
                // Check if empty -> non-empty
                // Modify already placed tile and move on
                if (lastTileWasEmpty && path[k].type != MapTileType.EMPTY)
                {
                    if (path[k].type == MapTileType.T) PlaceTile(RandomTile(xTiles), path[k].x, path[k].y);
                    else if (path[k].type == MapTileType.I)
                    {
                        Debug.Log("I: " + path[k].predecessor.type + "->" + path[k].type);
                        if (path[k].N && path[k].predecessor.x < path[k].x)
                        {
                            PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(1);
                        } else if (path[k].N && path[k].predecessor.x > path[k].x)
                        {
                            PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(3);
                        } else if (path[k].W && path[k].predecessor.y < path[k].y)
                        {
                            PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(0);
                        } else if (path[k].W && path[k].predecessor.y > path[k].y)
                        {
                            PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(2);
                        }
                    } else if (path[k].type == MapTileType.L)
                    {
                        Debug.Log("L: " + path[k].predecessor.type + "->" + path[k].type);
                        // From where are we coming?
                        if (path[k].predecessor.x != path[k].x)
                        {
                            if (path[k].N) PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(2);
                            else PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(0);
                        } else if (path[k].predecessor.y != path[k].y)
                        {
                            if (path[k].E) PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(3);
                            else PlaceTile(RandomTile(tTiles), path[k].x, path[k].y).Rotation(1);
                        }
                    }

                    lastTileWasEmpty = false;
                    continue;
                }

                // Check if non-empty -> empty: modify predecessor tile (additionally)
                // Check if non-empty -> non-empty: do nothing, since we are on an existing path
                if (!lastTileWasEmpty && path[k].type == MapTileType.EMPTY)
                {
                    if (path[k].predecessor.type == MapTileType.T)
                    {
                        PlaceTile(RandomTile(xTiles), path[k].predecessor.x, path[k].predecessor.y);
                    } else if (path[k].predecessor.type == MapTileType.I)
                    {
                        if (path[k].predecessor.N)
                        {
                            if (path[k].x > path[k].predecessor.x) PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(3);
                            else PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(1);
                        } else
                        {
                            if (path[k].y > path[k].predecessor.y) PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(2);
                            else PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(0);
                        }
                    } else if (path[k].predecessor.type == MapTileType.L)
                    {
                        // From where are we coming?
                        if (path[k].predecessor.x != path[k].x)
                        {
                            if (path[k].predecessor.N) PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(2);
                            else PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(0);
                        }
                        else if (path[k].predecessor.y != path[k].y)
                        {
                            if (path[k].predecessor.E) PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(3);
                            else PlaceTile(RandomTile(tTiles), path[k].predecessor.x, path[k].predecessor.y).Rotation(1);
                        }
                    }

                    lastTileWasEmpty = true;
                } else if (!lastTileWasEmpty && path[k].type != MapTileType.EMPTY)
                {
                    lastTileWasEmpty = false;
                    continue;
                }

                // If not blocked
                if (k == path.Count - 1)
                {
                    MapTile acessTile = PlaceTileAlongPath(path[k], playerBase, path[k].predecessor);
                    Debug.Log("Access Tile: " + acessTile.type);
                    acessTile.successor = playerBase;
                    playerBase.predecessor = acessTile;
                }
                else
                {
                    PlaceTileAlongPath(path[k], path[k].successor, path[k].predecessor);
                }

            }
            ResetTiles();
        }

        return true;
    }

    private void ResetTiles()
    {
        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                map[x, y].wasVisited = false;
                map[x, y].f = 0;
                map[x, y].g = 0;
                map[x, y].successor = null;
                map[x, y].predecessor = null;
            }
        }
    }

    private MapTile PlaceTileAlongPath(MapTile node, MapTile successor, MapTile predecessor)
    {
        // Check what tile type is required
        if (predecessor.x == successor.x)
        {
            // Adjust rotation
            MapTile newTile = PlaceTile(RandomTile(iTiles), node.x, node.y);
            newTile.Rotation(0);
            return newTile;
        }
        else if (predecessor.y == successor.y)
        {
            // Adjust rotation
            MapTile newTile = PlaceTile(RandomTile(iTiles), node.x, node.y);
            newTile.Rotation(1);
            return newTile;
        }
        else
        {
            // Adjust rotation
            MapTile newTile = PlaceTile(RandomTile(lTiles), node.x, node.y);

            if (successor.x > node.x && predecessor.y > node.y || predecessor.x > node.x && successor.y > node.y)
            {
                newTile.Rotation(0);
            } else if (successor.x < node.x && predecessor.y > node.y || predecessor.x < node.x && successor.y > node.y)
            {
                newTile.Rotation(3);
            } else if (successor.x > node.x && predecessor.y < node.y || predecessor.x > node.x && successor.y < node.y)
            {
                newTile.Rotation(1);
            } else
            {
                newTile.Rotation(2);
            }

            return newTile;
        }
    }

    private MapTile PlaceTile(MapTile tilePrefab, int x, int y)
    {
        Vector3 newTilePosition = new Vector3(x * TileSpacing, 0, y * TileSpacing) + transform.position;
        MapTile newTile = Instantiate(tilePrefab, newTilePosition, tilePrefab.transform.rotation, transform);

        // Check if tile was not empty
        if (map[x, y] != null)
        {
            // Copy some values and remove old tile
            newTile.costs = map[x, y].costs;

            newTile.predecessor = map[x, y].predecessor;
            newTile.successor = map[x, y].successor;

            if (map[x, y].successor != null) map[x, y].successor.predecessor = newTile;
            if (map[x, y].predecessor != null) map[x, y].predecessor.successor = newTile;

            // Check if this was a target
            foreach (MapTile spawnTile in spawnTiles)
            {
                if (spawnTile.target == map[x, y])
                {
                    spawnTile.target = newTile;
                }
            }

            // Finally, destroy the old tile
            DestroyImmediate(map[x, y].gameObject);
        }

        map[x, y] = newTile;
        map[x, y].gameObject.SetActive(true);

        map[x, y].x = x;
        map[x, y].y = y;

        return map[x, y];
    }

    /// <summary>
    /// Creates a path using A*.
    /// </summary>
    /// <param name="start"></param>
    /// <param name="end"></param>
    /// <returns></returns>
    private bool CreatePath(MapTile start, MapTile end)
    {
        List<MapTile> openList = new List<MapTile> { start };

        while (true)
        {
            MapTile currentCell = MinF(openList);

            if (currentCell == end)
            {
                break;
            }

            currentCell.wasVisited = true;

            // Expand cell
            foreach (MapTile successorCell in GetAdjacentCells(currentCell.x, currentCell.y))
            {
                if (successorCell.wasVisited) continue;

                float tempG = currentCell.g + successorCell.costs;

                if (openList.Contains(successorCell) && tempG >= successorCell.g) continue;

                successorCell.predecessor = currentCell;
                successorCell.g = tempG;

                float tempF = tempG + successorCell.costs;

                successorCell.f = tempF;
                if (!openList.Contains(successorCell))
                {
                    openList.Add(successorCell);
                }
            }

            if (openList.Count == 0)
            {
                // No path was found
                return false;
            }
        }

        return true;
    }

    private List<MapTile> GetAdjacentCells(int x, int y)
    {
        List<MapTile> result = new List<MapTile>();

        if (x > 0)
        {
            result.Add(map[x - 1, y]);
        }
        if (x < size - 1)
        {
            result.Add(map[x + 1, y]);
        }

        if (y > 0)
        {
            result.Add(map[x, y - 1]);
        }
        if (y < size - 1)
        {
            result.Add(map[x, y + 1]);
        }

        return result;
    }

    private MapTile MinF(List<MapTile> tiles)
    {
        MapTile result = null;
        float min = float.MaxValue;

        foreach (MapTile tile in tiles)
        {
            if (tile.wasVisited) continue;
            if (tile.f < min)
            {
                min = tile.f;
                result = tile;
            }
        }

        return result;
    }

    /// <summary>
    /// Returns a random tile out of a list of Tiles.
    /// </summary>
    /// <returns></returns>
    private MapTile RandomTile(List<MapTile> tiles)
    {
        return tiles[Random.Range(0, tiles.Count)];
    }
}