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
    private int range;
    // Start is called before the first frame update
    void Start()
    {
        attack = classe.attack;
        maxhealth = classe.maxhealth;
        currenthealth = classe.currenthealth;
        speed = classe.speed;
        range = classe.range;
    }

    // Update is called once per frame
    void Update()
    {
 
    }
    void attackaction(CharacterActor hero, CharacterActor target)
    {
        target.currenthealth = target.currenthealth - hero.attack;
    }
    
    
    void death(CharacterActor self)
    {
        if (self.currenthealth >= 0){
            Debug.Log("hero dies");
        }
    }
    void tankaction(CharacterActor hero)//tank for paladin
    {
        hero.currenthealth += 20;
        hero.maxhealth += 20;
    }
    void chargeaction(CharacterActor hero)//charge for cavalier
    {
        hero.attack += 10;
        hero.speed += 2;
    }
    void bigdamageaction(CharacterActor hero, CharacterActor damager)//bid damage for swordman
    {
        if (hero.currenthealth < hero.maxhealth)
        {
            hero.attack = hero.attack + (hero.maxhealth - hero.currenthealth);
        }
    }
    void longrangeaction(CharacterActor hero)//long range for bowman
    {
        hero.range += 5;
    }
    void untargetableaction(CharacterActor hero, CharacterActor damager)//immunity for gryffin
    {
        damager.attack = 0;
    }
    void healaction (CharacterActor hero, CharacterActor target)//heal for monk
    {
        target.currenthealth += 40;
    }

}
