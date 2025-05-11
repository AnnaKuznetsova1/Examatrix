using UnityEngine;

public class TestTrigger : MonoBehaviour
{
    void Start()
    {
        Debug.Log("TestTrigger initialized on " + gameObject.name);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log($"TestTrigger: OnTriggerEnter2D with {collision.gameObject.name}, Tag: {collision.tag}");
    }
}