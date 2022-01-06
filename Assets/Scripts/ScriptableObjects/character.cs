using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]//apperance in create
public class character : ScriptableObject
{
    public new string name;

    public int attack;
    public int health;

    public Sprite artwork;

    public void capacity()//function for capacity
    {

    }
    public void print() {
        Debug.Log(name + " does " + attack + " damage and has " + health + " live ");
      }
    
}
