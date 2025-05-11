using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // Цель, за которой следует камера (привяжем hero1)
    public Vector3 offset = new Vector3(0, 0, -10); // Смещение камеры

    void Start()
    {
        // Если target не привязан, найдём игрока
        if (target == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                target = player.transform;
                Debug.Log("CameraFollow: Target set to player: " + player.name);
            }
            else
            {
                Debug.LogError("CameraFollow: Player not found! Please ensure an object with tag 'Player' exists.");
            }
        }
    }

    void LateUpdate()
    {
        if (target != null)
        {
            // Следуем за целью с учётом смещения
            transform.position = target.position + offset;
        }
    }
}