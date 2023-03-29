using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AbilityHolder : MonoBehaviour
{
    [Header("Abiltiy Holder")]
    public AbilityObject ability;
    public KeyCode key;
    float cooldownTime;
    float activeTime;
    
    [Space]
    [HideInInspector]
    public bool onCooldown = true;//is cooldown active
    [HideInInspector]
    public float cooldown = 0.5f;//max cooldown amount
    [HideInInspector]
    public float cooldownTimer = 0f;//timer for cooldown amount
    private bool abilityActivated = false;
    private AbilityUI abilityUI;

    enum AbilityState
    {
        Ready,
        Cooldown,
        Active
    }
    AbilityState state = AbilityState.Ready;

    #region Update Loops
    private void Start()
    {
        abilityUI = GetComponent<AbilityUI>();
    }

    
    void Update()
    {
        StateLoop();
        AbilityInput();
    }
    #endregion

    #region State Loop
    private void StateLoop()
    {
        switch (state)
        {
            case AbilityState.Ready:
                if (abilityActivated)
                {
                    ability.Activate(gameObject);
                    activeTime = ability.activeTime;
                    state = AbilityState.Cooldown;
                }
                break;

            case AbilityState.Cooldown:
                abilityActivated = false;
                if (cooldownTime > 0)
                {
                    cooldownTime= Time.deltaTime;
                }
                else
                {
                    state = AbilityState.Ready;
                }
                break;

            case AbilityState.Active:
                abilityActivated = false;
                if (activeTime > 0)
                {
                    activeTime -= Time.deltaTime;
                }
                else
                {
                    cooldownTime = ability.cooldownTime;
                    onCooldown = false;
                    state = AbilityState.Active;
                }
                break;
        }
    }
    #endregion

    #region Ability Input
    //ability input
    private void AbilityInput()
    {
        //toggle off all targeting (only one at a time)
        if (Input.GetKeyDown(key))
        {

        }

        //show targeting ui while key is held and cancast
        if (Input.GetKey(key))
        {
            //calculate aim

        }

        //release ability when key release and can cast
        if (Input.GetKeyUp(key) && !onCooldown)
        {
            //cast triggers
            abilityActivated = true;
            onCooldown = true;
        }
    }
    #endregion
}