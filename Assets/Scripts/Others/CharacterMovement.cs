using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] CharacterController characterController;
    Vector3 currentTargetOrientation;
    // Start is called before the first frame update

    private void Awake()
    {
        currentTargetOrientation = transform.forward;
    }

    private void Update()
    {
        Vector2 moveValue = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Move(moveValue);
        RotateCharacter(moveValue);
    }

    private void Move(Vector2 moveValue)
    {
        Vector3 forwardCamXZ = Camera.main.transform.forward;
        forwardCamXZ.y = 0f;
        Vector3 rightCamXZ = Camera.main.transform.right;
        rightCamXZ.y = 0f;
        Vector3 movement = ((forwardCamXZ.normalized * moveValue.y) + (rightCamXZ.normalized * moveValue.x)) * movementSpeed;
        if(moveValue != Vector2.zero)
        {
            currentTargetOrientation = movement.normalized;
        }
        if(!characterController.isGrounded)
        {
            movement += Physics.gravity;
        }
        characterController.Move(movement  * Time.deltaTime);

    }

    private void RotateCharacter(Vector2 moveValue)
    {
        if(moveValue == Vector2.zero)
        {
            return;
        }

        Quaternion targetRotation = Quaternion.LookRotation(new Vector3(currentTargetOrientation.x, 0f, currentTargetOrientation.z), Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
