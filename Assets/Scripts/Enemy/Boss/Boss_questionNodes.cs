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
	public GameObject CheckMark;
	public GameObject XMark;

    [Header("Answer Handling")]
    [SerializeField] private float postAnswerDelay = 1.25f;

    void Start()
    {
        if (boss == null)
        {
            boss = GetComponentInParent<Boss_Health>();
            Debug.Log("[Boss_questionNodes] Auto Boss_Health: " + (boss != null));
        }

        if (playerHealth == null) playerHealth = FindAnyObjectByType<Player_health>();
        if (Pause == null)        Pause        = GameObject.Find("Pause");
        if (WeaponSystem == null) WeaponSystem = GameObject.Find("WeaponSystem");
    }

    public void OnHit()
    {
        if (questionActive) return;

        Time.timeScale = 0f;
        if (Pause)        Pause.SetActive(false);
        if (WeaponSystem) WeaponSystem.SetActive(false);

        TriggerQuestion();
    }

    void TriggerQuestion()
    {
        int count = (questionPanels != null) ? questionPanels.Length : 0;
        if (count == 0)
        {
            return;
        }

        var candidates = new System.Collections.Generic.List<GameObject>(count);
        for (int i = 0; i < count; i++)
        {
            if (questionPanels[i] != null)
                candidates.Add(questionPanels[i]);
        }

        if (candidates.Count == 0)
        {
            Debug.LogWarning("[Boss_questionNodes] All questionPanels were destroyed. Nothing to show.");
            return;
        }

        activeQuestion = candidates[Random.Range(0, candidates.Count)];
        activeQuestion.SetActive(true);
        questionActive = true;
        var handler = activeQuestion.GetComponent<QuestionUIHandler>();
        if (handler != null)
            handler.currentNode = this;
    }

    public void Correct() => CorrectAnswer();
    public void Wrong()   => WrongAnswer();

    public void CorrectAnswer()
    {
        if (boss != null) boss.Damage(1);
		CheckMark.SetActive(true);
        StartCoroutine(CloseAfterDelay(disableThisNode: true, destroyPanel: true));
    }

    public void WrongAnswer()
    {
        if (playerHealth != null) playerHealth.TakeDamage(1);
		XMark.SetActive(true);
        StartCoroutine(CloseAfterDelay(disableThisNode: false, destroyPanel: false));
    }

    private IEnumerator CloseAfterDelay(bool disableThisNode, bool destroyPanel)
    {
        yield return new WaitForSecondsRealtime(postAnswerDelay);

        if (destroyPanel && activeQuestion != null)
        {
            for (int i = 0; i < questionPanels.Length; i++)
                if (questionPanels[i] == activeQuestion) questionPanels[i] = null;
			
			CheckMark.SetActive(false);
            Destroy(activeQuestion);
            activeQuestion = null;
            questionActive = false;
        }
        else
        {
            if (activeQuestion != null)
			{
				activeQuestion.SetActive(false);
				XMark.SetActive(false);
			}
                
			
            activeQuestion = null;
            questionActive = false;
        }

        if (Pause)        Pause.SetActive(true);
        if (WeaponSystem) WeaponSystem.SetActive(true);
        Time.timeScale = 1f;

        if (disableThisNode) gameObject.SetActive(false); 
    }

}
