using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementController : MonoBehaviour
{
    /// The value for the right stick
    private Vector3 lookDirection;

    public Transform RotateContainer;
    
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
        var targetDirection = Vector3.RotateTowards(RotateContainer.forward, lookDirection, 1, 0.0f);
        RotateContainer.rotation = Quaternion.LookRotation(targetDirection);
    }
    
}
