using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [SerializeField] float velFor = 1f;
    [SerializeField] Animator animator;
    [SerializeField] float velRot = 50f;
    [SerializeField] float gravity = 15f;
    [SerializeField] float jumpVel = 0.01f;
    [SerializeField] CameraMovement cameraMovement;
    CharacterController characterController;
    [SerializeField] Transform groundCheck;
    Quaternion rotation;
    float yVel = 0f;
    bool isGrounded;

    Vector3 direction;
    float directionLength;
    float sin45 = Mathf.Sin(45f * Mathf.PI / 180f);
    public enum CharacterState
    {
        RUN, STRAFE, JUMP

    }
    CharacterState state = CharacterState.RUN;

    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();

        rotation = transform.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        float cameraXAngle = Camera.main.transform.rotation.eulerAngles.x;
        if (cameraXAngle > 180f)
        {
            cameraXAngle -= 360f;
        }
        animator.SetFloat("xAngle", -Mathf.Sin(cameraXAngle * Mathf.PI / 180f) / sin45);

        Debug.Log("ang: " + cameraXAngle + "   sin: " + -Mathf.Sin(cameraXAngle * Mathf.PI / 180f) / sin45);
        yVel -= Time.deltaTime * gravity;
        isGrounded = Physics.CheckSphere(groundCheck.position, 0.2f, LayerMask.GetMask("terrain"));
        if (isGrounded)
        {
            yVel = -Time.deltaTime * gravity;
            if (animator.GetBool("isJumping"))
                animator.SetBool("isJumping", false);
        }
        else
        {

            if (!animator.GetBool("isJumping"))
                animator.SetBool("isJumping", true);
            state = CharacterState.JUMP;
        }

        float vertAxis = Input.GetAxis("Vertical");
        float horAxis = Input.GetAxis("Horizontal");


        direction = new Vector3(horAxis, 0, vertAxis);
        directionLength = direction.magnitude;
        directionLength = Mathf.Clamp(directionLength, 0, 1);
        direction = direction.normalized * directionLength;

        if (state == CharacterState.RUN)
            updateRun();
        else if (state == CharacterState.STRAFE)
            updateStrafe();
        else if (state == CharacterState.JUMP)
            updateJump();


    }

    void updateRun()
    {


        Quaternion targetRot = Quaternion.identity;
        if (directionLength > 0.2f)
        {
            targetRot = Quaternion.LookRotation(direction);


        }
        rotation = Quaternion.Slerp(rotation, cameraMovement.getYRotation(), Time.deltaTime * velRot);
        targetRot = rotation * targetRot;
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * velRot * directionLength);
        if (directionLength > 0.2f)
            transform.rotation = targetRot;

        // transform.Rotate(Vector3.up * mouseX * Time.deltaTime * velFor);
        animator.SetFloat("Blend", directionLength);
        //characterController.Move(transform.forward.normalized * velFor * Time.deltaTime);
        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("atk1", true);
        }
        else
        {
            animator.SetBool("atk1", false);
        }

        if (Input.GetButton("Fire2"))
        {
            state = CharacterState.STRAFE;
            animator.SetBool("isStrafe", true);
        }
        if (Input.GetButton("Jump")) {
            yVel = jumpVel;
            state = CharacterState.JUMP;
        }



    }

    void updateJump()
    {

        if (!isGrounded)
        {

            direction = transform.forward * velFor * directionLength;
            direction.y = yVel;

            characterController.Move(direction * Time.deltaTime);
        }
        else
        {
            yVel = -Time.deltaTime * gravity;
            if (animator.GetBool("isJumping"))
                animator.SetBool("isJumping", false);
            state = CharacterState.RUN;
        }



    }

    void updateStrafe()
    {

        float vertAxis = Input.GetAxis("Vertical");
        float horAxis = Input.GetAxis("Horizontal");

        animator.SetFloat("dirX", horAxis);
        animator.SetFloat("dirY", vertAxis);

        if (Input.GetButton("Fire1"))
        {
            animator.SetBool("atk1", true);
        }
        else
        {
            animator.SetBool("atk1", false);
        }

        if (!Input.GetButton("Fire2"))
        {
            state = CharacterState.RUN;
            animator.SetBool("isStrafe", false);
        }


     
        Quaternion targetRot = cameraMovement.getYRotation();
        transform.rotation = targetRot;
    }


    private void OnAnimatorMove()
    {

        if (isGrounded)
        {
            Vector3 vel = animator.deltaPosition;
            characterController.Move(vel + Vector3.up * yVel * Time.deltaTime);
        }
    }

    public Quaternion getOrientation()
    {
        return rotation;
    }



}
