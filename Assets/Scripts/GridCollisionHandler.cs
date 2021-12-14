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

    private enum Exceptions
    {
        WALKABLE = 0,
        NOT_WALKABLE,
        NONE
    }

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
        Exceptions exeptres = Exceptions.NONE;
        //maps
        foreach (Tilemap item in walkable_maps)
        {
            TileBase local_tile = item.GetTile(pos);
            if (local_tile != null)
            {
                res = true;
                if (exceptions_true.ContainsKey(local_tile))
                {
                    if (exceptions_true[local_tile])
                        exeptres = Exceptions.WALKABLE;
                    else
                        exeptres = Exceptions.NOT_WALKABLE;
                }
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
                        if (exceptions_true[local_tile])
                            exeptres = Exceptions.WALKABLE;
                        else
                            exeptres = Exceptions.NOT_WALKABLE;
                    }
                }
            }

        //apply exceptions
        switch (exeptres)
        {
            case Exceptions.WALKABLE:
                return true;
            case Exceptions.NOT_WALKABLE:
                return false;
            case Exceptions.NONE:
                break;
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

    public bool isTileWalkableGlobal(Vector3 position) => isTileWalkableGlobal(new Vector2Int((int)position.x, (int)position.y));



    //DEBUG

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (isTileWalkableGlobal(globalMousePosition))
                Debug.Log("Tile walkable");
            else
                Debug.Log("Tile not walkable");
        }
    }
}