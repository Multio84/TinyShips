using System.Collections;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;



public class CameraController : MonoBehaviour
{
    // debug params
    Transform planeTransform;
    float planeSize = 10f;

    // objects
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    Camera _mainCamera;
    //public GameObject testPlaceObject;
    
    // transform params
    Plane _cameraPlane;
    Vector3 _cameraNormal;
    // public float movementSpeed = 10.0f; // Скорость движения камеры



    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (Input.GetMouseButtonUp(0)) {
            //Place();
            MoveToClick();
        }

        // if (Input.GetKey(KeyCode.Space)) {
        //     SetPos(GetPosByFieldPos(testPos));
        // }
    }
    
    void Initialize()
    {
        if (_cameraHolder is null || _cameraObject is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }

        _mainCamera = _cameraObject.GetComponent<Camera>();

        Vector3 rotationNormal = _cameraHolder.transform.rotation.eulerAngles;
        Vector3 pointOnPlane = _cameraHolder.transform.position;

        _cameraPlane = new Plane(rotationNormal, pointOnPlane);
        _cameraNormal = -_cameraPlane.normal;
    }

    // get position on gamefield under cursor
    Vector3 GetFieldPosByClick()
    {
        // create a ray from camera along cursor's direction
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit)) {
            if (hit.collider.gameObject.CompareTag("GameField")) {
                var fieldPos = hit.point;
                return fieldPos;
            }
            Debug.LogError("No intersection with object, tagged as 'GameField'.");
            return Vector3.zero;
        }
        Debug.LogError("No intersection with any object at all.");
        return Vector3.zero;
    }

    Vector3 GetPosByFieldPos(Vector3 fieldPos)
    {
        //_cameraNormal = _cameraHolder.transform.forward;
        Ray rayFromGameField = new Ray(fieldPos, -_cameraNormal);
        Debug.DrawRay(rayFromGameField.origin, rayFromGameField.direction * 100, Color.red);

        Vector3 intersection;
        
        if (_cameraPlane.Raycast(rayFromGameField, out float distanceToIntersection)) {
            intersection = rayFromGameField.GetPoint(distanceToIntersection);
        }
        else {
            Debug.LogError("No intersection with camera plane.");
            return Vector3.zero;
        }

        return intersection;
    }

    void SetPos(Vector3 pos)
    {   
        _cameraObject.transform.position = pos;
    }




    float speed = 0.5f;
    
    void MoveToClick()
    {
        Vector3 currentLocalPos = _cameraObject.transform.localPosition;

        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 targetPos = GetPosByFieldPos(fieldClickedPos);
        Vector3 targetLocalPos = _cameraHolder.transform.InverseTransformPoint(targetPos);
        
        _cameraObject.transform.localPosition = targetLocalPos;
        //StartCoroutine(AnimatePos(currentLocalPos, targetLocalPos));
    }

    IEnumerator AnimatePos(Vector3 startPos, Vector3 targetPos)
    {
        // Рассчитываем расстояние между позиций
        float distance = Vector3.Distance(startPos, targetPos);

        // Рассчитываем время, необходимое для перемещения между позициями с заданной скоростью
        float duration = distance / speed;

        float elapsedTime = 0f;

        // В цикле постепенно перемещаем объект
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            _cameraObject.transform.localPosition = Vector3.Lerp(startPos, targetPos, t);

            yield return null; // Ждём один кадр
        }

        // Гарантируем установку конечной позиции
        _cameraObject.transform.localPosition = targetPos;
    }
    

    // void Place() 
    // {
    //     Vector3 fieldClickedPos = GetFieldPosByClick();
    //     Vector3 newCameraPos = GetPosByFieldPos(fieldClickedPos);
    //     SetPos(newCameraPos);
    // }






    // debug method to draw a plane
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
