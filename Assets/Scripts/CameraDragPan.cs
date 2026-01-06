using UnityEngine;

public class CameraDragPan : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 0.5f;

    private Vector3 lastMousePosition;

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            // Invert to make dragging feel natural
            Vector3 move = new Vector3(
                -mouseDelta.x * panSpeed * Time.deltaTime,
                0f,
                -mouseDelta.y * panSpeed * Time.deltaTime
            );

            transform.Translate(move, Space.World);

            lastMousePosition = Input.mousePosition;
        }
    }
}
