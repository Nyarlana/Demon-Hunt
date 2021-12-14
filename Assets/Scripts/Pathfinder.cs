using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Pathfinder : MonoBehaviour
{
    [SerializeField]
    private GameObject maps;
    [SerializeField]
    private Tilemap debug;
    [SerializeField]
    private TileBase queuedTile;
    [SerializeField]
    private TileBase exploredtile;

    private GridCollisionHandler mapgridhandler;
    private Grid mapgrid;
    private static Vector2Int[] directions = { Vector2Int.down, Vector2Int.up, Vector2Int.right, Vector2Int.left };

    // Start is called before the first frame update
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

    public void BFS(Vector2Int origin, int range)
    {
        //setup
        if (!mapgridhandler.isTileWalkableLocal(origin))
        {
            return;
        }
        List<Vector2Int> queue = new List<Vector2Int>();
        List<Vector2Int> explored = new List<Vector2Int>();
        queue.Add(origin);
        debug.SetTile(new Vector3Int(origin.x, origin.y, 0), queuedTile);

        //loop
        while (queue.Count > 0)
        {
            
            range--;
            List<Vector2Int> queue_temp = new List<Vector2Int>(queue);
            foreach (Vector2Int point in queue_temp)
            {
                explored.Add(point);
                debug.SetTile(new Vector3Int(point.x, point.y, 0), exploredtile);
                queue.Remove(point);

                if (range > 0)
                foreach (Vector2Int direction in directions)
                {
                    if (mapgridhandler.isTileWalkableLocal(direction + point) && !explored.Contains(direction + point))
                    {
                        debug.SetTile(new Vector3Int(point.x+direction.x, point.y+direction.y, 0), queuedTile);
                        queue.Add(direction + point);
                    }
                }
            }
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            debug.ClearAllTiles();
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            globalMousePosition = mapgrid.WorldToCell(globalMousePosition);
            BFS(new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y), 3);
        }
    }
}
