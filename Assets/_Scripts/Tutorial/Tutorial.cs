using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject text, blackScreen, skipButton;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput;
    [SerializeField] private QuestUI QuestManager;
    [SerializeField] private string currentProgress;
    private bool textOn;
    private void Start()
    {
        textOn = true;
        currentProgress = "Welcome";
        DontDestroyOnLoad(gameObject);
        ContinueTutorial();
    }

    private void Update()
    {
        if(textOn){
            text.SetActive(true);
            blackScreen.SetActive(true);
            Time.timeScale = 0f;
        }
        else{
            text.SetActive(false);
            blackScreen.SetActive(false);
            Time.timeScale = 1f;
        }
    }
    private void ContinueTutorial(){
        switch(currentProgress){
            case "Welcome":
                tutorialText.text = "Lost Myths에 오신 걸 환엽합니다.\n이곳에서 당신은 신화와 맞서 싸우게 될 것입니다.\n(Space로 진행)";
                StartCoroutine(Welcome());
                break;
            case "Move":
                tutorialText.text = "WASD로 움직일 수 있습니다.\n한번 움직여 보세요.";
                StartCoroutine(MoveQuest());
                break;
            case "Roll":
                tutorialText.text = "Space로 구를 수 있습니다.\n한번 굴러 보세요.";
                StartCoroutine(RollQuest());
                break;
            case "Attack":
                tutorialText.text = "좌클릭으로 공격할 수 있습니다.\n보스를 공격하게 되면\n역사서를 구매할 수 있는 재화인 성혈을 얻을 수 있습니다.\n한번 공격해 보세요.";
                StartCoroutine(AttackQuest());
                break;
            case "WriteBook":
                tutorialText.text = "특정 조건을 만족하면 보스 공략에\n도움을 주는 역사서를 작성할 수 있습니다.\n한번 F를 눌러 작성해보세요.";
                StartCoroutine(WriteBookQuest());
                break;
            case "GoBookStore":
                tutorialText.text = "보스와의 전투가 끝나면\n역사서를 구매할 수 있는 서점으로 가게 됩니다.";
                StartCoroutine(GoBookStore());
                break;
            case "GoBookShelf":
                tutorialText.text = "작성한 역사서를 구매하기 위해서는\n책장 앞에서 E를 눌러 역사서 구매 화면으로 가야 합니다.";
                StartCoroutine(GoBookShelf());
                break;
            case "BuyBook":
                tutorialText.text = "책을 좌클릭하여 책에 대한 상세 설명을 볼 수 있고,\n책을 우클릭하여 역사서를 구매할 수 있습니다.";
                StartCoroutine(BuyQuest());
                break;
            case "BackToStore":
                tutorialText.text = "우측 하단의 화살표를 통해 상점으로 다시 돌아갈 수 있습니다.";
                StartCoroutine(BackToStore());
                break;
            case "Final":
                tutorialText.text = "이제 진짜 신화와 대적하러 갈 시간입니다.\n행운을 빕니다.";
                StartCoroutine(Final());
                break;
        }

    }

    private IEnumerator Welcome(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        currentProgress = "Move";
        ContinueTutorial();
    }
    private IEnumerator MoveQuest(){
        yield return new WaitForEndOfFrame();
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => playerInput.FrameInput.Move != Vector2.zero && playerController.CanMove);
        yield return new WaitForSeconds(2f);
        currentProgress = "Roll";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator RollQuest(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => playerController.IsRolling);
        yield return new WaitForSeconds(2f);
        currentProgress = "Attack";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator AttackQuest(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => playerInput.FrameInput.AttackDown && playerController.CanAttack && playerController.CanAttackFlag);
        yield return new WaitForSeconds(2f);
        currentProgress = "WriteBook";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator WriteBookQuest(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        QuestManager.NewQuest("Scarecrow1");
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.F));
        yield return new WaitForSeconds(6f);
        currentProgress = "GoBookStore";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator GoBookStore(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        currentProgress = "GoBookShelf";
        SceneLoader.Instance.LoadScene("TutorialStoreScene");
        yield return new WaitUntil(() => SceneLoader.Instance.SceneLoading == false);
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator GoBookShelf(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "BookShelfScene");
        yield return new WaitUntil(() => SceneLoader.Instance.SceneLoading == false);
        currentProgress = "BuyBook";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator BuyQuest(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => BookData.Instance.EquippedBookLevel["Scarecrow1"] == 1);
        currentProgress = "BackToStore";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator BackToStore(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "TutorialStoreScene");
        yield return new WaitUntil(() => SceneLoader.Instance.SceneLoading == false);
        currentProgress = "Final";
        textOn = true;
        ContinueTutorial();
    }
    private IEnumerator Final(){
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        textOn = false;
        SceneLoader.Instance.LoadMainStoreScene();
        yield return new WaitUntil(() => SceneLoader.Instance.SceneLoading == false);
        Destroy(gameObject);
    }
}
