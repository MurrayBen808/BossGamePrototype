using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Vector3 offset;
    [HideInInspector]
    public GameObject playerParent;


    private void Start()
    {
        transform.LookAt(Camera.main.transform);
        transform.Rotate(0, 0, 0);
        RectTransform rectTransform = GetComponent<RectTransform>();
        rectTransform.transform.rotation = Quaternion.Euler(0, 0, 0);
    }

    private void LateUpdate()
    {
        transform.transform.position = playerParent.transform.position + offset;
    }
}
