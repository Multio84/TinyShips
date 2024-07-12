using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // objects
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    public Camera _mainCamera;
    
    // transform params
    Plane _cameraPlane;
    Vector3 _cameraNormal;
    //float _moveSpeed = 2.5f;
    float duration = 1f;    // seconds for camera to move to clicked pos


    void Start()
    {
        Initialize();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) MoveToClick();
    }
    
    void Initialize()
    {
        if (_cameraHolder is null || _cameraObject is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }
        _mainCamera = _cameraObject.GetComponent<Camera>();

        Vector3 cameraRotationNormal = _cameraHolder.transform.rotation.eulerAngles;
        Vector3 cameraPointOnPlane = _cameraHolder.transform.position;

        _cameraPlane = new Plane(cameraRotationNormal, cameraPointOnPlane);
        _cameraNormal = -_cameraPlane.normal;
    }

    // get position on gamefield under cursor
    Vector3 GetFieldPosByClick()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
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

    // get camera world position by given game field position
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
            Debug.LogError("No intersection with camera plane");
            return Vector3.zero;
        }
        return intersection;
    }
    
    // place camera by click on game field
    void PlaceToClick() 
    {
        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 newWorldPos = GetPosByFieldPos(fieldClickedPos);

        _cameraObject.transform.position = newWorldPos;
    }

    void MoveToClick()
    {
        Vector3 startLocalPos = _cameraObject.transform.localPosition;

        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 targetWorldPos = GetPosByFieldPos(fieldClickedPos);
        Vector3 targetLocalPos = _cameraHolder.transform.InverseTransformPoint(targetWorldPos);
        targetLocalPos.z = 0;
    
        StartCoroutine(AnimateLocalPos(startLocalPos, targetLocalPos));
    }

    IEnumerator AnimateLocalPos(Vector3 start, Vector3 target)
    {
        float t = 0;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / duration;     // time normalized from 0 to 1
            float easedT = Mathf.Sin(t * Mathf.PI / 2); // easing function
            _cameraObject.transform.localPosition = Vector3.Lerp(start, target, easedT);

            yield return null;
        }
    }
    
}
