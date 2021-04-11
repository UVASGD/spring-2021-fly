using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Goal : MonoBehaviour
{
    public RectTransform uiTransform;
    public Transform target;
    public Camera cam;

    Vector3 toPosition;
    Vector3 fromPosition;
    Vector3 direction;
    float angle;

    private void Awake()
    {
        
    }

    private void Update()
    {
        fromPosition = transform.position;
        toPosition = target.position;
        print(target.position);
        direction = Vector3.ProjectOnPlane(fromPosition - toPosition, cam.transform.forward).normalized;
        angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        print(angle);
        uiTransform.localEulerAngles = Vector3.forward * angle;

        Vector3 targetPositionScreenPoint = cam.WorldToScreenPoint(toPosition);
        bool isOffScreen = targetPositionScreenPoint.x <= 0 ||
                            targetPositionScreenPoint.x >= Screen.width ||
                            targetPositionScreenPoint.y <= 0 || 
                            targetPositionScreenPoint.y >= Screen.height;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(fromPosition, 1f);
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(toPosition, 1f);
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(fromPosition, toPosition);
    }
}
