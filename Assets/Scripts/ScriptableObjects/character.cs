using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AnimatorController = UnityEditor.Animations.AnimatorController;

[CreateAssetMenu(fileName = "New Character", menuName = "Character ")]//apperance in create
public class character : ScriptableObject
{
    public new string name;

    public int attack;
    public int maxhealth;
    public int currenthealth;
    public int speed;
    public int range;

    public bool canHeal;
    public bool canTank;
    public bool canCharge;
    public bool canBigDamage;
    public bool canLongRange;
    public bool canAvoidDamage;
    public bool isHero;

    public Sprite artwork;

    public AnimatorController anim;


    public void print() {
        Debug.Log(name + " does " + attack + " damage " + " in " + speed + " time and has " + maxhealth + " heath ");
      }



}
