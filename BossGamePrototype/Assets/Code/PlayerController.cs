using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    #region Variables
    [Space]
    [Header("Health")]
    private int maxHealth = 100;//based on player stats
    private int currentHealth;
    public Slider healthSlider2D;//on flat ui
    public Slider healthSlider3D;//in world space


    [Space]
    [Header("Cam Shake")]
    public CameraShake camShake;//must be directly on camera
    public float shakeDuration;
    public float shakeMagnitude;


    [Space]
    [Header("Stats")]
    private int armor;//flat damage reduction


    [Space]
    [Header("Movement")]
    [HideInInspector]
    public float currentSpeed = 6f; //movement speed
    private float baseSpeed = 6f;//based on player stats
    [HideInInspector]
    public float rotationSpeed = 5f;
    private float currentSlow = 0f;


    [Space]
    [Header("Player Stats")]
    public PlayerStats baseStats;//scriptable object


    [Space]
    [Header("Debuff List")]
    public float dotTickRate = 0f;//how often the damage actually procs
    public List<SlowValues> slowValuesList = new List<SlowValues>();
    public List<DotValues> dotValuesList = new List<DotValues>();
    public List<CCValues> ccValuesList = new List<CCValues>();


    [System.Serializable]
    public class SlowValues
    {
        public float slowValue;
        public float slowDuration;
        public float slowTimer = 0f;
        public bool isDestroyed = false;
    }


    [System.Serializable]
    public class DotValues
    {
        public int dotDamagePerTick;//total damage applied
        //public int dotDamage;
        public float dotDuration;//total time it runs
        public float dotTimer;//running timer
        public bool isDestroyed = false;//flag for culling
        public float elapsedTime;//tick elapsed time
    }


    [System.Serializable]
    public class CCValues
    {
        public float ccDuration;
        public float ccTimer;
        public CCType ccType;
        public enum CCType
        {
            Root,
            Silence,
            Stun
        }
        public bool isDestroyed = false;
    }
    #endregion


    #region Update Loops
    //set health vals
    private void Start()
    {
        BaseStatsStart();
        HealthStart();
    }


    //update loops
    private void Update()
    {
        HealthUpdate();
        
        //temp debugging
        TestValues();
    }


    //movement and timing based updates
    private void FixedUpdate()
    {
        UpdateSlow();
        UpdateSpeed();
        UpdateDOT(); //UNTESTED
        UpdateCC();
    }
    #endregion


    #region Base Stats
    //instanciate base stats
    private void BaseStatsStart()
    {
        baseStats.baseMaxHealth = maxHealth;
        baseStats.baseArmor = armor;
        baseStats.baseMoveSpeed = baseSpeed;
        baseStats.baseRotationSpeed = rotationSpeed;
    }
    #endregion


    #region Health Based
    //health start values
    void HealthStart()
    {
        currentHealth = maxHealth;

        //null check for health bars
        if (healthSlider2D != null)
        {
            healthSlider2D.maxValue = maxHealth;
            healthSlider2D.value = currentHealth;
        }
        if (healthSlider3D != null)
        {
            healthSlider3D.maxValue = maxHealth;
            healthSlider3D.value = currentHealth;
        }
    }


    //update health bars
    private void HealthUpdate()
    {
        //null check and update 
        if (healthSlider2D != null)
        {
            healthSlider2D.value = currentHealth;
        }
        if (healthSlider3D != null)
        {
            healthSlider3D.value = currentHealth;
        }
    }


    //if health is over max, set it to max
    private void OverHeal()
    {
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
    }

    
    //if healed from ability update current health and check overheal
    public void TakeHealing(int healAmount)
    {
        currentHealth += healAmount;
        Debug.Log(transform.name + " heals " + healAmount + " health.");

        OverHeal();
    }

    
    //take in damage value, apply armor reduction, apply damage, apply effects, death check
    public void TakeDamage(int damage)
    {
        //apply armor reduction
        damage -= armor;
        damage = Mathf.Clamp(damage, 1, int.MaxValue);

        //apply damage value
        currentHealth -= damage;
        Debug.Log(transform.name + " takes " + damage + " dmg");

        //apply damage effects
        //TEMP

        DeathCheck();
    }


    //if health is less than 0, apply effects, and die
    private void DeathCheck()
    {
        if (currentHealth <= 0)
        {
            //Apply death effects
            Debug.Log(transform.name + " died");
            StartCoroutine(camShake.CameraShaker(shakeDuration, shakeMagnitude));//TEMP TEST CODE

            //death TEMP TEST CODE
            Destroy(gameObject);
        }
    }
    #endregion


    #region Movement Based

    //update current speed based on slow values
    private void UpdateSpeed()
    {
        //reset current speed
        currentSpeed = baseSpeed;

        //mult current speed by slow values (slows are 0-1 scale)
        currentSpeed = currentSpeed * currentSlow;
    }

    #endregion


    #region Slow
    //add slow to array
    public void AddSlow(float slowValue, float slowDuration)
    {
        SlowValues newSlowValues = new SlowValues();
        newSlowValues.slowValue = slowValue;
        newSlowValues.slowDuration = slowDuration;
        slowValuesList.Add(newSlowValues);
    }


    //update slow array and apply the greatest
    public void UpdateSlow()
    {
        //CurrentSlow takes in a slow float and a time float (slows are 0-1 scale)
        //stores an array of each slow and time float
        //checks the array every frame and removes any slow that has been active for longer than its time float
        //checks all the slows together and returns the highest slow value as the result


        //safety
        if (slowValuesList.Count == 0)
        {
            currentSlow = 1;
            return;
        }
        
        //check array for each slow
        foreach (SlowValues item in slowValuesList)
        {
            //if the slow has been active for longer than its time float, flag it for removal it from the array
            if (item.slowTimer > item.slowDuration)
            {
                item.isDestroyed = true;
            }
            //if the slow is still active, add to its timer
            else
            {
                item.slowTimer += Time.deltaTime;
            }

            //if the slow is the highest slow value, set it as the result
            if (currentSlow > item.slowValue)
            {
                currentSlow = item.slowValue;
            }
        }

        //remove flagged slows
        slowValuesList.RemoveAll(item => item.isDestroyed);
    }
    #endregion


    #region Damage DOT
    //add slow to array
    public void AddDOT(int dotDamageValue, float dotDuration)
    {
        //debug call
        Debug.Log("Damage DOT = " + dotDamageValue + "dmg for " + dotDuration + "s, for a total of = " + dotDuration * dotTickRate * dotDamageValue);


        //create and assign
        DotValues newDotValues = new DotValues();
        newDotValues.dotDuration = dotDuration;

        //connor dot tick code
        float dotTickCount = dotDuration / dotTickRate;
        newDotValues.dotDamagePerTick = Mathf.RoundToInt(dotDamageValue / dotTickCount);
        Debug.Log("tick damage is = " + newDotValues.dotDamagePerTick);

        //shit old code but uses int
        //newDotValues.dotDamage = dotDamageValue;

        dotValuesList.Add(newDotValues);
    }


    //update DOT array and apply the damage on tick
    public void UpdateDOT()
    {
        //itterate through list of dots, flag old dots for destroy, else incriment timers, and deal damage if valid tick


        //safty check
        if (dotValuesList.Count == 0)
        {
            return;
        }

        //itterate through list of dots
        foreach (DotValues item in dotValuesList)
        {
            //flag old dots for isDestroyed
            if (item.dotTimer > item.dotDuration)
            {
                item.isDestroyed = true;
            }

            //else incriment timer
            else
            {
                item.dotTimer += Time.deltaTime;
                item.elapsedTime += Time.deltaTime;

                //deal their damage if valid tick?
                if (item.elapsedTime >= dotTickRate)
                {
                    TakeDamage(item.dotDamagePerTick);
                    //TakeDamage(item.dotDamage);
                    item.elapsedTime = 0f;
                    Debug.Log("DOT tick");
                }
            }
        }

        //remove flagged DOT
        dotValuesList.RemoveAll(item => item.isDestroyed);
    }
    #endregion

    #region CC
    /*cc types are 
     * ROOT         No player movement
     * SILENCE      No player abilities
     * STUN         No player input
    */

    //add CC to array
    public void AddCC(CCValues.CCType ccType, float ccDuration)
    {
        CCValues newCCValues = new CCValues();
        newCCValues.ccType = ccType;
        newCCValues.ccDuration = ccDuration;
        ccValuesList.Add(newCCValues);
    }

    //itterate through CC array and apply effects
    private void UpdateCC()
    {
        //safty check
        if (ccValuesList.Count == 0)
        {
            return;
        }

        //check list of CC and apply effects
        foreach (CCValues item in ccValuesList)
        {
            switch (item.ccType)
            {
                case CCValues.CCType.Root:
                    break;
                case CCValues.CCType.Silence:
                    break;
                case CCValues.CCType.Stun:
                    break;
                default:
                    Debug.Log("ERROR : CC type not found");
                    break;
            }
        }
    }
    #endregion

    #region DEBUG AND TESTING
    private void TestValues()
    {
        //+ = heal by 5
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            TakeHealing(5);
        }
        //- = damage by 10
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            TakeDamage(10);
        }





        //0 = add slow by 0.99 for 1 seconds
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AddSlow(.99f, 1f);
        }
        //8 = add slow by 0.25 for 20 seconds
        if (Input.GetKeyDown(KeyCode.Alpha8))
        {
            AddSlow(.25f, 20f);
        }
        //9 = add slow by 0.5 for 5 seconds
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            AddSlow(.5f, 5f);
        }





        //1 = add dot 5dmg over 5s
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AddDOT(5, 5f);
            Debug.Log("DEBUG : add dot 5dmg over 5s");
        }
        //2 = add dot 50dmg over 3s
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            AddDOT(10, 3f);
            Debug.Log("DEBUG : add dot 10dmg over 3s");

        }
    }
    #endregion
}