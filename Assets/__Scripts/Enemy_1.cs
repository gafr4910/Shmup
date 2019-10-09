﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    [Header("Set in Inspector: Enemy_1")]
        public float waveFrequency = 2;
        public float waveWidth = 4;
        public float waveRotY = 45;
        private float x0;
        private float birthTime;
        private float y0;
        
    
    void Start()
    {
        x0 = pos.x;
        birthTime = Time.time;
    }

    public override void Move()
    {
        Vector3 tempPos = pos;
        float age = Time.time - birthTime;
        float theta = Mathf.PI * 2 * age / waveFrequency;
        float sin = Mathf.Sin(theta);
        if(!isBefriended)
        {
            tempPos.x = x0 + waveWidth * sin;
            pos = tempPos;
            y0 = pos.y;
        }
        else
        {
            tempPos.y = y0 + waveWidth * sin;
            pos = tempPos;
        }
        

        Vector3 rot = new Vector3(0, sin * waveRotY, 0);
        this.transform.rotation = Quaternion.Euler(rot);

        base.Move();
    }
}
