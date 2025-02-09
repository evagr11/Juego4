using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothness = 0.15f;
    public Vector3 offset; // Distancia entre la cámara y el jugador

    private Vector3 velocity = new Vector3(0, 0, 0);

    void Start()
    {
        if (offset == new Vector3(0, 0, 0))
        {
            offset = transform.position - player.position;
        }
    }

    void Update()
    {
        Vector3 targetPosition = player.position + offset;

        // Si la velocidad es muy baja, se mueve la cámara directamente
        if (velocity.sqrMagnitude < 0.01f)
        {
            velocity = new Vector3(0, 0, 0);
            transform.position = targetPosition;
        }
        else
        {
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothness);
        }
    }
}
