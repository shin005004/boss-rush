using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class SceneLoader : MonoBehaviour
{
    public GameObject BlackScreen;
    public CanvasGroup canvasGroup;


    private void Start()
    {
        canvasGroup = BlackScreen.GetComponent<CanvasGroup>();
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadTitleScene()
    {
        StartCoroutine(LoadSceneAsync("Title"));
    }

    IEnumerator LoadSceneAsync(string levelToLoad)
    {
        BlackScreen.SetActive(true);
        var tweening = canvasGroup.DOFade(1f, 0.5f);

        yield return tweening.WaitForCompletion();
        AsyncOperation loadOperation = SceneManager.LoadSceneAsync(levelToLoad);

        while (!loadOperation.isDone)
        {
            yield return null;
        }

        tweening = canvasGroup.DOFade(0f, 0.5f);

        yield return tweening.WaitForCompletion();
        BlackScreen.SetActive(false);
    }

    public void EnableTransition()
    {
        StartCoroutine(ScreenTransition());
    }

    IEnumerator ScreenTransition()
    {
        BlackScreen.SetActive(true);
        var tweening = canvasGroup.DOFade(1f, 0.5f);
        yield return tweening.WaitForCompletion();
        tweening = canvasGroup.DOFade(0f, 0.5f);
        yield return tweening.WaitForCompletion();
        BlackScreen.SetActive(false);
    }
}