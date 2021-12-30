using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TurnManager : MonoBehaviour
{
  public Pathfinder pathfinder;
  [SerializeField] //DEBUG
  private Grid mapgrid;

  [SerializeField] //DEBUG
  private List<GameObject> playerteam;
  [SerializeField] //DEBUG
  private List<GameObject> aiteam;

  [SerializeField] //DEBUG
  private GameObject current;

  public enum TurnState // made so player turn is when state%2=1
  {
    NONE = 0,
    PLAYER_WAIT, //1
    AI_WAIT,     //2
    PLAYER_ANIM, //3
    AI_ANIM      //4
  }
  public TurnState state = TurnState.NONE;


    void Start()
    {
      if (pathfinder == null) {
        pathfinder = (Pathfinder) GameObject.FindWithTag("Pathfinder").GetComponent("Pathfinder");
      }
      mapgrid = pathfinder.getGrid();

      playerteam = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerControlled"));
      aiteam = new List<GameObject>(GameObject.FindGameObjectsWithTag("AIControlled"));

      state = TurnState.PLAYER_WAIT;
    }

    // Update is called once per frame
    void Update()
    {
      if (state == TurnState.PLAYER_WAIT)
      {
        if (Input.GetMouseButtonDown(1) && current != null)
        {
            Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            globalMousePosition = mapgrid.WorldToCell(globalMousePosition);
            Vector2Int target = new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y);
            GridCharacterMovement player_entity = (GridCharacterMovement) current.GetComponent("GridCharacterMovement");
            if (player_entity.moveToPoint(target) && player_entity != null) {
              state = TurnState.PLAYER_ANIM;
            }
        }
      }
    }

    public void select(GameObject target) {
      if (state == TurnState.PLAYER_WAIT && playerteam.Contains(target)) {
        current = target;
      }
    }
}
