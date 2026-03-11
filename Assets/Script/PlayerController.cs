using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Photon.Pun;


public class PlayerController : MonoBehaviourPunCallbacks
{
    [SerializeField] float rotateSpeed;
    [SerializeField] GameObject camera;
    PlayerInputs inputAction;
    CharacterController controller;
    Animator animator;
    Vector2 movementInput;
    Vector3 currentMovement;
    Quaternion rotateDir;
    bool isRun, isWalk;
    PhotonView pv;
    [SerializeField] Camerafollow myCameraScript;

    void OnMovementActions(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
        currentMovement.x = movementInput.x;
        currentMovement.z = movementInput.y;
        isWalk = movementInput.x !=0 || movementInput.y !=0;
    }

    private void Awake()
    {
        pv = GetComponentInParent<PhotonView>();

        if (!pv.IsMine)
        {
            Destroy(myCameraScript.gameObject);
        }

        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        inputAction = new PlayerInputs();

        inputAction.CharacterControlls.Movement.started += OnMovementActions;
        inputAction.CharacterControlls.Movement.performed += OnMovementActions;
        inputAction.CharacterControlls.Movement.canceled += OnMovementActions;

        inputAction.CharacterControlls.Movement.started += OnCameraMovement;
        inputAction.CharacterControlls.Movement.performed += OnCameraMovement;
        inputAction.CharacterControlls.Movement.canceled += OnCameraMovement;

        inputAction.CharacterControlls.Run.started += OnRun;
        inputAction.CharacterControlls.Run.canceled += OnRun;


    }

    public override void OnEnable()
    {
        inputAction.CharacterControlls.Enable();
    }

    public override void OnDisable()
    {
        inputAction.CharacterControlls.Disable();
    }

    void PlayerRotate()
    {
        if (isWalk)
        {
            rotateDir = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(currentMovement), rotateSpeed * Time.deltaTime);
            transform.rotation = rotateDir;
        }
    }

    void OnRun(InputAction.CallbackContext context)
    {
        isRun = context.ReadValueAsButton();
    }
    void AnimationControl()
    {
        animator.SetBool("isWalk", isWalk);
        animator.SetBool("isRun", isRun);

    }

    private void Update()
    {
        if (!pv.IsMine) return;
        AnimationControl();
        PlayerRotate();

    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;
        controller.Move(currentMovement * Time.fixedDeltaTime);
    }

    private void OnCameraMovement(InputAction.CallbackContext context)
    {
        myCameraScript.SetOffset(currentMovement);
    }

    public void Respawn()
    {
        controller.enabled = false;
        transform.position = Vector3.up;
        controller.enabled |= true;
    }
}
