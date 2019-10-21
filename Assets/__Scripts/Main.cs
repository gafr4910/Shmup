using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Main : MonoBehaviour
{
    static public Main S;

    static Dictionary<WeaponType, WeaponDefinition> WEAP_DICT;

    [Header("Set in Inspector")]
        public GameObject[] prefabEnemies;
        public GameObject boss;
        public float enemySpawnPerSecond = 0.5f;
        public float enemyDefaultPadding = 1.5f; //padding for position
        public WeaponDefinition[] weaponDefinitions;
        public GameObject prefabPowerUp;
        public WeaponType[] powerUpFrequency = new WeaponType[]
        {WeaponType.blaster, WeaponType.blaster, WeaponType.spread, WeaponType.shield};
        private BoundsCheck bndCheck;
        public int maxEnemies = 10;
        public int[] enemyTypeMaxes = {10,5,3,2};
        public int enemiesToBeat = 30;

    [Header("Set Dynamically")]
        public int[] enemyNumbers;
        public int totalEnemiesBeaten = 0;
        public int enemiesDestroyed = 0;
        public int enemiesBefriended = 0;

    public void ShipDestroyed(Enemy e)
    {
        //If the enemy destroyed is not Befriended, chance to spawn any of the power ups
        if(!e.isBefriended && Random.value <= e.powerUpDropChance)
        {
            int ndx = Random.Range(0, powerUpFrequency.Length);
            WeaponType puType = powerUpFrequency[ndx];
            //Spawn a PowerUp
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Set it to the proper WeaponType
            pu.SetType(puType);
            //Set it to the position of the destroyed ship
            pu.transform.position = e.transform.position;
            enemiesDestroyed++;
        }
        //If it is Befriended, chance to spawn Push power up
        else if(e.isBefriended && Random.value <= e.powerUpDropChance)
        {
            WeaponType puType = powerUpFrequency[powerUpFrequency.Length - 1];
            GameObject go = Instantiate(prefabPowerUp) as GameObject;
            PowerUp pu = go.GetComponent<PowerUp>();
            //Set it to the proper WeaponType
            pu.SetType(puType);
            //Set it to the position of the destroyed ship
            pu.transform.position = e.befriendPos;
            enemiesBefriended++;
        }
        //print(e.name);
        if(e.name == "Enemy_0(Clone)")
        {
            enemyNumbers[0]--;
        }
        if(e.name == "Enemy_1(Clone)")
        {
            enemyNumbers[1]--;
        }
        if(e.name == "Enemy_3(Clone)")
        {
            enemyNumbers[2]--;
        }
        if(e.name == "Enemy_4(Clone)")
        {
            enemyNumbers[3]--;
        }
        if(e.name == "Boss(Clone)")
        {
            Main.S.DelayedRestart(2f);
        }
        totalEnemiesBeaten++;
    }

    public void enemyUpdateNums(int[] numsToAdd)
    {
        
    }

    void Awake()
    {
        S = this;
        bndCheck = GetComponent<BoundsCheck>();
        Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
        WEAP_DICT = new Dictionary<WeaponType, WeaponDefinition>();
        foreach(WeaponDefinition def in weaponDefinitions)
        {
            WEAP_DICT[def.type] = def;
        }
        enemyNumbers = new int[] {0,0,0,0};
    }

    public void SpawnEnemy()
    {
        if(totalEnemiesBeaten <= enemiesToBeat)
        {
            int ndx = Random.Range(0,prefabEnemies.Length);
            //print(prefabEnemies[ndx].name);
            if(prefabEnemies[ndx].name == "Enemy_0")
            {
                if(enemyNumbers[0] >= enemyTypeMaxes[0])
                {
                    Invoke("SpawnEnemy", 0f);
                    return;
                }
                enemyNumbers[0]++;
                //print(enemyNumbers[0] + "/" + enemyTypeMaxes[0]);
            }
            if(prefabEnemies[ndx].name == "Enemy_1")
            {
                if(enemyNumbers[1] >= enemyTypeMaxes[1])
                {
                    Invoke("SpawnEnemy", 0f);
                    return;
                }
                enemyNumbers[1]++;
                //print(enemyNumbers[1] + "/" + enemyTypeMaxes[1]);
            }
            if(prefabEnemies[ndx].name == "Enemy_3")
            {
                //print(enemyNumbers + ": " + enemyTypeMaxes);
                if(enemyNumbers[2] >= enemyTypeMaxes[2])
                {
                    Invoke("SpawnEnemy", 0f);
                    return;
                }
                enemyNumbers[2]++;
                //print(enemyNumbers[2] + "/" + enemyTypeMaxes[2]);
            }
            if(prefabEnemies[ndx].name == "Enemy_4")
            {
                if(enemyNumbers[3] >= enemyTypeMaxes[3])
                {
                    Invoke("SpawnEnemy", 0f);
                    //print("yep");
                    return;
                }
                enemyNumbers[3]++;
                //print(enemyNumbers[3] + "/" + enemyTypeMaxes[3]);
            }
            GameObject go = Instantiate<GameObject>(prefabEnemies[ndx]);
            float enemyPadding = enemyDefaultPadding;
            if(go.GetComponent<BoundsCheck>() != null)
            {
                enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
            }
            Vector3 pos = Vector3.zero;
            float xMin = -bndCheck.camWidth + enemyPadding;
            float xMax = bndCheck.camWidth - enemyPadding;
            pos.x = Random.Range(xMin, xMax);
            pos.y = bndCheck.camHeight + enemyPadding;
            go.transform.position = pos;
            //print(go.name);

            Invoke("SpawnEnemy", 1f/enemySpawnPerSecond);
        }
        else
        {
            GameObject go = Instantiate<GameObject>(boss);
            float enemyPadding = enemyDefaultPadding;
            if(go.GetComponent<BoundsCheck>() != null)
            {
                enemyPadding = Mathf.Abs(go.GetComponent<BoundsCheck>().radius);
            }
            Vector3 pos = Vector3.zero;
            float xMin = -bndCheck.camWidth + enemyPadding;
            float xMax = bndCheck.camWidth - enemyPadding;
            pos.x = Random.Range(xMin, xMax);
            pos.y = bndCheck.camHeight + enemyPadding;
            go.transform.position = pos;
        }
    }

    public void DelayedRestart(float delay)
    {
        Invoke("Restart", delay);
    }

    public void Restart()
    {
        SceneManager.LoadScene("_Scene_0");
    }

    ///<summary>
    ///Static function that gets a WeaponDefinition from the WEAP_DICT static
    ///protected field of the Main class.
    ///</summary>
    ///<returns>The WeaponDefinition or, if there is no WeaponDefinition with
    ///the WeaponType passed in, returns a new WeaponDefinition with a WeaponType of none. </returns>
    ///<param name = "wt">The WeaponType of the desired WeaponDefinition</param>

    static public WeaponDefinition GetWeaponDefinition(WeaponType wt)
    {
        if(WEAP_DICT.ContainsKey(wt))
        {
            return(WEAP_DICT[wt]);
        }
        return(new WeaponDefinition());
    }
}
