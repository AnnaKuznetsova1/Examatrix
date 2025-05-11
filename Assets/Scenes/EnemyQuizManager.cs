using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class EnemyQuizManager : MonoBehaviour
{
    public QuestionInputManager questionInputManager;
    public GameObject questionCanvas;
    public int bossIndex;
    private bool hasAnswered = false;
    private bool questionShown = false;
    private Animator animator;
    public float moveSpeed = 2f;
    public float moveDistance = 3f;
    private Vector2 startPosition;
    public float jumpHeight = 3f;
    public float jumpDuration = 2f;
    private bool isJumping = false;
    public float knockbackForce = 6f;
    public float returnSpeed = 2f;
    private bool isKnockedBack = false;
    private Vector2 playerStartPosition;
    public TextMeshProUGUI warningText;

    public static List<bool> bossResults = new List<bool>();

    void Start()
    {
        animator = GetComponent<Animator>();
        startPosition = transform.position;
        if (questionInputManager == null)
        {
            Debug.LogWarning("QuestionInputManager is not assigned in EnemyQuizManager on " + gameObject.name);
        }
        if (questionCanvas == null)
        {
            Debug.LogWarning("QuestionCanvas is not assigned in EnemyQuizManager on " + gameObject.name);
        }
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (!hasAnswered)
        {
            float newX = Mathf.PingPong(Time.time * moveSpeed, moveDistance) + startPosition.x - moveDistance / 2;
            transform.position = new Vector2(newX, startPosition.y);
        }
        if (isJumping)
        {
            JumpAnimation();
            ShakeBoss();
        }
        if (isKnockedBack)
        {
            ReturnPlayer();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !hasAnswered && !questionShown)
        {
            Debug.Log($"{gameObject.name} collided with Player! Activating QuestionCanvas...");
            if (questionCanvas != null)
            {
                questionCanvas.SetActive(true);
                Debug.Log("QuestionCanvas activated.");
                AudioManager.Instance.PlayBossMusic();
                ShowQuestion(); // Строка 75
                questionShown = true;
                playerStartPosition = other.transform.position;
            }
            else
            {
                Debug.LogError("QuestionCanvas is null in EnemyQuizManager!");
            }
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        Debug.Log($"{gameObject.name}: Player exited trigger zone, keeping QuestionCanvas active until answered.");
    }

    void ShowQuestion()
    {
        if (questionInputManager != null)
        {
            Debug.Log($"Calling ShowQuestion for boss {bossIndex + 1}...");
            questionInputManager.ShowQuestion(bossIndex, OnAnswerReceived); // Строка 96
        }
        else
        {
            Debug.LogError("Cannot show question: QuestionInputManager is null on " + gameObject.name);
        }
    }

    void OnAnswerReceived(string selectedAnswer)
    {
        if (hasAnswered)
        {
            Debug.Log($"{gameObject.name}: Answer already received, ignoring.");
            return;
        }

        hasAnswered = true;
        Debug.Log($"{gameObject.name} received answer: {selectedAnswer}");

        bool isCorrect = questionInputManager.CheckAnswer(bossIndex, selectedAnswer);
        if (isCorrect)
        {
            Debug.Log($"Correct answer! {gameObject.name} defeated.");
            AudioManager.Instance.PlayCorrectAnswerSound();
            animator.SetBool("IsDefeated", true);
            if (questionCanvas != null)
            {
                questionCanvas.SetActive(false);
                Debug.Log("QuestionCanvas deactivated after correct answer.");
            }
            AudioManager.Instance.StopBossMusic();
            bossResults.Add(true);
            Destroy(gameObject, 1f);
            FindObjectOfType<GameFlowManager>().OnBossDefeated();
        }
        else
        {
            Debug.Log("Wrong answer! Boss gets angry!");
            AudioManager.Instance.PlayWrongAnswerSound();
            GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 0.2f);
            animator.SetTrigger("Attack");
            isJumping = true;
            KnockbackPlayer();
            ShowWarningText();
            hasAnswered = false;
            questionShown = false;
            var gameFlowManager = FindObjectOfType<GameFlowManager>();
            if (gameFlowManager != null)
            {
                gameFlowManager.LoseLife();
            }
            else
            {
                Debug.LogWarning("GameFlowManager not found! Cannot decrease lives.");
            }
            bossResults.Add(false);
            Invoke("ResetState", jumpDuration + 5f);
        }
    }

    void JumpAnimation()
    {
        float time = Time.time % jumpDuration;
        float yOffset = Mathf.Sin(time * Mathf.PI / jumpDuration) * jumpHeight;
        Vector3 newPosition = transform.position;
        newPosition.y = startPosition.y + yOffset;
        transform.position = newPosition;
    }

    void ShakeBoss()
    {
        if (isJumping)
        {
            float shake = Mathf.Sin(Time.time * 20f) * 0.1f;
            Vector3 shakeOffset = new Vector3(shake, 0f, 0f);
            transform.position += shakeOffset;
        }
    }

    void KnockbackPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 knockbackDirection = (player.transform.position - transform.position).normalized;
            rb.AddForce(-knockbackDirection * knockbackForce, ForceMode2D.Impulse);
            isKnockedBack = true;
        }
    }

    void ReturnPlayer()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null && player.GetComponent<Rigidbody2D>() != null)
        {
            Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
            Vector2 targetPosition = playerStartPosition;
            player.transform.position = Vector2.MoveTowards(player.transform.position, targetPosition, returnSpeed * Time.deltaTime);
            if (Vector2.Distance(player.transform.position, targetPosition) < 0.1f)
            {
                isKnockedBack = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    void ShowWarningText()
    {
        if (warningText != null)
        {
            warningText.gameObject.SetActive(true);
            Debug.Log("Warning text activated!");
        }
    }

    void ResetState()
    {
        isJumping = false;
        GetComponent<SpriteRenderer>().color = Color.white;
        animator.SetBool("IsWrong", false);
        if (warningText != null)
        {
            warningText.gameObject.SetActive(false);
            Debug.Log("Warning text deactivated!");
        }
    }
}