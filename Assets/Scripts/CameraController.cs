using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;


public interface ICameraInputHandler
{
    public void MoveByClick();
    public void Zoom(InputAction.CallbackContext context);
    public void DragEnable(InputAction.CallbackContext context);
    public void Drag(InputAction.CallbackContext context);
}



public class CameraController : MonoBehaviour, ICameraInputHandler
{
    InputController _inputController;
    [SerializeField] MapGenerator _mapGenerator;
    Coroutine _currentMoveCoroutine;

    // objects
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    public Camera _mainCamera;
    
    // transform params
    Plane _cameraPlane;
    Vector3 _cameraNormal;
    float duration = 1f;    // time in seconds for camera to move to clicked pos

    // zoom
    const float DefaultZoom = 5f;
    const float MinZoom = 2f;
    const float MaxZoom = 10f;
    float _currentZoom;

    // drag
    bool _isDragging = false;
    Vector2 _prevoiusMousePos;
    Vector2 _currentMousePos;
    Vector3 _previousCameraPos;
    const float MouseDragSensitivity = 0.05f;


    void Start()
    {   
        // subcscribe to InputController
        _inputController = FindObjectOfType<InputController>();
        if (_inputController is not null) {
            _inputController.SubscribeCameraHandler(this);
        }
        else {
            Debug.LogError("InputController is not found!");
            return;
        }

        // get camera properties for controlling
        if (_cameraHolder is null || _cameraObject is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }
        _mainCamera = _cameraObject.GetComponent<Camera>();

        Vector3 cameraRotationNormal = _cameraHolder.transform.rotation.eulerAngles;
        Vector3 cameraPointOnPlane = _cameraHolder.transform.position;

        _cameraPlane = new Plane(cameraRotationNormal, cameraPointOnPlane);
        _cameraNormal = -_cameraPlane.normal;

        // set camera properties
        _currentZoom = DefaultZoom;
    }
    
    #region - OnDisable -
    void OnDisable()
    {
        // unsubscribe from InputController
        if (_inputController is not null) {
            _inputController.UnsubscribeCameraHandler(this);
        }
        else {
            Debug.LogError("InputController not found!");
            return;
        }
    }
    #endregion

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
            Debug.LogError("No camera ray intersection with gamefield plane.");
            return Vector3.zero;
        }
        Debug.LogError("No camera ray intersection with anything.");
        return Vector3.zero;
    }

    // get camera world position by given game field position
    Vector3 GetLocalPosByFieldPos(Vector3 fieldPos)
    {
        _cameraNormal = _cameraHolder.transform.forward;
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

        Vector3 intersectionLocalCoords = _cameraHolder.transform.InverseTransformPoint(intersection);
        intersectionLocalCoords.z = 0;

        return intersectionLocalCoords;
    }
    
    void PlaceByClick() 
    {
        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 newLocalPos = GetLocalPosByFieldPos(fieldClickedPos);

        _cameraObject.transform.localPosition = newLocalPos;
    }

    public void MoveByClick()
    {
        Vector3 startLocalPos = _cameraObject.transform.localPosition;

        Vector3 fieldClickedPos = GetFieldPosByClick();
        //Vector3 targetWorldPos = GetLocalPosByFieldPos(fieldClickedPos);
        //Vector3 targetLocalPos = _cameraHolder.transform.InverseTransformPoint(targetWorldPos);
        //targetLocalPos.z = 0;

        Vector3 targetLocalPos = GetLocalPosByFieldPos(fieldClickedPos);
    
        if (_currentMoveCoroutine != null) {
            StopCoroutine(_currentMoveCoroutine);
        }
        _currentMoveCoroutine = StartCoroutine( AnimateLocalPos(startLocalPos, targetLocalPos) );
    }

    IEnumerator AnimateLocalPos(Vector3 startPos, Vector3 targetPos)
    {
        float t = 0;
        float elapsedTime = 0;

        while (elapsedTime < duration) {
            elapsedTime += Time.deltaTime;
            t = elapsedTime / duration;     // time normalized from 0 to 1
            float easedT = Mathf.Sin(t * Mathf.PI / 2); // easing function
            _cameraObject.transform.localPosition = Vector3.Lerp(startPos, targetPos, easedT);

            yield return null;
        }
    }



    public void Zoom(InputAction.CallbackContext context)
    {
        _currentZoom = _mainCamera.orthographicSize + Mathf.RoundToInt(context.ReadValue<float>());
        _mainCamera.orthographicSize = Mathf.Clamp(_currentZoom, MinZoom, MaxZoom);
        //Debug.Log($"Changed camera zoom (size) to {_mainCamera.orthographicSize}");
    }
    
    public void DragEnable(InputAction.CallbackContext context)
    {
        _previousCameraPos = _cameraObject.transform.localPosition;

        if (context.phase == InputActionPhase.Performed) {
            _isDragging = true;
        }
        else if (context.phase == InputActionPhase.Canceled) {
            _isDragging = false;
        }
    }

    public void Drag(InputAction.CallbackContext context)
    {   
        if (_isDragging)
        {
            _currentMousePos = context.ReadValue<Vector2>();
            
            // if previous mouse pos is not set yet
            if (_prevoiusMousePos == Vector2.zero) {
                _prevoiusMousePos = _currentMousePos;
            }

            Vector2 deltaMousePos = _currentMousePos - _prevoiusMousePos;

            if (deltaMousePos != Vector2.zero)
            {
                Vector3 deltaCameraPos = new Vector3(deltaMousePos.x, deltaMousePos.y, 0);
                Vector3 newCameraPos = _previousCameraPos - deltaCameraPos * MouseDragSensitivity;

                _cameraObject.transform.localPosition = newCameraPos;

                _prevoiusMousePos = _currentMousePos;   // update previous mouse pos as current
                _previousCameraPos = _cameraObject.transform.localPosition;
            }
        }
        else {
            _prevoiusMousePos = context.ReadValue<Vector2>();
        }
    }





}
