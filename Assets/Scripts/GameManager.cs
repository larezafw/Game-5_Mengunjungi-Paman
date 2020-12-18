using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    // ANIMATION
    [SerializeField] Animator managerAnim;

    // GAMEOVER
    [SerializeField] GameObject gameoverImage;

    // MISSION
    [SerializeField] GameObject[] missionTriggerArea;
    [SerializeField] TextMeshProUGUI missionText;
    int missionCurrentOrder;

    // TOGGLE
    [SerializeField] Image toggleEnterImage;
    [SerializeField] Image toggleSpaceImage;
    [SerializeField] TextMeshProUGUI toggleEnterDescriptionText;
    [SerializeField] TextMeshProUGUI toggleSpaceDescriptionText;


    #region UNITY CALLBACK
    void Awake()
    {
        if (instance == null) instance = this;
        else if (instance != null && instance != this) Destroy(gameObject);

        foreach (GameObject value in missionTriggerArea) value.SetActive(false);
        Gameover(false);
        SwitchToggleDescription(true);
        AdjustToggleEnterAvailable(false);
        AdjustToggleSpaceAvailable(true);
        UpdateMission();
    }
    #endregion

    #region PUBLIC FUNCTION
    public void Gameover(bool isGameover)
    {
        if (isGameover) gameoverImage.SetActive(true);
        else gameoverImage.SetActive(false);
    }

    public void PlayAgainButton()
    {
        SceneManager.LoadScene(Keyword.SCENE_INGAME);
    }

    public void SwitchToggleDescription(bool isControllingCharacter)
    {
        if (isControllingCharacter)
        {
            toggleEnterImage.color = Keyword.COLOR_TOGGLE_CHARACTER;
            toggleSpaceImage.color = Keyword.COLOR_TOGGLE_CHARACTER;
            toggleEnterDescriptionText.SetText(Keyword.TOGGLE_DESCRIPTION_CHARACTERENTER);
            toggleSpaceDescriptionText.SetText(Keyword.TOGGLE_DESCRIPTION_CHARACTERSPACE[0]);
        }
        else
        {
            toggleEnterImage.color = Keyword.COLOR_TOGGLE_AIRPLANE;
            toggleSpaceImage.color = Keyword.COLOR_TOGGLE_AIRPLANE;
            toggleEnterDescriptionText.SetText(Keyword.TOGGLE_DESCRIPTION_AIRPLANEENTER);
            toggleSpaceDescriptionText.SetText(Keyword.TOGGLE_DESCRIPTION_AIRPLANESPACE[0]);
        }
    }

    public void AdjustToggleEnterAvailable(bool isAvailable)
    {
        Color adjustedColor = toggleEnterImage.color;
        if (isAvailable && adjustedColor.a < 1) adjustedColor.a = 1;
        else if (!isAvailable && adjustedColor.a > 0.2f) adjustedColor.a = 0.2f;

        toggleEnterImage.color = adjustedColor;
    }

    public void AdjustToggleSpaceAvailable(bool isAvailable)
    {
        Color adjustedColor = toggleSpaceImage.color;
        if (isAvailable && adjustedColor.a < 1) adjustedColor.a = 1;
        else if (!isAvailable && adjustedColor.a > 0.2f) adjustedColor.a = 0.2f;

        toggleSpaceImage.color = adjustedColor;
    }

    public void AdjustToggleSpaceAvailable(bool isAvailable, bool isControllingCharacter, bool isInFirstState)
    {
        Color adjustedColor = toggleSpaceImage.color;
        if (isAvailable && adjustedColor.a < 1) adjustedColor.a = 1;
        else if (!isAvailable && adjustedColor.a > 0.2f) adjustedColor.a = 0.2f;

        toggleSpaceImage.color = adjustedColor;

        string adjustedDescription;
        int descriptionOrder = isInFirstState ? 0 : 1;
        if (isControllingCharacter) adjustedDescription = Keyword.TOGGLE_DESCRIPTION_CHARACTERSPACE[descriptionOrder];
        else adjustedDescription = Keyword.TOGGLE_DESCRIPTION_AIRPLANESPACE[descriptionOrder];

        toggleSpaceDescriptionText.SetText(adjustedDescription);
    }

    public void UpdateMission()
    {
        managerAnim.SetBool(Keyword.ANIM_PARAMETER_ISDISPLAYMISSION, true);
        missionText.SetText(Keyword.MISSION_DESCRIPTION[missionCurrentOrder]);
        if (missionCurrentOrder < missionTriggerArea.Length) missionTriggerArea[missionCurrentOrder].SetActive(true);
        
        missionCurrentOrder += 1;
    }

    public GameObject GetCurrentMissionArea()
    {
        return missionTriggerArea[Mathf.Clamp(missionCurrentOrder - 1, 0, missionTriggerArea.Length - 1)];
    }

    public void AnimTriggerHideMission()
    {
        managerAnim.SetBool(Keyword.ANIM_PARAMETER_ISDISPLAYMISSION, false);
        if (missionCurrentOrder >= 4)
        {
            SceneManager.LoadScene(Keyword.SCENE_MENU);
        };
    }
    #endregion
}
