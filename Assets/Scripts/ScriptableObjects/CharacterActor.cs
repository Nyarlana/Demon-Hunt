using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : MonoBehaviour
{
    public character classe;
    private int attack;
    private int health;
    // Start is called before the first frame update
    void Start()
    {
        attack = classe.attack;
        health = classe.health;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
    void attackaction(CharacterActor hero, CharacterActor target)
    {
        target.health = target.health - hero.attack;
    }
}
