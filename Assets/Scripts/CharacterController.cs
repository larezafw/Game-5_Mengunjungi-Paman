using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class CharacterController : MonoBehaviour
{
    // MOTION
    Vector2 inputMovement;
    [SerializeField] Transform followCameraTarget;
    float speed = 7.5f;
    const float normalSpeed= 7.5f;
    const float superSpeed = 15f;
    const float rotationSpeed = 100f;

    // SWITCH CONTROLLER
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField]AirplaneController airplane;
    bool isAbleToSwitch;

    // ANIMATION
    [SerializeField] Animator characterAnim;

    #region UNITY CALLBACK
    private void Update()
    {
        // MOTION
        if (inputMovement.x != 0) RotateFollowTarget();
        if (inputMovement.y != 0) Walking();
        else characterAnim.SetFloat(Keyword.ANIM_PARAMETER_MOVEMENTSPEED, 0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.name == Keyword.MISSION_1 || other.name == Keyword.MISSION_2 || other.name == Keyword.MISSION_3)
        {
            other.gameObject.SetActive(false);
            GameManager.instance.UpdateMission();
        }
        else if (other.name == Keyword.GAMEOBJECT_AIRPLANEENTERAREA) 
        {
            isAbleToSwitch = true;
            GameManager.instance.AdjustToggleEnterAvailable(true); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == Keyword.GAMEOBJECT_AIRPLANEENTERAREA)
        {
            isAbleToSwitch = false;
            GameManager.instance.AdjustToggleEnterAvailable(false); 
        }
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }
    void OnSwitchMode()
    {
        if (speed == normalSpeed) speed = superSpeed;
        else speed = normalSpeed;
        GameManager.instance.AdjustToggleSpaceAvailable(true, true, speed == normalSpeed);
    }
    void OnSwitchController()
    {
        if (!isAbleToSwitch) return;

        speed = normalSpeed;
        GameManager.instance.SwitchToggleDescription(false);

        Cinemachine3rdPersonFollow personFollowCamera = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        personFollowCamera.CameraDistance = 12.5f;
        personFollowCamera.VerticalArmLength = 0f;
        virtualCamera.Follow = airplane.transform.Find(Keyword.GAMEOBJECT_FOLLOWTARGET);

        GetComponent<PlayerInput>().enabled = false;
        airplane.GetComponent<PlayerInput>().enabled = true;
        gameObject.SetActive(false);
        airplane.enabled = true;
    }
    #endregion

    #region PRIVATE FUNCTION
    void RotateFollowTarget()
    {
        followCameraTarget.localEulerAngles += Vector3.up * inputMovement.x * rotationSpeed * Time.deltaTime;
    }

    void Walking()
    {
        transform.localEulerAngles = followCameraTarget.eulerAngles;
        followCameraTarget.localEulerAngles = new Vector3();
        transform.Translate(Vector3.forward * inputMovement.y * speed * Time.deltaTime);
        characterAnim.SetFloat(Keyword.ANIM_PARAMETER_MOVEMENTSPEED, speed/normalSpeed);
    }
    #endregion
}
