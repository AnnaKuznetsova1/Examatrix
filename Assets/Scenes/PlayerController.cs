using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 5f; // Скорость движения влево/вправо
    public float jumpForce = 5f; // Сила прыжка
    public float climbSpeed = 3f; // Скорость подъёма по лестнице

    private Rigidbody2D rb;
    private bool isGrounded; // На земле ли герой
    private bool isClimbing; // Находится ли герой на лестнице
    private float originalGravityScale; // Исходная гравитация

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        originalGravityScale = rb.gravityScale; // Сохраняем исходную гравитацию
    }

    void Update()
    {
        // Движение влево/вправо
        float moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // Прыжок
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isClimbing)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }

        // Движение по лестнице
        if (isClimbing)
        {
            float climbInput = Input.GetAxisRaw("Vertical");
            rb.velocity = new Vector2(rb.velocity.x, climbInput * climbSpeed);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Hero is on the ladder!");
            isClimbing = true;
            rb.gravityScale = 0f; // Отключаем гравитацию
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Сбрасываем вертикальную скорость
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ladder"))
        {
            Debug.Log("Hero left the ladder!");
            isClimbing = false;
            rb.gravityScale = originalGravityScale; // Возвращаем гравитацию
        }
    }
}