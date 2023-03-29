using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Windows;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using Input = UnityEngine.Input;

public class PlayerMovement : MonoBehaviour
{
    //interactive variables
    [SerializeField] private bool RotateTowardMouse;

    //cam for targeting
    private Camera cam;

    //inputs
    [HideInInspector]
    [SerializeField] private Vector2 inputVector; // WASD input
    [HideInInspector]
    [SerializeField] private Vector3 mousePos; //world space mouse pos
    private Vector3 movementVector;

    //movement type
    public enum movementType
    {
        usingForce,
        usingVelocity,
        usingTranslate
    }

    public movementType currentType = movementType.usingTranslate;

    //scripts
    private PlayerController PC;
    private Rigidbody rb;
    

    //fetchs
    private void Start()
    {
        cam = Camera.main;
        PC = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    
    //update loops
    void Update()
    {
        InputVars();
        Targeting();
    }


    //determine targeting
    private void Targeting()
    {
        Vector3 targetVector = new Vector3(inputVector.x, 0, inputVector.y);


        //use selected movement type
        if (currentType == movementType.usingForce)
        {
            movementVector = ForceTowardTarget(targetVector);
        }
        else if (currentType == movementType.usingTranslate)
        {
            movementVector = MoveTowardTarget(targetVector);
        }
        else if (currentType == movementType.usingVelocity)
        {
            movementVector = VelocityTowardTarget(targetVector);
        }

        //if we using rotate bool
        if (RotateTowardMouse)
        {
            RotateFromMouseVector();
        }
        else
        {
            RotateTowardMovementVector(movementVector);
        }
    }


    //player input
    private void InputVars()
    {
        //fetch axis var
        var h = Input.GetAxis("Horizontal");
        var v = Input.GetAxis("Vertical");
        inputVector = new Vector2(h, v);

        //fetch mouse position
        mousePos = Input.mousePosition;
    }


    //rotate player body to look at mouse pos
    private void RotateFromMouseVector()
    {
        Ray ray = cam.ScreenPointToRay(mousePos);

        if (Physics.Raycast(ray, out RaycastHit hitInfo, maxDistance: 300f))
        {
            var target = hitInfo.point;
            target.y = transform.position.y;
            transform.LookAt(target);
        }
    }


    //move player towards 
    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        //calc speed
        var speed = PC.currentSpeed * Time.deltaTime;

        //find target
        targetVector = Quaternion.Euler(0, cam.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var targetPosition = transform.position + targetVector * speed;

        //translate
        transform.position = targetPosition;

        return targetVector;
    }


    //if using momentum
    private Vector3 ForceTowardTarget(Vector3 targetVector)
    {
        //calc speed
        var speed = PC.currentSpeed * Time.deltaTime;

        //find target
        targetVector = Quaternion.Euler(0, cam.gameObject.transform.rotation.eulerAngles.y, 0) * targetVector;
        var forceDirection = targetVector * speed;

        //add force
        rb.AddForce(forceDirection.normalized, ForceMode.Impulse);

        return targetVector;
    }


    //if using velocity
    private Vector3 VelocityTowardTarget(Vector3 targetVector)
    {
        //velocity = speed * direction
        rb.velocity = new Vector3(targetVector.x, 0, targetVector.z) * PC.currentSpeed;

        return targetVector;
    }


    //if moving, update rotation to move dir
    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if (movementDirection.magnitude == 0) 
        { 
            return; 
        }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, PC.rotationSpeed * Time.deltaTime);
    }
}
