using UnityEngine;

public class QuestionUIHandler : MonoBehaviour
{
    [HideInInspector] public Boss_questionNodes currentNode;

    // Called by Correct Answer Button
    public void Correct()
    {
        if (currentNode != null)
            currentNode.CorrectAnswer();
        else
            Debug.LogWarning("No current node reference found!");
    }

    // Called by Wrong Answer Buttons
    public void Wrong()
    {
        if (currentNode != null)
            currentNode.WrongAnswer();
        else
            Debug.LogWarning("No current node reference found!");
    }
}