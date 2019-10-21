using System.Collections;
using System.Collections.Generic;
using UnityEngine;

///<summary>
///Part is another serializable data storage class just like WeaponDefinition
///</summary>
[System.Serializable]
public class Part
{
    //These three fields need to be defined in the Inspector pane
    public string name;             //The name of this object
    public float health;            //The amount of health this part has
    public string[] protectedBy;    //The other parts that protect this
    public float friendPoints;      //The number of pushes needed to "befriend"

    //These two fields are set automatically in Start().
    //Caching like this makes it faster and easier to find these later
    [HideInInspector]   //Makes field on the next line not appear in the Inspector
    public GameObject go;   //GameObject of this part
    [HideInInspector]
    public Material mat;    //Material to show damage
}

public class Enemy_4 : Enemy
{
    [Header("Set in Inspector: Enemy_4")]
        public Part[] parts;    //Array of ship Parts
    private Vector3 p0, p1;         //the two points to interpolate
    private float timeStart;        //Birth time for this Enemy_4
    private float duration = 4;     //Duration of movement

    void Start()
    {
        p0 = p1 = pos;
        InitMovement();
        print(this.name);
        //Cache GameObject & Material of each Part in parts
        Transform t;
        foreach(Part prt in parts)
        {
            t = transform.Find(prt.name);
            if(t != null)
            {
                prt.go = t.gameObject;
                prt.mat = prt.go.GetComponent<Renderer>().material;
            }
        }
        if(this.name == "Boss(Clone)")
        {
            if(Main.S.enemiesDestroyed > Main.S.enemiesBefriended)
            {
                print("a");
                parts[0].health = 20;
                parts[0].friendPoints = -10;
                parts[1].health = 20;
                parts[1].friendPoints = -10;
                parts[2].health = 30;
                parts[2].friendPoints = -20;
                parts[3].health = 30;
                parts[3].friendPoints = -20;
            }
            else if(Main.S.enemiesDestroyed < Main.S.enemiesBefriended)
            {
                print("b");
                parts[0].health = 10;
                parts[0].friendPoints = -20;
                parts[1].health = 10;
                parts[1].friendPoints = -20;
                parts[2].health = 20;
                parts[2].friendPoints = -30;
                parts[3].health = 20;
                parts[3].friendPoints = -30;
            }
            else
            {
                print("c");
                parts[0].health = 15;
                parts[0].friendPoints = -15;
                parts[1].health = 15;
                parts[1].friendPoints = -15;
                parts[2].health = 25;
                parts[2].friendPoints = -25;
                parts[3].health = 25;
                parts[3].friendPoints = -25;
            }
        }
        print(parts[0].health + parts[1].health + parts[2].health + parts[3].health);
    }

    void InitMovement()
    {
        p0 = p1; //Set p0 to the old p1
        //Assign a new on-screen location to p1
        float widMinRad = bndCheck.camWidth - bndCheck.radius;
        //print(widMinRad);
        float hgtMinRad = bndCheck.camHeight - bndCheck.radius;
        //print(isBefriended);
        if(isBefriended)
        {
            p1.x = Random.Range(-10, 9);
            if(p1.x >= 0)
            {
                p1.x = bndCheck.camWidth + bndCheck.radius;
            }
            p1.y = 0;
        }
        else
        {
            p1.x = Random.Range(-widMinRad, widMinRad);
            p1.y = Random.Range(-hgtMinRad, hgtMinRad);
        }

        //Reset the time
        timeStart = Time.time;
    }

    public override void Move()
    {
        float u = (Time.time - timeStart) / duration;

        if(u >= 1)
        {
            InitMovement();
            u = 0;
        }

        if(!isBefriended)
        {
            u = 1 - Mathf.Pow(1-u, 2);  //Apply Ease Out easing to u
            pos = (1 - u) * p0 + u*p1;  //Simple linear interpolation
        }
        else
        {
            base.Move();
        }
    }

    Part FindPart(string n)
    {
        foreach(Part prt in parts)
        {
            if(prt.name == n)
            {
                return(prt);
            }
        }
        return(null);
    }

    Part FindPart(GameObject go)
    {
        foreach(Part prt in parts)
        {
            if(prt.go == go)
            {
                return(prt);
            }
        }
        return(null);
    }

    bool Destroyed(GameObject go)
    {
        return(Destroyed(FindPart(go)));
    }

    bool Destroyed(string n)
    {
        return(Destroyed(FindPart(n)));
    }

    bool Destroyed(Part prt)
    {
        if(prt == null)     //If no real prt was passed in
        {
            return(true);
        }
        //Returns the result of the comparison: prt.health <= 0
        //If prt.health is 0 or less, returns true (yes, it was destroyed)
        return(prt.health <= 0);
    }

    bool Befriended(GameObject go)
    {
        return(Befriended(FindPart(go)));
    }

    bool Befriended(string n)
    {
        return(Befriended(FindPart(n)));
    }

    bool Befriended(Part prt)
    {
        if(!prt.go.GetComponent<Collider>().enabled)     //If no real prt was passed in
        {
            return(true);
        }
        //print(prt.name + prt.friendPoints);
        //Returns the result of the comparison: prt.friendPoints >= 0
        //If prt.friendPoints is 0 or more, returns true (yes, it was befriended)
        return(prt.friendPoints >= 0);
    }

