using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character ")]//apperance in create 
public class character : ScriptableObject
{
    public new string name;
    public string capacity1;
    public string capacity2;

    public int attack;
    public int health;
    public int speed;

    public bool canHeal;
    public bool canCharge;

    public Sprite artwork;

    
    public void print() {
        Debug.Log(name + " does " + attack + " damage with " + capacity1 + " and " + capacity2 + " in " + speed + " time and has " + health + " heath ");
      }
   
    

}

