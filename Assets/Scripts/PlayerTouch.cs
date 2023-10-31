using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTouch : MonoBehaviour
{
    public delegate void OnBallPressed(Transform transform,GameObject gameObject);
    public static event OnBallPressed onBallPressed;

    private Camera mainCamera;
    private bool touchStatus=true;

    private void Awake()
    {
        mainCamera = Camera.main;
        InputManager.onPlayerPressed += ObjectTouched;
        GameManager.onLabel += TouchStatus;
    }
    private void OnDestroy()
    {
        InputManager.onPlayerPressed -= ObjectTouched;
        GameManager.onLabel -= TouchStatus;
    }

    private void ObjectTouched(Vector2 touchPosition)
    {
        if (touchStatus)
        {
            Ray ray = mainCamera.ScreenPointToRay(touchPosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider != null)
                {
                    if ((hit.collider.CompareTag("Ball")))
                    {
                        onBallPressed?.Invoke(hit.collider.transform, hit.collider.gameObject);
                    }
                }
            }
        }
    }
    private void TouchStatus(bool status)
    {
        touchStatus = status;
    }
}
