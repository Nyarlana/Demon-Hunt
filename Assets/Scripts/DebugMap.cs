using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DebugMap : MonoBehaviour
{
    public Tilemap debug;
    public Pathfinder pathfinder;
    public Grid mapgrid;

    // Start is called before the first frame update
    void Start()
    {
        target = new Vector2Int(0, 0);
        pathfinder = GameObject.FindGameObjectsWithTag("Pathfinder")[0].GetComponent<Pathfinder>();
        if (pathfinder == null)
            Destroy(gameObject);
    }


    private Vector2Int target;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            globalMousePosition = mapgrid.WorldToCell(globalMousePosition);
            target = new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y);
        }
        if (Input.GetMouseButtonDown(0))
        {
            debug.ClearAllTiles();
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            globalMousePosition = mapgrid.WorldToCell(globalMousePosition);
            //Dictionary<Vector2Int, Vector2Int> path = pathfinder.BFS(new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y), 3);
            pathfinder.findPath(new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y), target);
            //List<Vector2Int> var = pathfinder.restorePath(path,target, new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y));
        }
    }
}
