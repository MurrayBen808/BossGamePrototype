using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyInTime : MonoBehaviour
{
    //vars
    public float time = 10f;

    //destroy after time
    private void Start()
    {
        Destroy(gameObject, time);
    }
}
