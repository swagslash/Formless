using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerControllerAlex : MonoBehaviour
{
    /// The value for the right stick
    private Vector3 lookDirection;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        LookAround();
    }

    void OnLookAround(InputValue value)
    {
        Vector2 inputValueVector = value.Get<Vector2>();
        lookDirection = new Vector3(inputValueVector.x, 0, inputValueVector.y);
        Debug.Log(lookDirection);
    }

    void Move()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        Vector3 movementDirection = new Vector3(horizontalInput, 0, verticalInput);

        transform.Translate(movementDirection * Time.deltaTime);
    }

    void LookAround()
    {
        var rotateContainer = transform.Find("RotateContainer");
        var targetDirection = Vector3.RotateTowards(rotateContainer.forward, lookDirection, 1, 0.0f);
        rotateContainer.rotation = Quaternion.LookRotation(targetDirection);
    }
    
}