    //This changes the color of just one Part to red instead of the whole ship.
    void ShowLocalizedDamage(Material m)
    {
        m.color = Color.red;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }
    
    void ShowLocalizedFriendship(Material m)
    {
        m.color = befriendColor;
        damageDoneTime = Time.time + showDamageDuration;
        showingDamage = true;
    }

    // bool protectsBefriended(string[] s)
    // {
    //     foreach (string name in s)
    //     {
    //         if(FindPart(name).friendPoints < 0)
    //         {
    //             return false;
    //         }
    //     }
    //     return true;
    // }

    //This will override the OnCollisionEnter that is part of Enemy.cs
    void OnCollisionEnter(Collision coll)
    {
        GameObject other = coll.gameObject;
        //print(other.tag);
        switch(other.tag)
        {
            case "ProjectileHero":
                Projectile p = other.GetComponent<Projectile>();
                //If this Enemy is off screen, don't damage it.
                if(!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                GameObject goHit = coll.contacts[0].thisCollider.gameObject;
                Part prtHit = FindPart(goHit);
                //print(goHit + ": " + prtHit);
                if(prtHit == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //check whether this part is still protected
                if(prtHit.protectedBy != null)
                {
                    foreach(string s in prtHit.protectedBy)
                    {
                        //If one of the protectiong parts hasn't been destroyed...
                        if(!Destroyed(s))
                        {
                            //...then don't damage this part yet
                            Destroy(other); //Destroy ProjectileHero
                            return;         //return before damaging Enemy_4
                        }
                    }
                }
                //It's not protected so make it take damage
                //Get the damage amount from the projectile.type and Main.W_DEFS
                prtHit.health -= Main.GetWeaponDefinition(p.type).damageOnHit;
                print(prtHit.health);
                //Show damage on part
                ShowLocalizedDamage(prtHit.mat);
                if(prtHit.health <= 0)
                {
                    //Instead of destroying this enemy, disable the damaged part
                    prtHit.go.SetActive(false);
                }
                //Check to see if the whole ship is destroyed
                bool allDestroyed = true;   //assume it is destroyed
                foreach(Part prt in parts)
                {
                    if(!Destroyed(prt))
                    {
                        allDestroyed = false;
                        break;
                    }
                }
                if(allDestroyed)
                {
                    //tell Main singleton that this ship was destroyed
                    Main.S.ShipDestroyed(this);
                    Destroy(this.gameObject);
                }
                Destroy(other);
                break;
            case "ProjectilePush":
                Projectile p2 = other.GetComponent<Projectile>();
                if(!bndCheck.isOnScreen)
                {
                    Destroy(other);
                    break;
                }
                GameObject goHit2 = coll.contacts[0].thisCollider.gameObject;
                Part prtHit2 = FindPart(goHit2);
                //print(prtHit2.name);
                if(prtHit2 == null)
                {
                    goHit = coll.contacts[0].otherCollider.gameObject;
                    prtHit = FindPart(goHit);
                }
                //check whether this part is still protected
                if(prtHit2.protectedBy != null)     // || protectsBefriended(prtHit2.protectedBy)
                {
                    foreach(string s in prtHit2.protectedBy)
                    {
                        //If one of the protecting parts hasn't been destroyed...
                        //or befriended...
                        if(!Destroyed(s))
                        {
                            //print("not friendly");
                            //...then don't damage this part yet
                            Destroy(other); //Destroy ProjectileHero
                            return;         //return before damaging Enemy_4
                        }
                    }
                }
                //It's not protected so make it take damage
                //Get the damage amount from the projectile.type and Main.W_DEFS
                prtHit2.friendPoints += Main.GetWeaponDefinition(p2.type).damageOnHit;
                //Show damage on part
                ShowLocalizedFriendship(prtHit2.mat);
                if(prtHit2.friendPoints >= 0)
                {
                    print(prtHit2.name);
                    //Instead of destroying this enemy, disable the damaged part
                    prtHit2.mat.color = befriendColor;
                    if(prtHit2.go.GetComponent<Collider>().enabled)
                    {
                        prtHit2.go.GetComponent<Collider>().enabled = !prtHit2.go.GetComponent<Collider>().enabled;
                    }
                }
                //Check to see if the whole ship is destroyed
                bool allBefriended = true;   //assume it is destroyed
                foreach(Part prt in parts)
                {
                    if(!Befriended(prt))
                    {
                        allBefriended = false;
                        break;
                    }
                }
                if(allBefriended)
                {
                    isBefriended = true;
                    Move();
                }
                Destroy(other);
                break;
            // case "Hero":
            //     print("ok");
            //     foreach(Part prt in parts)
            //     {
            //         if(prt.protectedBy.Length < 0 && prt.go.activeSelf)
            //         {
            //             print("yay?");
            //             prt.go.SetActive(false);
            //             break;
            //         }
            //     }
            //     break;
        }
    }

    // void HeroCollision()
    // {
    //     foreach(Part prt in parts)
    //     {
    //         print(prt.protectedBy.Length);
    //         if(prt.protectedBy.Length < 0 && prt.go.activeSelf)
    //         {
    //             print("yay?");
    //             prt.go.SetActive(false);
    //             break;
    //         }
    //     }
    // }
}
