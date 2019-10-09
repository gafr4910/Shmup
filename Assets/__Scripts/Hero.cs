using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hero : MonoBehaviour
{
    static public Hero S;

    [Header("Set in Inspector")]
        public float speed = 30;
        public float rollMult = -45;
        public float pitchMult = 30;
        public float gameRestartDelay = 2f;
        public GameObject projectilePrefab;
        public float projectileSpeed = 40;
        public Weapon[] weapons;
        public WeaponType[] weaponTypes;
        ///<summary>
        //A dictionary that saves the number of weapons that have been unlocked for each 
        //WeaponType. The blaster and push are default, and will both be set as [1,0,0,0,0]
        //initially. Spread and future weapons will be locked until the player first picks
        //up that type of powerup, so they will be initialized as [0,0,0,0,0];
        ///</summary>
        public Dictionary<WeaponType, bool[]> weaponStates;

    [Header("Set Dynamically")]
        [SerializeField]
        private float _shieldLevel = 1;
        private int activeWeapon = 0;
    
    private GameObject lastTriggerGo = null;
    
    //Declare a new delegate type WeaponFireDelegate
    public delegate void WeaponFireDelegate();
    //Create a WeaponFireDelegate field named fireDelegate.
    public WeaponFireDelegate fireDelegate;

    
    void Start()
    {
        if(S == null)
        {
            S = this;
        }
        else
        {
            Debug.LogError("Hero.Awake() 0 Attempted to assign second Hero.S!");
        }
        //fireDelegate += TempFire;
        //ClearWeapons();
        //weapons[0].SetType(WeaponType.blaster);
        weaponStates = new Dictionary<WeaponType, bool[]>();
        foreach (WeaponType wt in weaponTypes)
        {
            if(wt != WeaponType.blaster && wt != WeaponType.push)
            {
                bool[] arr = new bool[]{false,false,false,false,false};
                weaponStates[wt] = arr;
            }
            else
            {
                bool[] arr = new bool[]{true,false,false,false,false};
                weaponStates[wt] = arr;
            }
        }
        SetWeapons(WeaponType.blaster);
        print(activeWeapon);
    }

    // Update is called once per frame
    void Update()
    {
        //Pull in information from the Input class
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        Vector3 pos = transform.position;
        pos.x += xAxis * speed * Time.deltaTime;
        pos.y += yAxis * speed * Time.deltaTime;
        transform.position = pos;

        //Rotate the ship to make it feel mor dynamic
        transform.rotation = Quaternion.Euler(yAxis*pitchMult, xAxis*rollMult, 0);

        // if(Input.GetKeyDown(KeyCode.Space))
        // {
        //     TempFire();
        // }
        if(Input.GetAxis("Jump") == 1 && fireDelegate != null)
        {
            fireDelegate();
        }
        print(activeWeapon);
        //print(weaponTypes[activeWeapon]);
        if(Input.GetKeyDown(KeyCode.Alpha1) )
        {
            // foreach(Weapon w in weapons)
            // {
            //     int val = (int)w.type;
            //     w.SetType((WeaponType)((val + 1) % weapons.Length));
            // }
            if(activeWeapon == 0)
            {
                activeWeapon = weaponTypes.Length - 1;
            }
            else
            {
                activeWeapon--;
            }
            SetWeapons((WeaponType)activeWeapon);
        }
        if(Input.GetKeyDown(KeyCode.Alpha2))
        {
            // foreach(Weapon w in weapons)
            // {
            //     int val = (int)w.type;
            //     w.SetType((WeaponType)((val - 1) % weapons.Length));
            // }
            activeWeapon = (activeWeapon + 1) % weaponTypes.Length;
            SetWeapons((WeaponType)activeWeapon);
        }
    }

    // void TempFire()
    // {
    //     GameObject projGO = Instantiate<GameObject>(projectilePrefab);
    //     projGO.transform.position = transform.position;
    //     Rigidbody rigidB = projGO.GetComponent<Rigidbody>();
    //     //rigidB.velocity = Vector3.up * projectileSpeed;
    //     Projectile proj = projGO.GetComponent<Projectile>();
    //     proj.type = WeaponType.blaster;
    //     float tSpeed = Main.GetWeaponDefinition(proj.type).velocity;
    //     rigidB.velocity = Vector3.up * tSpeed;
    // }

    void OnTriggerEnter(Collider other)
    {
        Transform rootT = other.gameObject.transform.root;
        GameObject go = rootT.gameObject;
        //print("Triggered: " + go.name);
        if(go == lastTriggerGo)
        {
            return;
        }
        lastTriggerGo = go;
        if(go.tag == "Enemy")
        {
            shieldLevel--;
            Destroy(go);
        }
        else if(go.tag == "PowerUp")
        {
            //If the shield was triggered by a PowerUp
            AbsorbPowerUp(go);
        }
        else
        {
            print("Triggered by non-Enemy: " + go.name);
        }
    }

    public void AbsorbPowerUp(GameObject go)
    {
        PowerUp pu = go.GetComponent<PowerUp>();
        switch(pu.type)
        {
            case WeaponType.shield:
                shieldLevel++;
                break;
            
            default:
                // if(pu.type == weapons[0].type)
                // {
                //     Weapon w = GetEmptyWeaponSlot();
                //     if(w != null)
                //     {
                //         w.SetType(pu.type);
                //     }
                // }
                // else
                // {
                //     ClearWeapons();
                //     weapons[0].SetType(pu.type);
                // }
                for(int i = 0; i < 5; i++)
                {
                    if(!weaponStates[pu.type][i])
                    {
                        weaponStates[pu.type][i] = true;
                        break;
                    }
                }
                break;
        }
        pu.AbsorbedBy(this.gameObject);
    }

    public float shieldLevel
    {
        get
        {
            return( _shieldLevel);
        }
        set
        {
            _shieldLevel = Mathf.Min(value,4);
            if(value < 0)
            {
                Destroy(this.gameObject);
                Main.S.DelayedRestart(gameRestartDelay);
            }
        }
    }
    
    Weapon GetEmptyWeaponSlot()
    {
        for(int i = 0; i < weapons.Length; i++)
        {
            if(weapons[i].type == WeaponType.none)
            {
                return(weapons[i]);
            }
        }
        return(null);
    }

    void ClearWeapons()
    {
        foreach(Weapon w in weapons)
        {
            w.SetType(WeaponType.none);
        }
        foreach(WeaponType wt in weaponTypes)
        {
            print(weaponStates);
            for(int i = 0; i < 5; i++)
            {
                if(wt == WeaponType.blaster && i == 0)
                {
                    weaponStates[wt][i] = true;
                }
                else if(wt == WeaponType.push && i == 0)
                {
                    weaponStates[wt][i] = true;
                }
                else
                {
                    weaponStates[wt][i] = false;
                }
            }
        }
    }

    void SetWeapons(WeaponType wt)
    {
        for(int i = 0; i < 5; i++)
        {
            if(weaponStates[wt][i])
            {
                weapons[i].SetType(wt);
            }
            else
            {
                weapons[i].SetType(WeaponType.none);
            }
        }
    }
}
