using UnityEngine;

public class DirectorQuizManager : MonoBehaviour
{
    public QuestionInputManager questionInputManager;
    public GameObject questionCanvas;
    private int currentQuestionIndex = 0;
    private bool hasStartedQuiz = false;
    private bool hasAnsweredCurrentQuestion = false;

    void Start()
    {
        if (questionInputManager == null)
        {
            Debug.LogWarning("QuestionInputManager is not assigned in DirectorQuizManager on " + gameObject.name);
        }
        if (questionCanvas == null)
        {
            Debug.LogWarning("QuestionCanvas is not assigned in DirectorQuizManager on " + gameObject.name);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasStartedQuiz)
        {
            Debug.Log("Director collided with Player! Starting quiz...");
            AudioManager.Instance.PlayBossMusic(); // ������������� ������ �����
            hasStartedQuiz = true;

            // ���������, ���� �� ���������� ������
            if (EnemyQuizManager.bossResults.Count == 5)
            {
                CheckBossResults();
            }
            else
            {
                ShowNextQuestion();
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && hasStartedQuiz)
        {
            Debug.Log("Director: Player exited trigger zone. Resetting current question.");
            hasAnsweredCurrentQuestion = false;
            if (questionCanvas != null)
            {
                questionCanvas.SetActive(false);
                Debug.Log("QuestionCanvas deactivated on trigger exit.");
            }
            AudioManager.Instance.StopBossMusic(); // ������������� ������ �����
        }
    }

    void ShowNextQuestion()
    {
        if (currentQuestionIndex >= 5) // 5 �������� (������� 0-4)
        {
            Debug.Log("Director: All questions answered correctly! Player wins!");
            if (questionCanvas != null)
            {
                questionCanvas.SetActive(false);
                Debug.Log("QuestionCanvas deactivated after all questions.");
            }
            AudioManager.Instance.StopBossMusic(); // ������������� ������ �����
            FindObjectOfType<GameFlowManager>().OnDirectorDefeated();
            Destroy(gameObject);
            return;
        }

        if (questionCanvas != null)
        {
            questionCanvas.SetActive(true); // ���������� ���� � ��������
            Debug.Log("QuestionCanvas activated.");
        }
        else
        {
            Debug.LogError("QuestionCanvas is null in DirectorQuizManager!");
            return;
        }

        if (questionInputManager != null)
        {
            Debug.Log($"Director: Showing question {currentQuestionIndex + 1}...");
            questionInputManager.ShowQuestion(currentQuestionIndex, OnAnswerReceived); // ���������� ������� 0-4
            hasAnsweredCurrentQuestion = false;
        }
        else
        {
            Debug.LogWarning("Cannot show question: QuestionInputManager is null on Director!");
        }
    }

    void OnAnswerReceived(string selectedAnswer)
    {
        if (hasAnsweredCurrentQuestion)
        {
            Debug.Log("Director: Answer already received for this question, ignoring.");
            return;
        }

        hasAnsweredCurrentQuestion = true;
        Debug.Log($"Director received answer for question {currentQuestionIndex + 1}: {selectedAnswer}");

        bool isCorrect = questionInputManager.CheckAnswer(currentQuestionIndex, selectedAnswer);
        if (isCorrect)
        {
            Debug.Log($"Director: Correct answer for question {currentQuestionIndex + 1}!");
            AudioManager.Instance.PlayCorrectAnswerSound(); // ���� ����������� ������
            currentQuestionIndex++;
            ShowNextQuestion();
        }
        else
        {
            Debug.Log("Director: Wrong answer! Player loses the game.");
            AudioManager.Instance.PlayWrongAnswerSound(); // ���� ������������� ������
            if (questionCanvas != null)
            {
                questionCanvas.SetActive(false);
                Debug.Log("QuestionCanvas deactivated after wrong answer.");
            }
            AudioManager.Instance.StopBossMusic(); // ������������� ������ �����
            var gameFlowManager = FindObjectOfType<GameFlowManager>();
            if (gameFlowManager != null)
            {
                gameFlowManager.GameOver();
            }
            else
            {
                Debug.LogWarning("GameFlowManager not found! Cannot trigger GameOver.");
            }
            Destroy(gameObject);
        }
    }

    void CheckBossResults()
    {
        int correctBossAnswers = 0;
        foreach (bool result in EnemyQuizManager.bossResults)
        {
            if (result)
            {
                correctBossAnswers++;
            }
        }

        Debug.Log($"Director: Checked boss results. Correct answers: {correctBossAnswers} out of 5.");

        if (correctBossAnswers == 5) // ������ ������ ���� ��� 5 �������� ����������
        {
            Debug.Log("Director: All 5 boss questions answered correctly! Player wins!");
            FindObjectOfType<GameFlowManager>().OnDirectorDefeated();
        }
        else
        {
            Debug.Log($"Director: Not all boss questions answered correctly ({correctBossAnswers}/5). Player loses!");
            var gameFlowManager = FindObjectOfType<GameFlowManager>();
            if (gameFlowManager != null)
            {
                gameFlowManager.GameOver();
            }
            else
            {
                Debug.LogWarning("GameFlowManager not found!");
            }
        }
        Destroy(gameObject);
    }
}