using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    [SerializeField] private Camera cam;
    private Vector3 dragOrigin;
    private float zoomStep, minCamSize, maxCamSize;

    // Start is called before the first frame update
    void Start()
    {
        // change accordingly if the canvas ever changes size
        // higher size means less zoom, etc etc.
        minCamSize = 500;
        maxCamSize = 900;
        zoomStep = 30;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        ZoomCamera();
    }

    private void PanCamera()
    {
        if (cam.orthographicSize < maxCamSize) // prevents zooming out further than the game
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
            }

            if (Input.GetMouseButton(0))
            {
                Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);
                float newLocationX = Mathf.Clamp(cam.transform.position.x + difference.x, 890, 1670);
                float newLocationY = Mathf.Clamp(cam.transform.position.y + difference.y, 320, 940);
                cam.transform.position = new Vector3(newLocationX, newLocationY, -10);
            }
        }
        else
        {
            cam.transform.position = new Vector3(1280, 720, -10); // sets camera back to default location when fully zoomed out
        }
    }

    private void ZoomCamera()
    {
        if (Input.GetAxis("Mouse ScrollWheel") > 0f)
        {
            ZoomIn();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f)
        {
            ZoomOut();
        }
    }

    private void ZoomIn()
    {
        float newSize = cam.orthographicSize - zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }

    private void ZoomOut()
    {
        float newSize = cam.orthographicSize + zoomStep;
        cam.orthographicSize = Mathf.Clamp(newSize, minCamSize, maxCamSize);
    }
}
