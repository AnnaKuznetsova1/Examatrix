using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestionInputManager : MonoBehaviour
{
    public TMP_InputField questionInput;
    public TMP_InputField correctAnswerInput;
    public TMP_InputField wrongAnswer1Input;
    public TMP_InputField wrongAnswer2Input;
    public Button addQuestionButton;
    public Button finishInputButton;
    public GameObject questionCanvas;

    public GameFlowManager gameFlowManager;

    private string[] questions = new string[5];
    private string[][] answers = new string[5][];
    private int[] correctAnswerIndices = new int[5];
    private int currentBossIndex = 0;
    private const int REQUIRED_QUESTIONS = 5;

    void Awake()
    {
        Debug.Log("QuestionInputManager Awake on " + gameObject.name);
        for (int i = 0; i < REQUIRED_QUESTIONS; i++)
        {
            answers[i] = new string[3];
            Debug.Log($"Initialized answers[{i}] with length: {answers[i].Length}");
        }
    }

    void Start()
    {
        // Проверки на null для всех полей
        if (questionInput == null)
        {
            Debug.LogError("questionInput is not assigned in QuestionInputManager!");
        }
        if (correctAnswerInput == null)
        {
            Debug.LogError("correctAnswerInput is not assigned in QuestionInputManager!");
        }
        if (wrongAnswer1Input == null)
        {
            Debug.LogError("wrongAnswer1Input is not assigned in QuestionInputManager!");
        }
        if (wrongAnswer2Input == null)
        {
            Debug.LogError("wrongAnswer2Input is not assigned in QuestionInputManager!");
        }
        if (addQuestionButton == null)
        {
            Debug.LogError("addQuestionButton is not assigned in QuestionInputManager!");
        }
        if (finishInputButton == null)
        {
            Debug.LogError("finishInputButton is not assigned in QuestionInputManager!");
        }
        if (questionCanvas == null)
        {
            Debug.LogError("questionCanvas is not assigned in QuestionInputManager!");
        }
        if (gameFlowManager == null)
        {
            Debug.LogError("gameFlowManager is not assigned in QuestionInputManager!");
        }

        // Добавляем слушатели только если кнопки не null
        if (addQuestionButton != null)
        {
            addQuestionButton.onClick.AddListener(AddQuestion);
        }
        if (finishInputButton != null)
        {
            finishInputButton.onClick.AddListener(FinishInput);
            finishInputButton.interactable = false; // Строка 35
        }

        Debug.Log("QuestionInputManager initialized on " + gameObject.name);
    }

    void AddQuestion()
    {
        if (string.IsNullOrEmpty(questionInput.text) ||
            string.IsNullOrEmpty(correctAnswerInput.text) ||
            string.IsNullOrEmpty(wrongAnswer1Input.text) ||
            string.IsNullOrEmpty(wrongAnswer2Input.text))
        {
            Debug.LogWarning("Заполните все поля!");
            return;
        }

        if (currentBossIndex >= REQUIRED_QUESTIONS)
        {
            Debug.LogWarning("Уже введено 5 вопросов! Нажмите 'Завершить ввод'.");
            return;
        }

        questions[currentBossIndex] = questionInput.text;
        answers[currentBossIndex][0] = correctAnswerInput.text;
        answers[currentBossIndex][1] = wrongAnswer1Input.text;
        answers[currentBossIndex][2] = wrongAnswer2Input.text;
        correctAnswerIndices[currentBossIndex] = 0;

        Debug.Log($"Question for Boss {currentBossIndex + 1} saved: {questions[currentBossIndex]}, Answers: {string.Join(", ", answers[currentBossIndex])}");

        currentBossIndex++;
        if (currentBossIndex < REQUIRED_QUESTIONS)
        {
            questionInput.text = "";
            correctAnswerInput.text = "";
            wrongAnswer1Input.text = "";
            wrongAnswer2Input.text = "";
            Debug.Log($"Ready to input question for Boss {currentBossIndex + 1}");
        }

        if (currentBossIndex == REQUIRED_QUESTIONS)
        {
            if (finishInputButton != null)
            {
                finishInputButton.interactable = true;
            }
            Debug.Log("All 5 questions entered! You can now finish input.");
        }
    }

    void FinishInput()
    {
        if (currentBossIndex < REQUIRED_QUESTIONS)
        {
            Debug.LogWarning($"Введено только {currentBossIndex} вопросов! Нужно ввести ровно 5 вопросов.");
            return;
        }

        if (gameFlowManager != null)
        {
            Debug.Log("All questions submitted, starting game.");
            gameFlowManager.OnQuestionSubmitted();
        }
        else
        {
            Debug.LogError("gameFlowManager is not assigned, cannot call OnQuestionSubmitted!");
        }
    }

    public void ShowQuestion(int bossIndex, System.Action<string> callback)
    {
        Debug.Log($"ShowQuestion called for Boss {bossIndex + 1}. Accessing QuestionCanvas with bossIndex: {bossIndex}");
        if (questionCanvas != null)
        {
            var questionDisplay = questionCanvas.GetComponent<QuestionDisplay>();
            if (questionDisplay != null)
            {
                Debug.Log($"QuestionDisplay found. Showing question for Boss {bossIndex + 1}...");
                if (questions[bossIndex] == null || answers[bossIndex] == null)
                {
                    Debug.LogError($"Question or answers for boss {bossIndex + 1} are null! Question: {questions[bossIndex]}, Answers: {answers[bossIndex]}");
                    return;
                }
                Debug.Log($"Question data: {questions[bossIndex]}, Answers: {string.Join(", ", answers[bossIndex] ?? new string[] { "null" })}");
                questionDisplay.ShowQuestion(questions[bossIndex], answers[bossIndex], callback);
            }
            else
            {
                Debug.LogWarning("QuestionDisplay component not found on QuestionCanvas!");
            }
        }
        else
        {
            Debug.LogWarning("QuestionCanvas is not assigned in QuestionInputManager!");
        }
    }

    public bool CheckAnswer(int bossIndex, string selectedAnswer)
    {
        bool isCorrect = answers[bossIndex][correctAnswerIndices[bossIndex]] == selectedAnswer;
        Debug.Log($"CheckAnswer for Boss {bossIndex + 1}: Selected = {selectedAnswer}, Correct = {answers[bossIndex][correctAnswerIndices[bossIndex]]}, Result = {isCorrect}");
        return isCorrect;
    }

    public string GetQuestion(int bossIndex) => questions[bossIndex];
    public string[] GetAnswers(int bossIndex) => answers[bossIndex];
    public int GetCorrectAnswerIndex(int bossIndex) => correctAnswerIndices[bossIndex];

    public void SetDefaultQuestions(string[] defaultQuestions, string[][] defaultAnswers, int[] defaultCorrectAnswers)
    {
        Debug.Log("Setting default questions in QuestionInputManager.");
        if (defaultQuestions.Length != REQUIRED_QUESTIONS || defaultAnswers.Length != REQUIRED_QUESTIONS || defaultCorrectAnswers.Length != REQUIRED_QUESTIONS)
        {
            Debug.LogError("Default questions, answers, or correct answer indices do not match the required number of questions (5)!");
            return;
        }

        questions = defaultQuestions;
        answers = defaultAnswers;
        correctAnswerIndices = defaultCorrectAnswers;
        currentBossIndex = REQUIRED_QUESTIONS;

        for (int i = 0; i < answers.Length; i++)
        {
            Debug.Log($"Default Answers for boss {i + 1}: {string.Join(", ", answers[i] ?? new string[] { "null" })}");
        }
    }
}