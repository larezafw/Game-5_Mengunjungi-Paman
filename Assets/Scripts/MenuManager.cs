using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] Button playButtnn;
    [SerializeField] Image progressFillBar;
    [SerializeField] TextMeshProUGUI progressFillText;
    List<AsyncOperation> scheduledSceneToLoad;

    public void PlayButton()
    {
        playButtnn.interactable = false;
        scheduledSceneToLoad = new List<AsyncOperation>();
        scheduledSceneToLoad.Add(SceneManager.LoadSceneAsync(Keyword.SCENE_INGAME));
        StartCoroutine(StartLoadProgress());
    }

    IEnumerator StartLoadProgress()
    {
        while (!scheduledSceneToLoad[0].isDone)
        {
            float progress = scheduledSceneToLoad[0].progress;
            progressFillBar.fillAmount = progress;
            progressFillText.SetText((int)(progress * 100) + "%");
            yield return null;
        }
    }
}