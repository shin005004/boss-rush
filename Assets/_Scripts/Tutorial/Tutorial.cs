using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Tutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI tutorialText;
    [SerializeField] private GameObject text, blackScreen, skipButton;
    [SerializeField] private PlayerController playerController;
    [SerializeField] private PlayerInput playerInput;
    private List<string> progress = new List<string>() {"Welcome", "Move", "Roll", "Attack", "WriteBook", ""};
    [SerializeField] private string currentProgress;
    private bool textOn;
    private void Start()
    {
        textOn = true;
        currentProgress = progress[0];
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
                tutorialText.text = "Lost Myths에 오신 걸 환엽합니다.\n이곳에서 당신은 신화와 맞서 싸우게 될 것입니다.\nSpace로 설명을 넘길 수 있습니다.";
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
                tutorialText.text = "좌클릭으로 공격할 수 있습니다.\n한번 공격해 보세요.";
                StartCoroutine(AttackQuest());
                break;
            case "WriteBook":
                tutorialText.text = "";
                break;
            case "":
                tutorialText.text = "";
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
}
