using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character", menuName = "Character")]
public class character : ScriptableObject
{
    public new string name;

    public int attack;
    public int health;

    public Sprite artwork;
    
}
