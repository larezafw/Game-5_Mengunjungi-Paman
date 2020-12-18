using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class AirplaneController : MonoBehaviour
{
    // MOTION
    Vector2 inputMovement;
    float speed;
    const float maxSpeed = 75f;
    const float acceleratingTime = 2.5f;

    const float rotationSpeed = 50f;
    const float zMaxPositifRotation = 40f;
    const float zMaxNegatifRotation = 320f;
    const float xMaxPositifRotation = 30f;
    const float xMaxNegatifRotation = 330f;
    const float maxHeight = 500f;

    // TAKE OFF
    bool isFLying;
    bool isTakeoff;

    // LANDING
    [SerializeField] LayerMask landingLayer;
    [SerializeField] Transform landingChecker;
    RaycastHit raycastHit;
    float rotationAdjustmentTime;
    float positionAdjustmentTime;
    float yPositionLandingAdjusment;
    float zRotationLandingAdjustment;
    float xRotationLandingAdjustment;
    bool isAbleToLanding;
    bool isLanding;

    // SWITCH TO CHARACTER
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] CharacterController character;
    [SerializeField] Transform characterExitPosition;

    // ANIMATION
    [SerializeField] Animator airplaneAnim;

    // OTHER
    [SerializeField] GameObject explosionPrefab;

    #region UNITY CALLBACK
    private void Awake()
    {
        Physics.IgnoreLayerCollision(Keyword.LAYER_AIRPLANE, Keyword.LAYER_CHARACTER);
        Physics.IgnoreLayerCollision(Keyword.LAYER_AIRPLANE, Keyword.LAYER_DEFAULT);
    }

    private void Update()
    {
        // TAKE OFF
        if (isTakeoff && speed < maxSpeed) TakeOff();
        else if (isTakeoff) SuccessfullyTakeoff();

        // LANDING
        if (!isLanding && !isTakeoff && isFLying) isAbleToLanding = AbleToLanding();
        else isAbleToLanding = false;

        if (isLanding && speed > 0) Landing();
        else if (isLanding) SuccessfullyLanding();

        // MOTION
        if (!isTakeoff && !isLanding && isFLying) RotateAirplane();
        transform.Translate(Vector3.forward * Time.deltaTime * speed);

        // ANIMATION
        airplaneAnim.SetFloat(Keyword.ANIM_PARAMETER_SWINGSPEED, speed / (maxSpeed / 2));
        GameManager.instance.AdjustToggleSpaceAvailable(isFLying ? isAbleToLanding : !isTakeoff, false, !isFLying);
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameManager.instance.Gameover(true);
        Instantiate(explosionPrefab).transform.position = transform.position;
        transform.gameObject.SetActive(false);
    }

    void OnMove(InputValue value)
    {
        inputMovement = value.Get<Vector2>();
    }

    void OnSwitchMode()
    {
        if (!isLanding && !isTakeoff && isAbleToLanding)
        {
            isLanding = true;

            yPositionLandingAdjusment = (raycastHit.point.y + 3f) - transform.position.y;
            if (transform.localEulerAngles.z > 180) zRotationLandingAdjustment = 360f - transform.localEulerAngles.z;
            else zRotationLandingAdjustment = 0f - transform.localEulerAngles.z;
            if (transform.localEulerAngles.x > 180) xRotationLandingAdjustment = 360f - transform.localEulerAngles.x;
            else xRotationLandingAdjustment = 0f - transform.localEulerAngles.x;

            rotationAdjustmentTime = 1f;
            positionAdjustmentTime = 1f;

            GameManager.instance.AdjustToggleSpaceAvailable(false);
        }
        else if (!isLanding && !isTakeoff && !isFLying)
        {
            isTakeoff = true;

            GameManager.instance.AdjustToggleEnterAvailable(false);
            GameManager.instance.AdjustToggleSpaceAvailable(false);
        }
        else Debug.Log("Can't change mode");
    }
    
    void OnSwitchController()
    {
        if (isFLying || isTakeoff || isLanding) return;

        character.transform.position = characterExitPosition.position;
        GameManager.instance.SwitchToggleDescription(true);

        Cinemachine3rdPersonFollow personFollowCamera = virtualCamera.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        personFollowCamera.CameraDistance = 5f;
        personFollowCamera.VerticalArmLength = 1f;
        virtualCamera.Follow = character.transform.Find(Keyword.GAMEOBJECT_FOLLOWTARGET);

        
        GetComponent<PlayerInput>().enabled = false;
        character.GetComponent<PlayerInput>().enabled = true;
        enabled = false;
        character.gameObject.SetActive(true);
    }

    #endregion

    #region PRIVATE FUNCTION
    bool AbleToLanding()
    {
        Vector3 rayDirection = landingChecker.position - transform.position;
        float rayMaxDistance = rayDirection.magnitude;
        rayDirection = rayDirection.normalized;

        Ray ray = new Ray(transform.position, rayDirection);

        bool qualified = Physics.Raycast(ray, out raycastHit, rayMaxDistance, landingLayer);
        if (qualified) Debug.DrawLine(transform.position, raycastHit.point, Color.green);
        else Debug.DrawLine(transform.position, transform.position + (rayDirection * rayMaxDistance), Color.red);

        return qualified;
    }

    void Landing()
    {
        if (rotationAdjustmentTime > 0)
        {
            Vector3 currentRotation = transform.localEulerAngles;
            currentRotation.z += zRotationLandingAdjustment * Time.deltaTime;
            currentRotation.x += xRotationLandingAdjustment * Time.deltaTime;
            transform.localEulerAngles = currentRotation;
            rotationAdjustmentTime -= Time.deltaTime;
        }
        else if (positionAdjustmentTime > 0)
        {
            Vector3 currentPosition = transform.position;
            currentPosition.y += yPositionLandingAdjusment * Time.deltaTime;
            transform.position = currentPosition;
            positionAdjustmentTime -= Time.deltaTime;
        }
        else if (airplaneAnim.GetBool(Keyword.ANIM_PARAMETER_ISFLYING))
        {
            airplaneAnim.SetBool(Keyword.ANIM_PARAMETER_ISFLYING, false);
        }
        else
        {
            speed -= Time.deltaTime * (maxSpeed / acceleratingTime);
            if (speed < 0) speed = 0;
        }
    }

    void SuccessfullyLanding()
    {
        isLanding = false;
        isFLying = false;

        GameManager.instance.AdjustToggleEnterAvailable(true);
        GameManager.instance.AdjustToggleSpaceAvailable(true, false, true);
    }

    void TakeOff()
    {
        speed += Time.deltaTime * (maxSpeed / acceleratingTime);
        if (speed > maxSpeed) speed = maxSpeed;
    }

    void SuccessfullyTakeoff()
    {
        airplaneAnim.SetBool(Keyword.ANIM_PARAMETER_ISFLYING, true);
        isTakeoff = false;
        isFLying = true;
    }

    void RotateAirplane()
    {
        // Z ROTATION 
        Vector3 currentRotation = transform.localEulerAngles;
        float zRotationValue = inputMovement.x * -1;
        float zCurrentAngle = currentRotation.z;

        if (zRotationValue > 0)
        {
            if (zCurrentAngle < zMaxPositifRotation || zCurrentAngle >= zMaxNegatifRotation - 5f) currentRotation.z += zRotationValue * rotationSpeed / 2 * Time.deltaTime;
        }
        else if (zRotationValue < 0)
        {
            if (zCurrentAngle > zMaxNegatifRotation || zCurrentAngle <= zMaxPositifRotation + 5f) currentRotation.z += zRotationValue * rotationSpeed / 2 * Time.deltaTime;
        }
        else if (zCurrentAngle > 0.5f && zCurrentAngle < 359.5f)
        {
            float direction;
            if (zCurrentAngle > 180) direction = 1;
            else direction = -1;
            currentRotation.z += direction * rotationSpeed / 4 * Time.deltaTime;
        }

        // Y ROTATION
        float zAfterRotatedAngle = currentRotation.z;

        if (zAfterRotatedAngle > 5 && zAfterRotatedAngle < 355)
        {
            float direction;
            float interpolateRotationSpeed;
            if (zAfterRotatedAngle > 180)
            {
                direction = 1;
                interpolateRotationSpeed = Mathf.Lerp(0f, rotationSpeed, Mathf.Clamp(360 - zAfterRotatedAngle, 0f, zMaxPositifRotation) / zMaxPositifRotation);
            }
            else
            {
                direction = -1;
                interpolateRotationSpeed = Mathf.Lerp(0f, rotationSpeed, Mathf.Clamp(0 + zAfterRotatedAngle, 0f, zMaxPositifRotation) / zMaxPositifRotation);
            }
            currentRotation.y += direction * interpolateRotationSpeed * Time.deltaTime;
        }

        // X ROTATION
        float xRotationValue = inputMovement.y;
        float xCurrentAngle = currentRotation.x;

        if (xRotationValue > 0)
        {
            if (xCurrentAngle < xMaxPositifRotation || xCurrentAngle >= xMaxNegatifRotation - 5f) currentRotation.x += xRotationValue * rotationSpeed / 3 * Time.deltaTime;
        }
        else if (xRotationValue < 0 && transform.position.y < maxHeight)
        {
            if (xCurrentAngle > xMaxNegatifRotation || xCurrentAngle <= xMaxPositifRotation + 5f) currentRotation.x += xRotationValue * rotationSpeed / 3 * Time.deltaTime;
        }
        else if (xCurrentAngle > 0.5f && xCurrentAngle < 359.5f)
        {
            float directian;
            if (xCurrentAngle > 180) directian = 1;
            else directian = -1;
            currentRotation.x += directian * rotationSpeed / 5 * Time.deltaTime;
        }

        transform.localEulerAngles = currentRotation;
    }
    #endregion
}
