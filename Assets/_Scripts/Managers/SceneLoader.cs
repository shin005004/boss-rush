using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using Unity.VisualScripting;

public class SceneLoader : MonoBehaviour
{
    public bool SceneLoading;
    public GameObject BlackScreen;
    public CanvasGroup canvasGroup;
    private static SceneLoader _Instance;
    void Awake()
    {
        if (null == _Instance)
        {
            _Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }

        SceneLoading = false;
    }

    public static SceneLoader Instance
    {
        get
        {
            if (null == _Instance)
            {
                return null;
            }
            return _Instance;
        }
    }

    private void Start()
    {
        canvasGroup = BlackScreen.GetComponent<CanvasGroup>();
    }

    private void OnEnable(){
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnDisable(){
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        if (scene.name == "MainStoreScene"){
            GameObject Player = GameObject.Find("Player");
            GameObject Portal = GameObject.Find("Portal " + GameManager.Instance.BookManager.BookRoomType);
            if (Player != null && Portal != null)
                Player.transform.position = Portal.transform.position;
        }
    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    public void LoadMainStoreScene()
    {
        StartCoroutine(LoadSceneAsync("MainStoreScene"));
    }
    public void LoadBookShelfScene(){
        StartCoroutine(LoadSceneAsync("BookShelfScene"));
    }
    public void LoadBossScene(string sceneName) {
        StartCoroutine(LoadSceneAsync(sceneName));
    }
    IEnumerator LoadSceneAsync(string levelToLoad)
    {
        SceneLoading = true;

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

        SceneLoading = false;
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