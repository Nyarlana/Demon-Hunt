using System.Collections;
using System.Collections.Generic;
using UnityEngine;


    [CreateAssetMenu(fileName = "New ennemi", menuName = "ennemi ")]//apperance in create 
    public class ennemi : ScriptableObject
    {
        public new string name;
     

        public int attack;
        public int health;
        public int speed;

        public Sprite artwork;


    }
