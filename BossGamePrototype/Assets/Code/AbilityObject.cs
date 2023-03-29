using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityObject : ScriptableObject
{
    [Space]
    [Header("Ability Basics")]
    public new string name;
    public float cooldownTime;
    public float activeTime;

    
    public virtual void Activate(GameObject parent)
    {
        Debug.Log("Ability Activated : " + name);
    }
}
