using System.Threading.Tasks;
using UnityEngine;



public class CameraController : MonoBehaviour
{
    // debug params
    public Transform planeTransform;
    public float planeSize = 10f;

    // objects
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    public Camera _mainCamera;
    //public GameObject testPlaceObject;
    
    // transform params
    Plane _cameraPlane;
    Vector3 _cameraNormal;
    public float movementSpeed = 10.0f; // Скорость движения камеры



    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            Place();
        }

        // if (Input.GetKey(KeyCode.Space)) {
        //     SetPos(GetPosByFieldPos(testPos));
        // }
    }
    
    void Initialize()
    {
        if (_cameraHolder is null || _cameraObject is null || _mainCamera is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }

        _cameraPlane = GetCameraPlane();
        _cameraNormal = -_cameraPlane.normal;
    }

    Plane GetCameraPlane()
    {
        Vector3 rotationNormal = _cameraHolder.transform.rotation.eulerAngles;
        Vector3 pointOnPlane = _cameraHolder.transform.position;

        return new Plane(rotationNormal, pointOnPlane);
    }

    // get position on gamefield under cursor
    Vector3 GetFieldPosByClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); // Создаем луч из камеры в направлении курсора мыши
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject.CompareTag("GameField")) {
                var fieldPos = hit.point;
                return fieldPos;
            }
            return Vector3.zero;
        }
        return Vector3.zero;
    }

    Vector3 GetPosByFieldPos(Vector3 fieldPos)
    {
        _cameraNormal = _cameraHolder.transform.forward;
        Ray rayFromGameField = new Ray(fieldPos, -_cameraNormal);
        Debug.DrawRay(rayFromGameField.origin, rayFromGameField.direction * 100, Color.red);

        Vector3 intersection;
        
        if (_cameraPlane.Raycast(rayFromGameField, out float distanceToIntersection)) {
            intersection = rayFromGameField.GetPoint(distanceToIntersection);
        }
        else {
            Debug.LogError("Нет пересечения c плоскостью камеры.");
            return Vector3.zero;
        }

        return intersection;
    }

    void SetPos(Vector3 pos)
    {   
        _cameraObject.transform.position = pos;
    }

    void Place() 
    {
        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 newCameraPos = GetPosByFieldPos(fieldClickedPos);
        SetPos(newCameraPos);
    }







    void DrawPlane(Transform transform)
    {
        if (transform != null)
        {
            Vector3 center = planeTransform.position;
            Vector3 normal = planeTransform.forward; // plane's normal

            // corners of a plane on distance from center = planeSize
            Vector3 right = planeTransform.right * planeSize;
            Vector3 up = planeTransform.up * planeSize;
            Vector3 p1 = center + right + up;
            Vector3 p2 = center + right - up;
            Vector3 p3 = center - right - up;
            Vector3 p4 = center - right + up;

            Debug.DrawLine(p1, p2, Color.green);
            Debug.DrawLine(p2, p3, Color.green);
            Debug.DrawLine(p3, p4, Color.green);
            Debug.DrawLine(p4, p1, Color.green);

            // drawing normal
            Debug.DrawLine(center, center + normal * planeSize, Color.red);
        }
    }

}
