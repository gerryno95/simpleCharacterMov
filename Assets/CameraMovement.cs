using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] CharacterMovement characterMovement;
    [SerializeField] Vector3 offset;
    [SerializeField] Vector3 aimOffset;
    [SerializeField] Vector3 aimTarget;
    [SerializeField] float dumpFactor;
    float yAngle = 0f;
    float xAngle = 0f;
    [SerializeField] float yVel = 1f;
    [SerializeField] float xVel = 1f;
    Quaternion yRot,xRot;

    // Start is called before the first frame update
    void Start()
    {
        yRot = Quaternion.identity;
        xRot = Quaternion.identity;
    }

    // Update is called once per frame
    void Update()
    {
        float mouseY = Input.GetAxis("Mouse Y");

        float mouseX = Input.GetAxis("Mouse X");
        xAngle += mouseY * xVel * Time.deltaTime;

        xAngle = Mathf.Clamp(xAngle, -45f, 45f);
        yRot *= Quaternion.Euler(0f, mouseX * yVel*Time.deltaTime,0f);
        xRot = Quaternion.Euler(xAngle, 0f, 0f);

        Vector3 cameraOffset =  offset;
        Vector3 cameraAimTarget = Vector3.zero;
        if (Input.GetButton("Fire2")) {
            cameraOffset = aimOffset;
            cameraAimTarget = characterMovement.transform.rotation * aimTarget;
        }
        
        transform.position = Vector3.Lerp(transform.position,
            characterMovement.transform.position + yRot * xRot* cameraOffset + cameraAimTarget,
            Time.deltaTime*dumpFactor);
       
        Quaternion targetRot = Quaternion.LookRotation(cameraAimTarget+characterMovement.transform.position-transform.position);
        transform.rotation=Quaternion.Slerp(transform.rotation,targetRot,Time.deltaTime*dumpFactor);
    }

    public Quaternion getYRotation() {

        return yRot;
    }

   
}
