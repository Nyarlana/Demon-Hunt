using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : MonoBehaviour
{
    public character classe;
    private int attack;
    private int health;
    private int speed;
    // Start is called before the first frame update
    void Start()
    {
        attack = classe.attack;
        health = classe.health;
        speed = classe.speed;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
    void attackaction(CharacterActor hero, CharacterActor target)
    {
        target.health = target.health - hero.attack;
    }
    void moveaction(CharacterActor hero)
    {
        hero.speed
    }
    void death(CharacterActor self)
    {
        if self.health >= 0{
            Debug.Log("hero dies");
        }
    }
    void healaction(CharacterActor hero)//heal for paladin
    {
        hero.health += 20;
    }
    void chargeaction(CharacterActor hero)//charge for cavalier
    {
        hero.attack += 10;
    }
    void bigdamageaction(CharacterActor hero, CharacterActor damager)//bid damage for swordman
    {
        if hero.health
        {
            damager.health = damager.health - hero.attack;
        }
    }
}
