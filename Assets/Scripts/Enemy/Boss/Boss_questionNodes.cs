using UnityEngine;
using System.Collections;

public class Boss_questionNodes : MonoBehaviour
{
    public Player_health playerHealth;

    [Header("References")]
    public Boss_Health boss;

    // IMPORTANT: Assign SCENE objects here (not prefabs).
    // Typically these are children of your BossBattleQuestions canvas and start INACTIVE.
    public GameObject[] questionPanels;

    // Optional: kept for compatibility, but no longer used to spawn clones.
    public Transform questionParent;

    private GameObject activeQuestion;   // the currently shown panel from the scene
    private bool questionActive = false; // prevents spawning another while one is shown

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

        if (playerHealth == null) playerHealth = FindAnyObjectByType<Player_health>();
        if (Pause == null)        Pause        = GameObject.Find("Pause");
        if (WeaponSystem == null) WeaponSystem = GameObject.Find("WeaponSystem");
    }

    public void OnHit()
    {
        if (questionActive) return;

        // Pause game and HUD
        Time.timeScale = 0f;
        if (Pause)        Pause.SetActive(false);
        if (WeaponSystem) WeaponSystem.SetActive(false);

        TriggerQuestion();
    }

    void TriggerQuestion()
    {
        // pick a panel from the SCENE list that still exists
        int count = (questionPanels != null) ? questionPanels.Length : 0;
        if (count == 0)
        {
            Debug.LogWarning("[Boss_questionNodes] No questionPanels assigned in the scene!");
            return;
        }

        // Build a small pool of valid panels (not null; optionally exclude ones already destroyed)
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

        // Pick one at random and enable it
        activeQuestion = candidates[Random.Range(0, candidates.Count)];
        activeQuestion.SetActive(true);
        questionActive = true;

        // Wire handler so it can call Correct()/Wrong()
        var handler = activeQuestion.GetComponent<QuestionUIHandler>();
        if (handler != null)
            handler.currentNode = this;
    }

    public void Correct() => CorrectAnswer();
    public void Wrong()   => WrongAnswer();

    public void CorrectAnswer()
    {
        if (boss != null) boss.Damage(1);
        // Destroy the current panel after a short delay and disable this node
        StartCoroutine(CloseAfterDelay(disableThisNode: true, destroyPanel: true));
    }

    public void WrongAnswer()
    {
        if (playerHealth != null) playerHealth.TakeDamage(1);
        // Keep the current panel; resume game after delay; do NOT destroy panel
        StartCoroutine(CloseAfterDelay(disableThisNode: false, destroyPanel: false));
    }

    private IEnumerator CloseAfterDelay(bool disableThisNode, bool destroyPanel)
    {
        // Wait in real time so this still works while Time.timeScale == 0
        yield return new WaitForSecondsRealtime(postAnswerDelay);

        if (destroyPanel && activeQuestion != null)
        {
            // CORRECT: destroy and remove from pool
            for (int i = 0; i < questionPanels.Length; i++)
                if (questionPanels[i] == activeQuestion) questionPanels[i] = null;

            Destroy(activeQuestion);
            activeQuestion = null;
            questionActive = false;
        }
        else
        {
            // WRONG: just hide (no destroy), allow re-use later
            if (activeQuestion != null)
                activeQuestion.SetActive(false);

            activeQuestion = null;     // clear ref so we can pick a panel next time
            questionActive = false;    // allow re-trigger
        }

        // Resume gameplay & HUD
        if (Pause)        Pause.SetActive(true);
        if (WeaponSystem) WeaponSystem.SetActive(true);
        Time.timeScale = 1f;

        if (disableThisNode) gameObject.SetActive(false); 
    }

}
