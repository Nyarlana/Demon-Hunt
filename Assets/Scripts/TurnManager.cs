using System;
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
  private Dictionary<GameObject, bool> playerteam;
  [SerializeField] //DEBUG
  private Dictionary<GameObject, bool> aiteam;

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
        if (pathfinder == null) 
        {
            pathfinder = (Pathfinder) GameObject.FindWithTag("Pathfinder").GetComponent("Pathfinder");
        }
        mapgrid = pathfinder.getGrid();

        List<GameObject> pt = new List<GameObject>(GameObject.FindGameObjectsWithTag("PlayerControlled"));
        List<GameObject> ait = new List<GameObject>(GameObject.FindGameObjectsWithTag("AIControlled"));
        playerteam = new Dictionary<GameObject, bool>();
        aiteam = new Dictionary<GameObject, bool>();
        foreach (GameObject item in pt)
        {
            playerteam[item] = true;
        }
        foreach (GameObject item in ait)
        {
            aiteam[item] = true;
        }


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
                if (player_entity.moveToPoint(target) && player_entity != null) 
                {
                    state = TurnState.PLAYER_ANIM;
                    current = null;
                }
             }
         }
    }

    public void select(GameObject target) 
    {
        if (!playerteam.ContainsKey(target)) return;
        if (state == TurnState.PLAYER_WAIT && playerteam[target]) 
        {
            current = target;
            playerteam[target] = false;
        }
    }

    internal void Advance()
    {
        switch (state)
        {
            case TurnState.NONE:
                state = TurnState.PLAYER_WAIT;
                break;
            case TurnState.PLAYER_WAIT:
                state = TurnState.PLAYER_WAIT;
                break;
            case TurnState.AI_WAIT:
                state = TurnState.AI_WAIT;
                break;
            case TurnState.PLAYER_ANIM:
                if (!playerteam.ContainsValue(true))
                {
                    state = TurnState.AI_WAIT;
                    break;
                }
                state = TurnState.PLAYER_WAIT;
                break;
            case TurnState.AI_ANIM:
                state = TurnState.AI_ANIM;
                break;
            default:
                break;
        }
    }
}
