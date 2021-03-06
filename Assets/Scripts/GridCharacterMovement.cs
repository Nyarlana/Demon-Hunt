using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(BoxCollider2D))]
public class GridCharacterMovement : MonoBehaviour
{
    public Pathfinder pathfinder;
    public TurnManager turnmanager;
    //public bool selected;
    private Grid grid;
    private SpriteRenderer spr;

    public int range = 3;

    public float maxTimer = 0.2f; // Time between moves
    private float timer;
    [SerializeField] //DEBUG
    private List<Vector2Int> locQueue; // Queue of positions to follow
    private Vector3 previous_position;

    public void setLocQueue(List<Vector2Int> _locQueue)
    {
        if (locQueue.Count <= 0) locQueue = new List<Vector2Int>(_locQueue);
    }

    // Start is called before the first frame update
    // Used for empty variable setup
    void Start()
    {
      if (spr == null) {
        spr = gameObject.GetComponent<SpriteRenderer>();
      }
      if (pathfinder == null) {
        pathfinder = (Pathfinder) GameObject.FindWithTag("Pathfinder").GetComponent("Pathfinder");
      }
      if (turnmanager == null) {
        turnmanager = (TurnManager) GameObject.FindWithTag("Pathfinder").GetComponent("TurnManager");
      }
      grid = pathfinder.getGrid();

      locQueue = new List<Vector2Int>();
        transform.position = grid.WorldToCell(gameObject.transform.position) + grid.cellSize/2;
      previous_position = transform.position - grid.cellSize/2;
      timer = maxTimer;
    }

    // Update is called once per frame
    void Update()
    {
      // if there is a location to move to
      if (locQueue.Count > 0) {
          if (timer > 0.0) { // GO THERE IF TWEEN NOT FINISHED => lerp position and update tween timer
            float tx = Mathf.Lerp( previous_position.x, getWorldPosition(locQueue[0]).x, 1.0f-(timer/maxTimer));
            float ty = Mathf.Lerp( previous_position.y, getWorldPosition(locQueue[0]).y, 1.0f-(timer/maxTimer));
            gameObject.transform.position = new Vector3(tx, ty, gameObject.transform.position.z) + grid.cellSize/2;
            timer -= Time.deltaTime;
          }
          else { // GO THERE IF TWEEN FINISHED => set position cleanly, setup for next move
            gameObject.transform.position = getWorldPosition(locQueue[0]) + grid.cellSize/2;
            previous_position = gameObject.transform.position - grid.cellSize/2;
            locQueue.Remove(locQueue[0]);
            if (locQueue.Count > 0)
               { // GO THERE IF MOVES LEFT => reset timer
                    timer = maxTimer;
                    if (locQueue[0].x < getGridPosition().x) spr.flipX = true;
                    if (locQueue[0].x > getGridPosition().x) spr.flipX = false;
               }
                else
                {
                    OnActionDone();
                    turnmanager.Advance();
                }
          }
      }
    }

    /*
    returns grid position of gameObject
    */
    public Vector2Int getGridPosition() {
      Vector3 grid_position = grid.WorldToCell(gameObject.transform.position);
      return new Vector2Int((int)grid_position.x, (int)grid_position.y);
    }

    /*
    converts a grid position to world space. Shouldn't be defined there but kinda useful.
    */
    private Vector3 getWorldPosition(Vector2Int v2i) => grid.CellToWorld(new Vector3Int(v2i.x, v2i.y, 0));

    /*
    move to point target via grid movement. Queues movements.
    returns true if a path is found.
    */
    public bool moveToPoint(Vector2Int target) {
      Vector2Int local_position = getGridPosition();

      List<Vector2Int> path = pathfinder.findPath(local_position, target, range);
      if (path.Count <= 1) {
        return false;
      }
      //path.Reverse();
      locQueue = path;
      return true;
    }

    /*
     * returns mapdata of movement range
     */
    public Dictionary<Vector2Int, Vector2Int> areaOfEffect()
    {
        return pathfinder.BFS(getGridPosition(), range);
    }
    public Dictionary<Vector2Int, Vector2Int> areaOfEffect(ref List<GameObject> filter)
    {
        return pathfinder.BFS(getGridPosition(), range, filter);
    }

    /*
    on character clicked
    */
    void OnMouseDown() {
      turnmanager.select(gameObject);
      //moveToPoint(new Vector2Int(0,0));
    }

    public void OnActionDone()
    {
        spr.color = Color.gray;
    }

    public void OnActionReset()
    {
        spr.color = Color.white;
    }
}
