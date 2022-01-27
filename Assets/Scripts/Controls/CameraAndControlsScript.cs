using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAndControlsScript : MonoBehaviour
{
    public UI_Script uiScript;

    private float cameraMoveSpeed = 10f;
    private float cameraZoomSpeed = 1000f;

    private bool movementEnabled = true;

    private float minZ, maxZ, minX, maxX;

    void Update()
    {
        if (movementEnabled)
        {
            //zoom kontrola
            if (Input.GetAxis("Mouse ScrollWheel") != 0)
            {
                this.transform.Translate(cameraZoomSpeed * new Vector3(0, -Input.GetAxis("Mouse ScrollWheel"), 0) * Time.deltaTime, Space.World);
            }


            //edge panning kontrola
            //gore
            if (Input.mousePosition.y >= Screen.height * 0.999)
            {
                this.transform.Translate(Vector3.forward * Time.deltaTime * cameraMoveSpeed, Space.World);
                //transform.position += cameraMoveSpeed * Vector3.forward * Time.deltaTime;
            }
            //dolje
            if (Input.mousePosition.y <= Screen.height * 0.001)
            {
                this.transform.Translate(Vector3.back * Time.deltaTime * cameraMoveSpeed, Space.World);
                //transform.position += cameraMoveSpeed * Vector3.back * Time.deltaTime;
            }
            //lijevo
            if (Input.mousePosition.x <= Screen.width * 0.001)
            {
                this.transform.Translate(Vector3.left * Time.deltaTime * cameraMoveSpeed, Space.World);
                //transform.position += cameraMoveSpeed * Vector3.left * Time.deltaTime;
            }
            //desno
            if (Input.mousePosition.x >= Screen.width * 0.999)
            {
                this.transform.Translate(Vector3.right * Time.deltaTime * cameraMoveSpeed, Space.World);
                //transform.position += cameraMoveSpeed * Vector3.right * Time.deltaTime;
            }


            //wasd kontrola i kontrola strelicama
            this.transform.Translate(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")) * Time.deltaTime * cameraMoveSpeed, Space.World);
            //if (Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0)
            //{
            //    transform.position += cameraMoveSpeed * new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * Time.deltaTime;
            //}



            //limitiranje dosega kamere
            Vector3 pos = transform.position;
            pos.x = Mathf.Clamp(transform.position.x, minX, maxX);
            pos.z = Mathf.Clamp(transform.position.z, minZ, maxZ);
            pos.y = Mathf.Clamp(transform.position.y, 2.5f, 15f);
            transform.position = pos;
        }

        //meni
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            uiScript.OpenMenu();
        }
    }

    public void SetCameraRange(Vector3 range)
    {
        minX = range.x;
        maxX = range.y;
        minZ = -2.5f;
        maxZ = range.z - 2.5f;
    }

    public void DisableMovement()
    {
        movementEnabled = false;
    }

    public void EnableMovement()
    {
        movementEnabled = true;
    }
}
