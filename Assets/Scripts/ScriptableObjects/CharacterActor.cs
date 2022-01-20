using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterActor : MonoBehaviour
{
    public character classe;
    private int attack;
    private int maxhealth;
    private int currenthealth;
    private int speed;
    // Start is called before the first frame update
    void Start()
    {
        attack = classe.attack;
        maxhealth = classe.maxhealth;
        currenthealth = classe.currenthealth;
        speed = classe.speed;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
    void attackaction(CharacterActor hero, CharacterActor target)
    {
        target.currenthealth = target.currenthealth - hero.attack;
    }
    /*void moveaction(CharacterActor hero)
    {
        hero.speed
    }*/
    void death(CharacterActor self)
    {
        if (self.currenthealth >= 0){
            Debug.Log("hero dies");
        }
    }
    void healaction(CharacterActor hero)//heal for paladin
    {
        hero.currenthealth += 20;
        hero.maxhealth += 20;
    }
    void chargeaction(CharacterActor hero)//charge for cavalier
    {
        hero.attack += 10;
    }
    void bigdamageaction(CharacterActor hero, CharacterActor damager)//bid damage for swordman
    {
        if (hero.currenthealth < hero.maxhealth)
        {
            hero.attack = hero.attack + (hero.maxhealth - hero.currenthealth);
        }
    }

}
