using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float panSpeed = 20f;

    [Header("Boundary Settings")]
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX = 50f;
    [SerializeField] private float minZ = -50f;
    [SerializeField] private float maxZ = 50f;

    [Header("Zoom Settings")]
    [SerializeField] private float zoomSpeed = 20f;
    [SerializeField] private float minZoom = 5f;
    [SerializeField] private float maxZoom = 30f;

    private Camera cam;
    private Vector3 dragOrigin;
    private bool isDragging = false;

    private void Start()
    {
        cam = GetComponent<Camera>();
        if (cam == null)
        {
            cam = Camera.main;
        }
        ClampCameraPosition();
    }

    private void Update()
    {
        if (!IsPointerOverUI())
        {
            HandlePanning();
            HandleZooming();
        }
        else if (isDragging)
        {
            isDragging = false;
        }
    }

    private bool IsPointerOverUI()
    {
        if (Input.touchCount > 0)
        {
            return EventSystem.current != null &&
                   EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
        }

        return EventSystem.current != null &&
               EventSystem.current.IsPointerOverGameObject();
    }

    private void HandlePanning()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = Input.mousePosition;
            isDragging = true;
            return;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isDragging = false;
            return;
        }

        if (!isDragging)
            return;

        Vector3 pos = transform.position;
        Vector3 currentPos = Input.mousePosition;

        if (dragOrigin == currentPos)
            return;

        Vector3 worldPosStart = cam.ScreenToWorldPoint(new Vector3(dragOrigin.x, dragOrigin.y, cam.transform.position.y));
        Vector3 worldPosEnd = cam.ScreenToWorldPoint(new Vector3(currentPos.x, currentPos.y, cam.transform.position.y));
        Vector3 diff = worldPosStart - worldPosEnd;

        pos.x += diff.x;
        pos.z += diff.z;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);

        transform.position = pos;
        dragOrigin = currentPos;
    }

    private void HandleZooming()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");

        if (scroll == 0 && Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (EventSystem.current != null &&
                (EventSystem.current.IsPointerOverGameObject(touchZero.fingerId) ||
                 EventSystem.current.IsPointerOverGameObject(touchOne.fingerId)))
            {
                return;
            }

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
            float difference = currentMagnitude - prevMagnitude;
            scroll = difference * 0.01f;
        }

        if (scroll != 0)
        {
            Vector3 pos = transform.position;
            pos.y -= scroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }

    private void ClampCameraPosition()
    {
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.z = Mathf.Clamp(pos.z, minZ, maxZ);
        transform.position = pos;
    }

    public void SetBoundaries(float newMinX, float newMaxX, float newMinZ, float newMaxZ)
    {
        minX = newMinX;
        maxX = newMaxX;
        minZ = newMinZ;
        maxZ = newMaxZ;
        ClampCameraPosition();
    }

    public void SetZoomLimits(float newMinZoom, float newMaxZoom)
    {
        minZoom = newMinZoom;
        maxZoom = newMaxZoom;
        Vector3 pos = transform.position;
        pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
        transform.position = pos;
    }
}