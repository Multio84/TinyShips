using System.Collections;
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
    WorldGenerator _mapGenerator;
    Coroutine _currentMoveCoroutine;

    // objects
    public Transform _cameraRoot;
    public Transform _cameraHolder;
    public GameObject _cameraObject;
    public Camera _mainCamera;
    
    // transform params
    Plane _cameraHolderPlane;   // plane in which cameraHolder is moving
    Vector3 _cameraNormal;      // normal for getting the camera direction
    float _moveAnimDuration = 0.9f;    // time in seconds for camera to move to clicked pos
    Vector3[] _gamefieldLocalCorners;

    // zoom & distance
    const int _farClipPlane = 1000;
    const float DefaultZoom = 5f;
    const float MinZoom = 4f;
    const float MaxZoom = 14f;
    float _currentZoom;

    // drag
    bool _isDragging = false;
    Vector2 _prevoiusMousePos;
    Vector2 _currentMousePos;
    Vector3 _previousCameraHolderPos;
    const float MouseDragSensitivity = 0.03f;



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

        Initialize();
    }

    public void Initialize()
    {
        _mapGenerator = GameManager.Instance.WorldGenerator;

        // camera transform
        if (_cameraHolder is null || _cameraObject is null) {
            Debug.LogError("CameraHolder or Camera isn't assigned!");
            return;
        }
        _mainCamera = _cameraObject.GetComponent<Camera>();
        _mainCamera.farClipPlane = _farClipPlane;

        _cameraNormal = _cameraObject.transform.forward;
        _cameraHolderPlane = new Plane(-_cameraHolder.up, _cameraHolder.transform.position);

        _gamefieldLocalCorners = GetCameraLocalBoundaryCorners();

        // camera zoom
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

    // get cameraHolder extreme local positions to prevent moving out of gamefield
    Vector3[] GetCameraLocalBoundaryCorners()
    {
        Vector3[] terrainCorners = _mapGenerator.GetBoundaryCorners();
        Vector3[] localCameraHolderCorners = new Vector3[2];

        for (int i = 0; i < terrainCorners.Length; i++)
        {
            Vector3 cameraHolderCorner = GetCameraHolderPosByFieldPos(terrainCorners[i]);
            localCameraHolderCorners[i] = _cameraHolder.InverseTransformPoint(cameraHolderCorner);
        }

        return localCameraHolderCorners;
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
            Debug.LogError("No camera ray intersection with gamefield plane.");
            return Vector3.zero;
        }
        Debug.LogError("No camera ray intersection with anything.");
        return Vector3.zero;
    }

    Vector3 GetCameraHolderPosByFieldPos(Vector3 fieldPos)
    {
        Ray rayFromGameField = new Ray(fieldPos, -_cameraNormal);

        Debug.DrawRay(rayFromGameField.origin, rayFromGameField.direction * 100, Color.red);

        Vector3 intersection;

        if (_cameraHolderPlane.Raycast(rayFromGameField, out float distanceToIntersection))
        {
            intersection = rayFromGameField.GetPoint(distanceToIntersection);
        }
        else
        {
            Debug.LogError("No intersection with camera plane for ray from fieldPos.");
            return Vector3.zero;
        }

        Debug.DrawLine(fieldPos, intersection);

        return intersection;
    }

    // method to debug camera pos
    void PlaceByClick() 
    {
        Vector3 fieldClickedPos = GetFieldPosByClick();
        Vector3 newCameraHolderPos = GetCameraHolderPosByFieldPos(fieldClickedPos);

        _cameraHolder.transform.position = newCameraHolderPos;
    }

    public void MoveByClick()
    {
        Vector3 startpos = _cameraHolder.transform.position;
        Vector3 targetpos = GetCameraHolderPosByFieldPos(GetFieldPosByClick());

        if (_currentMoveCoroutine is not null)
        {
            StopCoroutine(_currentMoveCoroutine);
        }
        _currentMoveCoroutine = StartCoroutine(AnimatePosition(startpos, targetpos));
    }

    IEnumerator AnimatePosition(Vector3 startPos, Vector3 targetPos)
    {
        float elapsedTime = 0;

        while (elapsedTime < _moveAnimDuration) {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / _moveAnimDuration;      // time normalized from 0 to 1
            float easedT = Mathf.Sin(t * Mathf.PI / 2);     // easing function
            _cameraHolder.transform.position = Vector3.Lerp(startPos, targetPos, easedT);

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
        _previousCameraHolderPos = _cameraHolder.transform.localPosition;

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
                Vector3 deltaCameraHolderPos = new Vector3(deltaMousePos.x, 0, deltaMousePos.y);
                Vector3 newCameraHolderPos = _previousCameraHolderPos - deltaCameraHolderPos * MouseDragSensitivity;
                
                _cameraHolder.transform.localPosition = LimitCameraHolderPosByField(newCameraHolderPos);
                Debug.Log($"Holder current pos: {_cameraHolder.transform.localPosition}");

                _prevoiusMousePos = _currentMousePos;   // update previous mouse pos as current
                _previousCameraHolderPos = _cameraHolder.transform.localPosition;
            }
        }
        else {
            _prevoiusMousePos = context.ReadValue<Vector2>();
        }
    }

    Vector3 LimitCameraHolderPosByField(Vector3 pos)
    {
        Vector3 newPos = new Vector3() {
            x = Mathf.Clamp(pos.x, _gamefieldLocalCorners[0].x, _gamefieldLocalCorners[1].x),
            z = Mathf.Clamp(pos.z, _gamefieldLocalCorners[0].z, _gamefieldLocalCorners[1].z)
        };

        return newPos;
    }
}
