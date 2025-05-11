using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionDisplay : MonoBehaviour
{
    public TextMeshProUGUI questionText;
    public Button[] answerButtons;
    public TextMeshProUGUI[] answerTexts;
    private System.Action<string> answerCallback;

    void Start()
    {
        if (answerButtons == null || answerButtons.Length == 0 || answerTexts == null || answerTexts.Length != answerButtons.Length)
        {
            Debug.LogError("Answer Buttons or Texts are not assigned properly!");
            return;
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerButtons[i] == null)
            {
                Debug.LogError($"Answer Button at index {i} is not assigned in QuestionDisplay!");
                continue;
            }

            int index = i;
            answerButtons[i].onClick.RemoveAllListeners();
            answerButtons[i].onClick.AddListener(() => OnAnswerSelected(index));
        }
    }

    public void ShowQuestion(string question, string[] answers, System.Action<string> callback)
    {
        Debug.Log($"ShowQuestion called with question: {question}, answers: {string.Join(", ", answers ?? new string[] { "null" })}");

        if (questionText == null)
        {
            Debug.LogError("Question Text is not assigned in QuestionDisplay!");
            return;
        }

        if (answerButtons == null || answerButtons.Length == 0 || answerTexts == null || answerTexts.Length != answerButtons.Length)
        {
            Debug.LogError("Answer Buttons or Texts are not assigned properly!");
            return;
        }

        if (answers == null)
        {
            Debug.LogError("Answers array is null!");
            return;
        }

        questionText.text = question ?? "No question";
        answerCallback = callback;

        for (int i = 0; i < answerButtons.Length; i++)
        {
            if (answerTexts[i] != null && i < answers.Length)
            {
                answerTexts[i].text = answers[i] ?? "No answer";
            }
        }
    }

    void OnAnswerSelected(int index)
    {
        if (answerButtons[index] == null || answerTexts[index] == null)
        {
            Debug.LogWarning($"Answer Button or Text at index {index} is null in QuestionDisplay!");
            return;
        }

        string selectedAnswer = answerTexts[index].text;
        answerCallback?.Invoke(selectedAnswer);
    }
}