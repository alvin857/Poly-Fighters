using UnityEngine;

public class CameraDragPan : MonoBehaviour
{
    [Header("Pan Settings")]
    public float panSpeed = 0.5f;

    [Header("Zoom Settings")]
    public float zoomSpeed = 20f;
    public float minZoomY = 10f;
    public float maxZoomY = 40f;

    [Header("Pan Constraints")]
    public float minX = -50f;
    public float maxX = 50f;
    public float minZ = -50f;
    public float maxZ = 50f;

    private Vector3 lastMousePosition;

    void Update()
    {
        HandlePan();
        HandleZoom();
        ClampPosition();
    }

    void HandlePan()
    {
        if (Input.GetMouseButtonDown(1))
        {
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButton(1))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            Vector3 move = new Vector3(
                -mouseDelta.x * panSpeed * Time.deltaTime,
                0f,
                -mouseDelta.y * panSpeed * Time.deltaTime
            );

            transform.Translate(move, Space.World);
            lastMousePosition = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.mouseScrollDelta.y;

        if (Mathf.Abs(scroll) > 0.01f)
        {
            Vector3 zoomMove = transform.forward * scroll * zoomSpeed;
            transform.position += zoomMove;
        }
    }

    void ClampPosition()
    {
        Vector3 pos = transform.position;

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        pos.y = Mathf.Clamp(pos.y, minZoomY, maxZoomY);

        transform.position = pos;
    }
}