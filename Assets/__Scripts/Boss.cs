using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy_4
{
    void Awake()
    {
        print(Main.S.enemiesDestroyed + ", " + Main.S.enemiesBefriended);
        if(Main.S.enemiesDestroyed > Main.S.enemiesBefriended)
        {
            print("a");
            parts[0].health = 100;
            parts[0].friendPoints = -30;
            parts[1].health = 100;
            parts[1].friendPoints = -30;
            parts[2].health = 100;
            parts[2].friendPoints = -40;
            parts[3].health = 100;
            parts[3].friendPoints = -40;
        }
        else if(Main.S.enemiesDestroyed < Main.S.enemiesBefriended)
        {
            print("b");
            parts[0].health = 50;
            parts[0].friendPoints = -50;
            parts[1].health = 50;
            parts[1].friendPoints = -50;
            parts[2].health = 70;
            parts[2].friendPoints = -60;
            parts[3].health = 70;
            parts[3].friendPoints = -60;
        }
        else
        {
            print("c");
            parts[0].health = 80;
            parts[0].friendPoints = -30;
            parts[1].health = 80;
            parts[1].friendPoints = -30;
            parts[2].health = 90;
            parts[2].friendPoints = -50;
            parts[3].health = 90;
            parts[3].friendPoints = -50;
        }
        print(parts[0].health + parts[1].health + parts[2].health + parts[3].health);
    }
}
