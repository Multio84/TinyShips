using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    private List<ICameraInputHandler> _cameraSubscribers = new List<ICameraInputHandler>();


    public void SubscribeCameraHandler(ICameraInputHandler handler)
    {
        if (!_cameraSubscribers.Contains(handler)) {
            _cameraSubscribers.Add(handler);
        }
    }

    public void UnsubscribeCameraHandler(ICameraInputHandler handler)
    {
        if (_cameraSubscribers.Contains(handler)) {
            _cameraSubscribers.Remove(handler);
        }
    }



    public void OnMoveByClick(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Performed) {
            foreach (var subscriber in _cameraSubscribers) {
                subscriber.MoveByClick();
            }
        }
    }

    public void OnZoom(InputAction.CallbackContext context)
    {
        if (context.phase == InputActionPhase.Started) {
            foreach (var subscriber in _cameraSubscribers) {
                subscriber.Zoom(context);
            }
        }
    }

}
