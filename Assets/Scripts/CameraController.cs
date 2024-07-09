using UnityEngine;



public class CameraController : MonoBehaviour
{
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    public Camera _mainCamera;
    public GameObject testPlaceObject;
    
    Vector3 _cameraNormal;
    Plane _cameraPlane;
    public float movementSpeed = 10.0f; // Скорость движения камеры

    Vector3 testPos = new Vector3(10f, 0f, 7.5f);


    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (Input.GetMouseButton(0)) {
            SetPos(GetPosByFieldPos(GetFieldPosByCursor()));
            //SetCameraPos(GetCameraPosByFieldPos(testPos));
        }

        if (Input.GetKey(KeyCode.Space)) {
            SetPos(GetPosByFieldPos(testPos));
        }
    }

    void Initialize()
    {
        if (_cameraHolder is null || _cameraObject is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }

        _cameraPlane = GetCameraPlane();
        _cameraNormal = _cameraHolder.transform.forward;
    }

    Plane GetCameraPlane()
    {
        Vector3 rotationNormal = _cameraHolder.transform.rotation.eulerAngles;
        Vector3 pointOnPlane = _cameraHolder.transform.position;

        Plane plane = new Plane(rotationNormal, pointOnPlane);

        return plane;
    }

    // get position on gamefield under cursor
    Vector3 GetFieldPosByCursor()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition); // Создаем луч из камеры в направлении курсора мыши
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) // Проверяем пересечение луча с объектами сцены
        {
            if (hit.collider.gameObject.CompareTag("GameField")) // Проверяем, что попадание происходит на игровое поле
            {
                return hit.point;
            }
            return Vector3.zero;
        }
        return Vector3.zero;
    }

    Vector3 GetPosByFieldPos(Vector3 fieldPos)
    {
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
        var localPos = _cameraObject.transform.InverseTransformPoint(pos);
        _cameraObject.transform.localPosition = new Vector3(localPos.x, localPos.y, 0);   // camera sholdn't move along the local Z
    }













    
    Vector3 GetCameraRayXZIntersection(Vector3 cameraPos)
    {
        // XZ plane to catch rays from camera
        Plane plane = new Plane(Vector3.up, Vector3.zero);

        // cast a ray from camera in camera's Forward direction
        Ray ray = new Ray(cameraPos, _cameraObject.transform.forward);
        Debug.DrawRay(ray.origin, ray.direction * 100, Color.red);

        Vector3 intersection;

        if (plane.Raycast(ray, out float distanceToIntersection)) {
            intersection = ray.GetPoint(distanceToIntersection);
        }
        else {
            Debug.LogError("Нет пересечения с плоскостью Y = 0.");
            return Vector3.zero;
        }

        return intersection;
    }






}
