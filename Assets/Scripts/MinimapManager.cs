using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MinimapManager : MonoBehaviour
{
    [SerializeField] CinemachineVirtualCamera virtualCamera;
    [SerializeField] LineRenderer missionGuideLine;
    [SerializeField] Transform[] missionGuideBillboard;

    private void LateUpdate()
    {
        Vector3 currentVirtualCameraRotation = virtualCamera.Follow.parent.eulerAngles;
        currentVirtualCameraRotation.x = transform.localEulerAngles.x;
        currentVirtualCameraRotation.z = transform.localEulerAngles.z;
        transform.localEulerAngles = currentVirtualCameraRotation;

        Vector3 currentVirtualCameraPosition = virtualCamera.Follow.position;
        currentVirtualCameraPosition.y = transform.position.y;
        transform.position = currentVirtualCameraPosition;

        missionGuideLine.SetPosition(0, virtualCamera.Follow.position);
        missionGuideLine.SetPosition(1, GameManager.instance.GetCurrentMissionArea().transform.position);
        foreach (Transform billboard in missionGuideBillboard) billboard.localEulerAngles = transform.localEulerAngles;
    }
}
