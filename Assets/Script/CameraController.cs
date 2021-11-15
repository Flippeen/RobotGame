using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Vector3Int gridSize;
    Vector3 centerPoint;
    [SerializeField] float minDistance, maxDistance;
    [SerializeField] int panSpeed;
    public List<Canvas> robotUIs = new List<Canvas>();
    private void Awake()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            robotUIs.Add(player.GetComponentInChildren<Canvas>());
        }
    }
    void Start()
    {
        gridSize = GridManager.Instance.GetGridSize();
        centerPoint = (gridSize-Vector3.one)/2;
        transform.position = centerPoint + (Vector3.left * ((float)gridSize.x));
        transform.forward = centerPoint - transform.position;
        maxDistance = gridSize.magnitude;
    }
    void Update()
    {
        if (!robotUIs.TrueForAll(item => !item.gameObject.activeSelf))
            return;

        if (Input.GetMouseButton(0) && SystemInfo.deviceType == DeviceType.Desktop)
        {
            transform.RotateAround(centerPoint, Vector3.up, Input.GetAxis("Mouse X"));
            transform.RotateAround(centerPoint, transform.right, -Input.GetAxis("Mouse Y"));
        }
        if(Input.GetMouseButton(1) && SystemInfo.deviceType == DeviceType.Desktop)
        {
            float yValue = Input.GetAxis("Mouse Y");
            ZoomCamera(yValue);
            //if (yValue < 0 && (centerPoint - transform.position).magnitude > maxDistance)
            //    return;
            //if (yValue > 0 && (centerPoint - transform.position).magnitude < minDistance)
            //    return;

            //transform.position += (centerPoint - transform.position).normalized * yValue;
        }
        if(Input.touchCount == 1)
        {
            print("One touch");

            transform.RotateAround(centerPoint, Vector3.up, Input.GetTouch(0).deltaPosition.x * Time.fixedDeltaTime * panSpeed);
            transform.RotateAround(centerPoint, transform.right, -Input.GetTouch(0).deltaPosition.y * Time.fixedDeltaTime * panSpeed);
        }
        if(Input.touchCount == 2)
        {
            Touch firstFinger = Input.GetTouch(0);
            Touch secondFinger = Input.GetTouch(1);

            Vector2 touchFirstPrevPos = firstFinger.position - firstFinger.deltaPosition;
            Vector2 touchSecondPrevPos = secondFinger.position - secondFinger.deltaPosition;

            float prevMag = (touchFirstPrevPos - touchSecondPrevPos).magnitude;
            float currMag = (firstFinger.position - secondFinger.position).magnitude;

            float diff = currMag - prevMag;
            diff = Mathf.Clamp(diff, -50, 50);
            print(diff);
            ZoomCamera(diff * Time.fixedDeltaTime);
            //if (diff > 0.1f)
            //{
            //    print("Two touch" + diff);
            //    if (diff > 0 && (centerPoint - transform.position).magnitude < maxDistance)
            //    {
            //        transform.position += (centerPoint - transform.position).normalized * diff;
            //    }
            //    if(diff < 0 && (centerPoint - transform.position).magnitude > minDistance)
            //    {
            //        transform.position += (centerPoint - transform.position).normalized * diff;
            //    }
            //}

        }
    }

    void ZoomCamera(float increment)
    {
        if (increment < 0 && (centerPoint - transform.position).magnitude > maxDistance)
            return;
        if (increment > 0 && (centerPoint - transform.position).magnitude < minDistance)
            return;

        transform.position += (centerPoint - transform.position).normalized * increment;
    }
}
