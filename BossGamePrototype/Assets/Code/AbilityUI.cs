using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUI : MonoBehaviour
{
    [HideInInspector]
    public enum AbilityType { None, Bolt, Melee, Strike }
    private PlayerTargeting playerTargeting;
    private AbilityHolder abilityHolder;


    [Space]
    [Header("Ability Mods")]
    //public GameObject ability0Prefab;//the object spawned
    public AbilityType abilityType;//the type of ability
    private KeyCode abilityKey;//keycode to activate ability


    [Space]
    [Header("Ability UI")]
    public Image iconHUD;//color image on the player's HUD
    public Image strikeUI;//targeting image ui on ground
    public Image strikeMaxRangeUI;//max range for ground targeting circle

    [Space]
    public Canvas boltCanvas;//used to move the canvas
    public Image boltCircleUI;//ground targeting circle

    [Space]
    [Header("Ability Variables")]
    public float maxDistance;//max distance the ability can travel / be cast

    [Space]
    private Vector3 abilityPosition;//position of the ability
    public float groundOffset;//offset from the ground

    [Space]
    private bool onCooldown = true;//is cooldown active
    public float cooldown = 0.5f;//max cooldown amount
    private float cooldownTimer = 0f;//timer for cooldown amount


    //get targeting, toggle images off and reset fill
    void Start()
    {
        playerTargeting = GetComponent<PlayerTargeting>();
        abilityHolder = GetComponent<AbilityHolder>();

        abilityKey = abilityHolder.key;
        //reset cd icon, then clear targeting, 
        iconHUD.fillAmount = 0;

        //toggle all canvas off
        TargetingOff();
    }


    //update loops
    private void Update()
    {
        AbilityInput();
        CooldownLoop();
    }


    //ability input
    private void AbilityInput()
    {
        //toggle off all targeting (only one at a time)
        if (Input.GetKeyDown(abilityKey))
        {
            TargetingOff();
        }

        //show targeting ui while key is held and cancast
        if (Input.GetKey(abilityKey))
        {
            //calculate aim
            CalcAimLocation();
        }

        //release ability when key release and can cast
        if (Input.GetKeyUp(abilityKey) && !onCooldown)
        {
            //cast triggers
            onCooldown = true;
            iconHUD.fillAmount = 1;
        }
    }


    //calculate aim location using PlayerTargeting and target type
    private void CalcAimLocation()
    {
        //calc aim location
        abilityPosition = playerTargeting.Aim();

        //target type
        if (abilityType == AbilityType.Bolt)
        {
            BoltTarget();
        }
        if (abilityType == AbilityType.Strike)
        {
            StrikeTarget();
        }
        if (abilityType == AbilityType.Melee)
        {
            //MeleeTarget();
        }
        if (abilityType == AbilityType.None)
        {
            //NoneTarget();
        }
    }

    
    //bolt targeting ui-----------------------------------------------------------------------------------------------
    private void BoltTarget() //crossbow/firebolt
    {
        //canvas inputs, rotate ability canvas to look
        Quaternion transRot = Quaternion.LookRotation(abilityPosition - transform.position);
        boltCanvas.transform.rotation = Quaternion.Lerp(transRot, boltCanvas.transform.rotation, 0f);
        boltCanvas.enabled = true;
    }

    
    //strike targeting ui
    private void StrikeTarget() //lightning strike a location
    {
        //find bolt spawn location direction, find distance, check if within range
        var hitPosDir = (abilityPosition - transform.position).normalized;
        float distance = Vector3.Distance(abilityPosition, transform.position);
        distance = Mathf.Min(distance, maxDistance);

        //move bolt spawn canvas to new location
        var newHitPos = transform.position + (hitPosDir * distance);

        //add sett y offset
        newHitPos.y = groundOffset;

        //update ui position, enable ui
        strikeUI.transform.position = newHitPos;
        strikeUI.enabled = true;
        strikeMaxRangeUI.enabled = true;
    }

    //melee attack hitbox
    //melee targeting ui-----------------------------------------------------------------------------------------------------
    private void MeleeTarget()
    {
        Debug.LogError("ability type melee not implemented");

    }

    //targeting ui onto player target
    //non targeted ui--------------------------------------------------------------------------------------------------------
    private void NoneTarget()
    {
        Debug.LogError("ability type none not implemented");
    }


    //ability cooldowns
    private void CooldownLoop()
    {
        //if the ability is on cooldown
        if (onCooldown)
        {
            //toggle off all the targeting
            TargetingOff();

            //count down the cooldown, and update fill amount
            cooldownTimer += Time.deltaTime;
            iconHUD.fillAmount = (cooldownTimer / cooldown);

            //if the cooldown is done, reset the cooldown
            if (cooldownTimer >= cooldown)
            {
                cooldownTimer = 0;
                onCooldown = false;
            }
        }
    }


    //turn off all Images-------------------------------------------------------------------------------------------
    private void TargetingOff()
    {
        strikeUI.enabled = false;
        strikeMaxRangeUI.enabled = false;

        boltCanvas.enabled = false;

    }
}
