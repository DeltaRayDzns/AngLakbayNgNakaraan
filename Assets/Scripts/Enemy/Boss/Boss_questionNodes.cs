using UnityEngine;
using System.Collections;

public class Boss_questionNodes : MonoBehaviour
{
    public Player_health playerHealth;

    [Header("References")]
    public Boss_Health boss;
    public GameObject[] questionPanels;
    public Transform questionParent;

    private GameObject activeQuestion;
    private bool questionActive = false;

    public GameObject Pause;
    public GameObject WeaponSystem;

    [Header("Answer Handling")]
    [SerializeField] private float postAnswerDelay = 1.25f;

    void Start()
    {
        if (boss == null)
        {
            boss = GetComponentInParent<Boss_Health>();
            Debug.Log("[Boss_questionNodes] Auto Boss_Health: " + (boss != null));
        }

        if (questionParent == null)
        {
            var canvas = GameObject.Find("BossBattleQuestions");
            if (canvas != null)
            {
                questionParent = canvas.transform;
                Debug.Log("[Boss_questionNodes] Auto questionParent: " + questionParent.name);
            }
            else
            {
                Debug.LogWarning("[Boss_questionNodes] No questionParent found! Assign it in the Inspector.");
            }
        }

        if (playerHealth == null) playerHealth = FindAnyObjectByType<Player_health>();
        if (Pause == null)        Pause        = GameObject.Find("Pause");
        if (WeaponSystem == null) WeaponSystem = GameObject.Find("WeaponSystem");
    }

    public void OnHit()
    {
        Time.timeScale = 0f;
        if (Pause)        Pause.SetActive(false);
        if (WeaponSystem) WeaponSystem.SetActive(false);

        if (!questionActive) TriggerQuestion();
    }

    void TriggerQuestion()
    {
        if (questionPanels == null || questionPanels.Length == 0 || questionParent == null) return;

        int randomIndex = Random.Range(0, questionPanels.Length);
        activeQuestion = Instantiate(questionPanels[randomIndex], questionParent);
        activeQuestion.SetActive(true);
        questionActive = true;

        var handler = activeQuestion.GetComponent<QuestionUIHandler>();
        if (handler != null) handler.currentNode = this;
    }

    public void Correct() => CorrectAnswer();
    public void Wrong()   => WrongAnswer();

    public void CorrectAnswer()
    {
        if (boss != null) boss.Damage(1);
        StartCoroutine(CloseAfterDelay(disableThisNode: true));
    }

    public void WrongAnswer()
    {
        if (playerHealth != null) playerHealth.TakeDamage(1);
        StartCoroutine(CloseAfterDelay(disableThisNode: false));
    }

    private IEnumerator CloseAfterDelay(bool disableThisNode)
    {
        yield return new WaitForSecondsRealtime(postAnswerDelay);

        if (activeQuestion) Destroy(activeQuestion);
        activeQuestion = null;
        questionActive = false;

        if (disableThisNode) gameObject.SetActive(false);

        if (Pause)        Pause.SetActive(true);
        if (WeaponSystem) WeaponSystem.SetActive(true);
        Time.timeScale = 1f;

        Debug.Log("[Boss_questionNodes] Answer resolved; panel closed and game resumed.");
    }
}
