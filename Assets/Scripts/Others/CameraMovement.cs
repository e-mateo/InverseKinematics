using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] Transform cameraFollow;
    [SerializeField] Transform gameobjectToFollow;
    [SerializeField] Vector3 offsetCamera;

    [SerializeField] Transform camera;

    [SerializeField] float moveSpeedX;
    [SerializeField] float moveSpeedY;
    [SerializeField] float cameraDistance;

    bool bFirstFrame = true;

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        cameraFollow.transform.rotation = gameobjectToFollow.transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (bFirstFrame)
        {
            bFirstFrame = false;
            Input.GetAxis("Mouse X");
            Input.GetAxis("Mouse Y");
            return;
        }

        float moveCamX = moveSpeedX * Input.GetAxis("Mouse X");
        float moveCamY = moveSpeedY * Input.GetAxis("Mouse Y");

        cameraFollow.transform.position = gameobjectToFollow.transform.position + offsetCamera;
        cameraFollow.eulerAngles += new Vector3(-moveCamY * Time.deltaTime, moveCamX * Time.deltaTime, 0f);

        Camera.main.transform.position = cameraFollow.transform.position - (cameraFollow.transform.forward * cameraDistance);
        Camera.main.transform.rotation = Quaternion.LookRotation(cameraFollow.forward);
    }
}
