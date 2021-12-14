using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[System.Serializable]
public struct TileData
{
    public TileBase tile;
    public bool isWalkable;
}

public class GridCollisionHandler : MonoBehaviour
{
    [SerializeField]
    private List<Tilemap> walkable_maps;
    [SerializeField]
    private List<Tilemap> mask_maps;
    [SerializeField]
    private List<TileData> exceptions; //last will override
    private Dictionary<TileBase, bool> exceptions_true;

    public TileBase test;

    private void Awake()
    {
        exceptions_true = new Dictionary<TileBase, bool>();
        foreach (TileData item in exceptions)
        {
            exceptions_true[item.tile] = item.isWalkable;
        }
        exceptions.Clear();
    }

    public bool isTileWalkableLocal(Vector2Int position)
    {
        Vector3Int pos = new Vector3Int(position.x, position.y, 0);
        Debug.Log(pos);
        bool res = false;
        //maps
        foreach (Tilemap item in walkable_maps)
        {
            TileBase local_tile = item.GetTile(pos);
            if (local_tile != null)
            {
                res = true;
                if (exceptions_true.ContainsKey(local_tile))
                {
                    res = exceptions_true.ContainsKey(local_tile);
                }
                break;
            }
        }
        //Masks
        if (res)
            foreach (Tilemap item in mask_maps)
            {
                TileBase local_tile = item.GetTile(pos);

                if (local_tile != null)
                {
                    res = false;
                    if (exceptions_true.ContainsKey(local_tile))
                    {
                        res = exceptions_true.ContainsKey(local_tile);
                    }
                    break;
                }
            }
        //return
        return res;
    }

    public bool isTileWalkableGlobal(Vector2Int position)
    {
        Vector3Int temp = new Vector3Int(position.x, position.y, 0);
        temp = gameObject.GetComponent<Grid>().WorldToCell(temp);
        return isTileWalkableLocal(new Vector2Int(temp.x, temp.y));
    }



    //DEBUG

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isTileWalkableGlobal(new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y)))
                Debug.Log("Tile walkable");
            else
                Debug.Log("Tile not walkable");
        }
    }
}