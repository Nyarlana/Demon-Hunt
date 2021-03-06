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
    [SerializeField] private Tilemap GUI;
    [SerializeField] private TileBase guitile;
    [SerializeField] private TileBase attackguitile;

    [SerializeField] //DEBUG
    private Dictionary<GameObject, bool> playerteam;
    [SerializeField] //DEBUG
    private Dictionary<GameObject, bool> aiteam;

    [SerializeField] //DEBUG
    private GameObject current;

    [SerializeField] //DEBUG
    private CharacterActor attack_target;
    private CharacterActor attack_origin;

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
        attack_target = null;
        attack_origin = null;
        if (pathfinder == null)
        {
            pathfinder = (Pathfinder) GameObject.FindWithTag("Pathfinder").GetComponent("Pathfinder");
        }
        mapgrid = pathfinder.getGrid();
        if (GUI == null) GUI = mapgrid.gameObject.GetComponent<GridCollisionHandler>().GetGUI();

        playerteam = new Dictionary<GameObject, bool>();
        aiteam = new Dictionary<GameObject, bool>();
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("PlayerControlled"))
        {
            playerteam[item] = true;
        }
        foreach (GameObject item in GameObject.FindGameObjectsWithTag("AIControlled"))
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
             if (Input.GetMouseButtonDown(0) && current != null)
             {
                //get entity
                GridCharacterMovement player_entity = current.GetComponent<GridCharacterMovement>();

                //get mouse
                Vector3 globalMousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                globalMousePosition = mapgrid.WorldToCell(globalMousePosition);
                Vector2Int target = new Vector2Int((int)globalMousePosition.x, (int)globalMousePosition.y);

                // deal with entity collision
                foreach (GameObject item in playerteam.Keys)
                {
                    GridCharacterMovement gcm = item.GetComponent<GridCharacterMovement>();
                    if (target == gcm.getGridPosition()) return;
                }
                foreach (GameObject item in aiteam.Keys)
                {
                    GridCharacterMovement gcm = item.GetComponent<GridCharacterMovement>();
                    if (target == gcm.getGridPosition())
                    {
                        //target = pathfinder.BFS(current.GetComponent<GridCharacterMovement>().getGridPosition(), 99)[target];
                        Vector2Int res;
                        if (player_entity.areaOfEffect().TryGetValue(target, out res))
                        {
                            //target = player_entity.areaOfEffect()[target];
                            target = res;
                            attack_target = item.GetComponent<CharacterActor>();
                            attack_origin = current.GetComponent<CharacterActor>();
                        }
                    }
                }

                //execute movement
                if (player_entity.moveToPoint(target) && player_entity != null)
                {
                    Advance();
                    playerteam[current] = false;
                    current = null;
                    GUI.ClearAllTiles();
                }
                else if (attack_target != null)
                {
                    Advance();
                    playerteam[current] = false;
                    player_entity.OnActionDone();
                    current = null;
                    GUI.ClearAllTiles();
                    Advance();
                    //attack_origin.attackaction(attack_target);
                    //attack_origin = null; attack_target = null;
                }
            }
         }
    }

    public void select(GameObject target)
    {
        if (!playerteam.ContainsKey(target)) return;
        if (state == TurnState.PLAYER_WAIT && playerteam[target])
        {
            GUI.ClearAllTiles();
            current = target;
            List<GameObject> tenemylist = new List<GameObject>(aiteam.Keys);

            foreach (Vector2Int item in target.GetComponent<GridCharacterMovement>().areaOfEffect(ref tenemylist).Keys)
            {
                GUI.SetTile(new Vector3Int(item.x, item.y, 0), guitile);
                foreach (GameObject enemy in tenemylist)
                {
                    if (enemy.GetComponent<GridCharacterMovement>().getGridPosition() == item)
                    {
                        GUI.SetTile(new Vector3Int(item.x, item.y, 0), attackguitile);
                    }
                }
            }
            //playerteam[target] = false;
        }
    }

    private void OnAiWait()
    {
        List<GameObject> temp = new List<GameObject>(aiteam.Keys);
        foreach (GameObject enemy in temp)
        {
            if (aiteam[enemy])
            {
                current = enemy;
                List<GameObject> templayer = new List<GameObject>(playerteam.Keys);
                GridCharacterMovement gcm = enemy.GetComponent<GridCharacterMovement>();

                DoAiMeta(enemy);
                Dictionary<Vector2Int, Vector2Int> pathdata = gcm.areaOfEffect(ref templayer);
                mapgrid.gameObject.GetComponent<GridCollisionHandler>().GetMeta().ClearAllTiles();

                if (templayer.Count > 0)
                {
                    // Debug.Log("target : " + templayer[0] + " | origin : " + enemy);
                    Vector2Int arrival = templayer[0].GetComponent<GridCharacterMovement>().getGridPosition();
                    // Debug.Log("target : " + arrival + " | origin : " + gcm.getGridPosition());
                    List<Vector2Int> path = pathfinder.restorePath(pathdata, gcm.getGridPosition(), arrival); //pathfinder.findPath(gcm.getGridPosition(), arrival, gcm.range);
                    path.Remove(arrival);
                    gcm.setLocQueue(path);
                    aiteam[enemy] = false;
                    attack_target = templayer[0].GetComponent<CharacterActor>();
                    attack_origin = current.GetComponent<CharacterActor>();
                }
                else
                {
                    current = null;
                    aiteam[enemy] = false;
                    Advance();
                }
                Advance();
                return;
            }
        }
    }

    private void DoAiMeta(GameObject gm)
    {
        Tilemap tilemap = mapgrid.gameObject.GetComponent<GridCollisionHandler>().GetMeta();
        foreach (GameObject item in aiteam.Keys)
        {
            if (item != gm)
            {
                Vector2Int locPos = item.GetComponent<GridCharacterMovement>().getGridPosition();
                tilemap.SetTile(new Vector3Int(locPos.x, locPos.y, 0), guitile);
            }
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
                state = TurnState.PLAYER_ANIM;
                break;
            case TurnState.AI_WAIT:
                state = TurnState.AI_ANIM;
                break;
            case TurnState.PLAYER_ANIM:
                if (attack_target != null)
                {
                    attack_origin.attackaction(attack_target);
                    if (attack_target.Death())
                    {
                        aiteam.Remove(attack_target.gameObject);
                    }
                    attack_origin = null; attack_target = null;
                }
                if (!playerteam.ContainsValue(true))
                {
                    List<GameObject> temp = new List<GameObject>(aiteam.Keys);
                    foreach (GameObject item in temp)
                    {
                        aiteam[item] = true;
                    }
                    foreach (GameObject item in playerteam.Keys) item.GetComponent<GridCharacterMovement>().OnActionReset();
                    state = TurnState.AI_WAIT;
                    OnAiWait();
                    break;
                }
                state = TurnState.PLAYER_WAIT;
                break;
            case TurnState.AI_ANIM:
                if (attack_target != null)
                {
                    attack_origin.attackaction(attack_target);
                    if (attack_target.Death())
                    {
                        playerteam.Remove(attack_target.gameObject);
                    }
                    attack_origin = null; attack_target = null;
                }
                if (!aiteam.ContainsValue(true))
                {
                    List<GameObject> temp = new List<GameObject>(playerteam.Keys);
                    foreach (GameObject item in temp)
                    {
                        playerteam[item] = true;
                    }
                    foreach (GameObject item in aiteam.Keys) item.GetComponent<GridCharacterMovement>().OnActionReset();
                    state = TurnState.PLAYER_WAIT;
                    break;
                }
                state = TurnState.AI_WAIT;
                OnAiWait();
                break;
            default:
                break;
        }
    }
}
