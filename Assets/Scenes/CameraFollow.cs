using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target; // ����, �� ������� ������� ������ (�������� hero1)
    public Vector3 offset = new Vector3(0, 0, -10); // �������� ������

    void Start()
    {
        // ���� target �� ��������, ����� ������
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
            // ������� �� ����� � ������ ��������
            transform.position = target.position + offset;
        }
    }
}