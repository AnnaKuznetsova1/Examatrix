using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class GameFlowManager : MonoBehaviour
{
    public GameObject welcomeCanvas;
    public GameObject modeSelectionCanvas;
    public GameObject questionInputCanvas;
    public GameObject questionCanvas;
    public GameObject victoryCanvas;
    public GameObject hero1;
    public GameObject[] hearts;
    public GameObject gameOverCanvas;
    public TMP_Dropdown categoryDropdown;
    public QuestionInputManager questionInputManager;

    private string[] mathQuestions;
    private string[][] mathAnswers;
    private int[] mathCorrectAnswers;
    private string[] russianQuestions;
    private string[][] russianAnswers;
    private int[] russianCorrectAnswers;

    private int defeatedBosses = 0;
    private const int TOTAL_BOSSES = 5;
    private int lives = 3;

    void Awake()
    {
        // Инициализация сложных математических вопросов
        mathQuestions = new string[] {
            "Какое значение выражения 2x^2 + 3x - 4 при x = 1?",
            "Найдите НОД чисел 48 и 18.",
            "Решите уравнение 2x + 5 = 11.",
            "Каков результат √144 + 12?",
            "Чему равно 15% от 200?"
        };
        mathAnswers = new string[][] {
            new string[] { "1", "2", "3" }, // Правильный: 1
            new string[] { "6", "4", "8" }, // Правильный: 6
            new string[] { "3", "4", "5" }, // Правильный: 3
            new string[] { "18", "24", "20" }, // Правильный: 24
            new string[] { "30", "25", "35" } // Правильный: 30
        };
        mathCorrectAnswers = new int[] { 0, 0, 0, 1, 0 }; // Индексы правильных ответов (0 — первый вариант)

        // Инициализация сложных вопросов русского языка
        russianQuestions = new string[] {
            "Какой частью речи является слово 'быстро' в предложении 'Он бежал быстро'?",
            "Укажите правильное написание слова с приставкой 'пре-' или 'при-': '____вести' (привести к порядку).",
            "Какой падеж у слова 'лесу' в предложении 'Я гулял в лесу'?",
            "Какое слово является синонимом к 'прекрасный'?",
            "Какое слово является однокоренным к 'дружба'?"
        };
        russianAnswers = new string[][] {
            new string[] { "Существительное", "Наречие", "Прилагательное" }, // Правильный: Наречие
            new string[] { "Превести", "Привести" }, // Правильный: Привести
            new string[] { "Дательный", "Предложный", "Родительный" }, // Правильный: Предложный
            new string[] { "Уродливый", "Великолепный", "Простой" }, // Правильный: Великолепный
            new string[] { "Друг", "Враг", "Любовь" } // Правильный: Друг
        };
        russianCorrectAnswers = new int[] { 1, 1, 1, 1, 0 }; // Индексы правильных ответов

        // Остальной код Awake остается без изменений
    }

    void Start()
    {
        Debug.Log("GameFlowManager Start: Initializing canvases...");
        welcomeCanvas.SetActive(true);
        modeSelectionCanvas.SetActive(false);
        questionInputCanvas.SetActive(false);
        questionCanvas.SetActive(false);
        victoryCanvas.SetActive(false);
        if (gameOverCanvas != null) gameOverCanvas.SetActive(false);

        lives = 3;
        UpdateLivesUI();

        if (categoryDropdown != null)
        {
            categoryDropdown.ClearOptions();
            categoryDropdown.AddOptions(new System.Collections.Generic.List<string> { "Математика", "Русский язык" });
        }

        if (welcomeCanvas != null)
        {
            Button startButton = welcomeCanvas.GetComponentInChildren<Button>();
            if (startButton != null)
            {
                startButton.onClick.RemoveAllListeners();
                startButton.onClick.AddListener(OnWelcomeContinue);
                Debug.Log("Start button on WelcomeCanvas found and assigned to OnWelcomeContinue.");
            }
            else
            {
                Debug.LogWarning("Start button not found on WelcomeCanvas!");
            }
        }
        else
        {
            Debug.LogWarning("WelcomeCanvas is not assigned in GameFlowManager!");
        }

        if (hero1 != null)
        {
            hero1.SetActive(true);
            Debug.Log("Hero1 activated at start.");
        }
        else
        {
            Debug.LogWarning("Hero1 is not assigned in GameFlowManager!");
        }

        if (questionCanvas != null)
        {
            var qd = questionCanvas.GetComponent<QuestionDisplay>();
            if (qd == null)
            {
                Debug.LogWarning("QuestionCanvas does not have QuestionDisplay component!");
            }
            else
            {
                Debug.Log("QuestionDisplay found on QuestionCanvas at start.");
            }
        }
        else
        {
            Debug.LogWarning("QuestionCanvas is not assigned in GameFlowManager!");
        }

        // Инициализация вопросов по умолчанию (например, математика)
        if (questionInputManager == null)
        {
            questionInputManager = FindObjectOfType<QuestionInputManager>();
            if (questionInputManager == null)
            {
                Debug.LogError("QuestionInputManager not found in the scene!");
                return;
            }
        }

        QuestionData questionData = new QuestionData(mathQuestions, mathAnswers, mathCorrectAnswers);
        questionInputManager.SetDefaultQuestions(questionData.questions, questionData.answers, questionData.correctAnswers);
        Debug.Log("Default questions (Math) set at Start.");
    }

    public void OnWelcomeContinue()
    {
        Debug.Log("OnWelcomeContinue called.");
        if (welcomeCanvas != null)
        {
            welcomeCanvas.SetActive(false);
            Debug.Log("WelcomeCanvas deactivated.");
        }

        if (modeSelectionCanvas != null)
        {
            modeSelectionCanvas.SetActive(true);
            Debug.Log("ModeSelectionCanvas activated.");
        }
        else
        {
            Debug.LogWarning("ModeSelectionCanvas is not assigned!");
        }
    }

    public void OnPlayWithDefaultQuestions()
    {
        Debug.Log("Play with default questions selected.");
        if (modeSelectionCanvas != null)
        {
            modeSelectionCanvas.SetActive(false);
        }
        else
        {
            Debug.LogWarning("ModeSelectionCanvas is not assigned in GameFlowManager!");
        }

        string[] selectedQuestions;
        string[][] selectedAnswers;
        int[] selectedCorrectAnswers;

        if (categoryDropdown != null)
        {
            int selectedCategory = categoryDropdown.value;
            if (selectedCategory == 0)
            {
                selectedQuestions = mathQuestions;
                selectedAnswers = mathAnswers;
                selectedCorrectAnswers = mathCorrectAnswers;
                Debug.Log($"Math questions: {string.Join(", ", mathQuestions)}, Answers: {string.Join(", ", Array.ConvertAll(mathAnswers, x => string.Join(",", x)))}");
            }
            else
            {
                selectedQuestions = russianQuestions;
                selectedAnswers = russianAnswers;
                selectedCorrectAnswers = russianCorrectAnswers;
                Debug.Log($"Russian questions: {string.Join(", ", russianQuestions)}, Answers: {string.Join(", ", Array.ConvertAll(russianAnswers, x => string.Join(",", x)))}");
            }

            if (questionInputManager != null)
            {
                questionInputManager.SetDefaultQuestions(selectedQuestions, selectedAnswers, selectedCorrectAnswers);
                Debug.Log("Default questions set, proceeding to OnQuestionSubmitted.");
                OnQuestionSubmitted();
            }
            else
            {
                Debug.LogError("QuestionInputManager is not assigned in GameFlowManager!");
            }
        }
        else
        {
            Debug.LogError("CategoryDropdown is not assigned in GameFlowManager!");
        }
    }

    public void OnInputCustomQuestions()
    {
        Debug.Log("Input custom questions selected.");
        if (modeSelectionCanvas != null)
        {
            modeSelectionCanvas.SetActive(false);
        }

        if (questionInputCanvas != null)
        {
            questionInputCanvas.SetActive(true);
            Debug.Log("QuestionInputCanvas activated.");
        }
        else
        {
            Debug.LogWarning("QuestionInputCanvas is not assigned!");
        }
    }

    public void OnQuestionSubmitted()
    {
        Debug.Log("OnQuestionSubmitted called.");
        if (questionInputCanvas != null)
        {
            questionInputCanvas.SetActive(false);
            Debug.Log("QuestionInputCanvas deactivated.");
        }
        if (questionCanvas != null)
        {
            questionCanvas.SetActive(false);
            Debug.Log("QuestionCanvas set to inactive.");
        }
        Debug.Log("All questions submitted, starting game.");
    }

    public void OnBossDefeated()
    {
        defeatedBosses++;
        Debug.Log($"Boss defeated! Total defeated bosses: {defeatedBosses}/{TOTAL_BOSSES}");

        if (questionCanvas != null)
        {
            questionCanvas.SetActive(false);
            Debug.Log("QuestionCanvas deactivated after boss defeat.");
        }

        if (defeatedBosses >= TOTAL_BOSSES)
        {
            Debug.Log("All bosses defeated! Proceed to Director.");
        }
        else
        {
            Debug.Log("More bosses to defeat!");
        }
    }

    public void OnDirectorDefeated()
    {
        Debug.Log("Director defeated! Player wins!");
        if (victoryCanvas != null)
        {
            victoryCanvas.SetActive(true);
            Debug.Log("Showing VictoryCanvas.");
        }
        else
        {
            Debug.LogWarning("VictoryCanvas is not assigned!");
        }
    }

    public void LoseLife()
    {
        lives--;
        Debug.Log($"Player lost a life! Lives remaining: {lives}");
        UpdateLivesUI();

        if (lives <= 0)
        {
            GameOver();
        }
    }

    private void UpdateLivesUI()
    {
        if (hearts != null && hearts.Length == 3)
        {
            for (int i = 0; i < hearts.Length; i++)
            {
                hearts[i].SetActive(i < lives);
            }
            Debug.Log($"Updated lives UI: Lives: {lives}");
        }
        else
        {
            Debug.LogWarning("Hearts array is not assigned or does not contain exactly 3 elements!");
        }
    }

    public void GameOver()
    {
        Debug.Log("Game Over! Player ran out of lives.");
        if (gameOverCanvas != null)
        {
            gameOverCanvas.SetActive(true);
            Debug.Log("Showing GameOverCanvas.");
        }
        else
        {
            Debug.LogWarning("GameOverCanvas is not assigned! Using VictoryCanvas as fallback.");
            if (victoryCanvas != null)
            {
                victoryCanvas.SetActive(true);
                Debug.Log("Showing VictoryCanvas with default text.");
            }
        }

        if (hero1 != null)
        {
            hero1.SetActive(false);
            Debug.Log("Hero1 deactivated due to Game Over.");
        }
    }
}

// Класс для хранения данных вопросов
[System.Serializable]
public class QuestionData
{
    public string[] questions;
    public string[][] answers;
    public int[] correctAnswers;

    public QuestionData(string[] questions, string[][] answers, int[] correctAnswers)
    {
        this.questions = questions;
        this.answers = answers;
        this.correctAnswers = correctAnswers;
    }
}