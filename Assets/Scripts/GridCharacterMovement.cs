using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class GridCharacterMovement : MonoBehaviour
{
  public Pathfinder pathfinder;
  //public bool selected;
  private Grid grid;

  private float timer;
  public float maxTimer = 0.2f;
  private List<Vector2Int> locQueue;
  private Vector3 previous_position;

    // Start is called before the first frame update
    void Start()
    {
      if (pathfinder == null) {
        pathfinder = (Pathfinder) GameObject.FindWithTag("Pathfinder").GetComponent("Pathfinder");
      }
      grid = pathfinder.getGrid();

      locQueue = new List<Vector2Int>();
      previous_position = grid.WorldToCell(gameObject.transform.position);
      timer = maxTimer;
    }

    // Update is called once per frame
    void Update()
    {
      if (locQueue.Count > 0) {
          if (timer > 0.0) {
            float tx = Mathf.Lerp( previous_position.x, getWorldPosition(locQueue[0]).x, 1.0f-(timer/maxTimer));
            float ty = Mathf.Lerp( previous_position.y, getWorldPosition(locQueue[0]).y, 1.0f-(timer/maxTimer));
            gameObject.transform.position = new Vector3(tx, ty, gameObject.transform.position.z) + grid.cellSize/2;
            timer -= Time.deltaTime;
          }
          else {
            gameObject.transform.position = getWorldPosition(locQueue[0]) + grid.cellSize/2;
            previous_position = gameObject.transform.position - grid.cellSize/2;
            locQueue.Remove(locQueue[0]);
            if (locQueue.Count > 0) {
              timer = maxTimer;
            }
          }
      }
    }


    public Vector2Int getGridPosition() {
      Vector3 grid_position = grid.WorldToCell(gameObject.transform.position);
      return new Vector2Int((int)grid_position.x, (int)grid_position.y);
    }

    private Vector3 getWorldPosition(Vector2Int v2i) => grid.CellToWorld(new Vector3Int(v2i.x, v2i.y, 0));

    public void moveToPoint(Vector2Int target) {
      Vector2Int local_position = getGridPosition();
      Debug.Log(local_position);

      List<Vector2Int> path = pathfinder.findPath(local_position, target);
      if (path.Count <= 1) {
        Debug.Log("Path not found");
        return;
      }
      Debug.Log(path);
      //path.Reverse();
      locQueue = path;
      return;
    }

    void OnMouseDown() {
      //selected = true;
      moveToPoint(new Vector2Int(0,0));
    }
}
