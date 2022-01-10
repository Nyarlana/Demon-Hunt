using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{

    [SerializeField]
    private GameObject maps;
    //debug
    [SerializeField]
    private Tilemap debug;
    [SerializeField]
    private TileBase queuedTile;
    [SerializeField]
    private TileBase exploredtile;
    [SerializeField]
    private TileBase pathtile;
    //end debug

    private GridCollisionHandler mapgridhandler;
    private Grid mapgrid;
    private static Vector2Int[] directions = { Vector2Int.down, Vector2Int.up, Vector2Int.right, Vector2Int.left };

    public Grid getGrid() => mapgrid;

    // When awakened, get variables components
    void Awake()
    {
        mapgridhandler = maps.GetComponent<GridCollisionHandler>();
        if (mapgridhandler == null)
        {
            maps.AddComponent<GridCollisionHandler>();
            mapgridhandler = maps.GetComponent<GridCollisionHandler>();
        } // will treat every tile as not walkable but prevents crashes

        mapgrid = maps.GetComponent<Grid>();
        if (mapgrid == null)
        {
            maps.AddComponent<Grid>();
            mapgrid = maps.GetComponent<Grid>();
        } //same
    }

    /*
     * BFS Implementation
     * Searching algorithm that scans a range in a grid in every direction equally. Used to make paths and scan terrain.
     * V2Int, int, List<GameObject> -> Dictionary<V2Int, V2int>
     * origin : starting point from which the search starts
     * range : how far should the search go
     * filter : list of gameobjects to filter from. This will edit the list to remove every GameObject not in search range from it.
     * return parameter : map data dictionary. Key is an unique cell, value is its predecessor (from). Use map.Keys to get every cell in range, use in restorePath to generate a path.
     */
    public Dictionary<Vector2Int, Vector2Int> BFS(Vector2Int origin, int range, List<GameObject> filter = null)
    {
        //setup
        if (!mapgridhandler.isTileWalkableLocal(origin))
        {
            return new Dictionary<Vector2Int, Vector2Int>();
        }
        List<Vector2Int> queue = new List<Vector2Int>();
        List<Vector2Int> explored = new List<Vector2Int>();
        Dictionary<Vector2Int, Vector2Int> pathdata = new Dictionary<Vector2Int, Vector2Int>();
        queue.Add(origin);
        pathdata.Add(origin, origin);
        debug.SetTile(new Vector3Int(origin.x, origin.y, 0), queuedTile); //DEBUG

        //loop
        while (queue.Count > 0)
        {

            range--;
            List<Vector2Int> queue_temp = new List<Vector2Int>(queue);
            foreach (Vector2Int point in queue_temp)
            {
                explored.Add(point);
                debug.SetTile(new Vector3Int(point.x, point.y, 0), exploredtile); //DEBUG
                queue.Remove(point);

                if (range > 0)
                foreach (Vector2Int direction in directions)
                {
                    if (mapgridhandler.isTileWalkableLocal(direction + point) && !explored.Contains(direction + point) && !queue.Contains(direction + point))
                    {
                        debug.SetTile(new Vector3Int(point.x+direction.x, point.y+direction.y, 0), queuedTile); //DEBUG
                        queue.Add(direction + point);
                        pathdata.Add(point+direction, point);
                    }
                }
            }
        }

        //filter out GameObjects
        if (filter != null)
        {
            List<GameObject> gobjarr = new List<GameObject>(filter);
            foreach (GameObject gObj in filter)
            {
                Vector3 localpos = mapgrid.WorldToCell(gObj.transform.position);
                if (!explored.Contains(new Vector2Int((int)localpos.x, (int)localpos.y)))
                {
                    gobjarr.Remove(gObj);
                }
            }
            filter = gobjarr;
        }

        return pathdata;
    }

    /*
     * pathfinding simple access
     * Creates a path from origin to arrival, or nothing if the path does not exist.
     * V2int, V2Int, int -> List<V2Int>
     * origin : starting point to find a path from
     * arrival : ending point of the path
     * depth : how far to search before giving up, defaults should be larger than map size.
     * return value : series of positions in map space that forms a path.
     */
    public List<Vector2Int> findPath(Vector2Int origin, Vector2Int arrival, int depth = int.MaxValue) => restorePath(BFS(origin, depth), origin, arrival);

    /*
     * Generates a path from path data
     * Dictionary<V2Int, V2Int>, V2Int, V2Int -> List<V2Int>
     * map : pathdata (see BFS)
     * origin : map coordinates of the starting point of the path
     * target : map coordinates of the ending point of the path
     * return value : path, in map space
     */
    public List<Vector2Int> restorePath(Dictionary<Vector2Int, Vector2Int> map, Vector2Int origin, Vector2Int target)
    {
        List<Vector2Int> res = new List<Vector2Int>();
        if (!map.ContainsKey(origin) || !map.ContainsKey(target))
        {
            return res;
        }
        Vector2Int current = target;
        while (current != origin)
        {
            res.Add(current);
            debug.SetTile(new Vector3Int(current.x, current.y, 0), pathtile);
            current = map[current];
        }
        res.Add(origin);
        debug.SetTile(new Vector3Int(current.x, current.y, 0), pathtile);
        res.Reverse();
        return res;
    }

}
