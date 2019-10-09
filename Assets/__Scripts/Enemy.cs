using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Set in Inspector: Enemy")]
        public float speed = 10f; //m/s
        public float fireRate = 0.3f; //Seconds/shot (unused)
        public float health = 10;
        public int score = 100; //earned for destroying this
        public float showDamageDuration = 0.1f;
        public float powerUpDropChance = 1f; //chance to drop a PowerUp
        public float pushDuration = 0.2f;
        public float pushSpeed = 4f;
        public float speedHolder;
        public Color befriendColor; //Color to make part when befriended
        public float friendPoints;

    [Header("Set Dynamically: Enemy")]
        public Color[] originalColors;
        public Material[] materials;
        public bool showingDamage = false;
        public float damageDoneTime;
        public bool notifiedOfDestruction = false;
        public bool beingPushed = false;
        public float pushDoneTime;

    protected BoundsCheck bndCheck;
    protected bool isBefriended = false;
    protected int dir;

    void Awake()
    {
        bndCheck = GetComponent<BoundsCheck>();
        materials = Utils.GetAllMaterials(gameObject);
        originalColors = new Color[materials.Length];
        for(int i = 0; i < materials.Length; i++)
        {
            originalColors[i] = materials[i].color;
        }
        speedHolder = speed;
        dir = Random.Range(0,2)*2-1;
    }
    public Vector3 pos
    {
        get
        {
            return (this.transform.position);
        }
        set
        {
            this.transform.position = value;
        }
    }
    
    void Update()
    {
        Move();

        if(showingDamage && Time.time > damageDoneTime)
        {
            UnShowDamage();
        }

        if(beingPushed && Time.time > pushDoneTime)
        {
            UnPush();
        }

        if(bndCheck != null && (bndCheck.offDown || bndCheck.offLeft || bndCheck.offRight))
        {
            Destroy(gameObject);
        }

        if(isBefriended)
        {
            foreach (Material m in materials)
            {
                m.color = befriendColor;
            }
        }
    }

    public virtual void Move()
    {
        if(!isBefriended)
        {
            Vector3 tempPos = pos;
            tempPos.y -= speed * Time.deltaTime;
            pos = tempPos;
        }
        else
        {
            speed = 20;
            Vector3 tempPos = pos;
            tempPos.x -= speed * Time.deltaTime * dir;
            pos = tempPos;
        }
    }

    // void OnCollisionEnter(Collision coll)
    // {
    //     GameObject otherGO = coll.gameObject;
    //     if(otherGO.tag == "ProjectileHero")
    //     {
    //         Destroy(otherGO);
    //         Destroy(gameObject);
    //     }
    //     else
    //     {
    //         print("Enemy hit by non-ProjectileHero: " + otherGO.name);
    //     }
    // }

    void OnCollisionEnter(Collision coll)
    {
        GameObject otherGO = coll.gameObject;
        switch(otherGO.tag)
        {
            case "ProjectileHero":
                Projectile p = otherGO.GetComponent<Projectile>();
                if(!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                ShowDamage();
                if(health <= 0)
                {
                    //Tell the Main singleton that this ship was destroyed
                    if(!notifiedOfDestruction)
                    {
                        Main.S.ShipDestroyed(this);
                    }
                    notifiedOfDestruction = true;
                    Destroy(this.gameObject);
                }
                //print(otherGO.tag);
                Destroy(otherGO);
                break;
            case "ProjectilePush":
                Projectile pP = otherGO.GetComponent<Projectile>();
                if(!bndCheck.isOnScreen)
                {
                    Destroy(otherGO);
                    break;
                }
                //ShowDamage();
                friendPoints += Main.GetWeaponDefinition(pP.type).damageOnHit;
                Push();
                if(friendPoints >=0)
                {
                    isBefriended = true;
                }
                Destroy(otherGO);
                break;
            default:
                print("Enemy hit by non-Projectile: " + otherGO.name);
                break;
        }
    }

    void ShowDamage()
    {
        foreach(Material m in materials)
        {
            m.color = Color.red;
        }
        showingDamage = true;
        damageDoneTime = Time.time + showDamageDuration;
    }

    void UnShowDamage()
    {
        for(int i = 0; i < materials.Length; i++)
        {
            materials[i].color = originalColors[i];
        }
        showingDamage = false;
    }

    void Push()
    {
        beingPushed = true;
        speed = pushSpeed;
        pushDoneTime = Time.time + pushDuration;
    }

    void UnPush()
    {
        speed = speedHolder;
        beingPushed = false;
    }
}
