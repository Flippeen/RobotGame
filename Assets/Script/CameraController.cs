using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CameraController : MonoBehaviour
{
    Vector3Int gridSize;
    Vector3 centerPoint;
    [SerializeField] float minDistance, maxDistance;
    [SerializeField] float zoomSpeed;
    [SerializeField] int panSpeed;
    List<Canvas> robotUIs = new List<Canvas>();
    bool canZoom;
    private void Awake()
    {
        SetPlayersUI();
    }
    void Start()
    {
        gridSize = GridManager.Instance.GetGridSize();
        centerPoint = (gridSize-Vector3.one)/2;
        transform.position = centerPoint + (Vector3.left * ((float)gridSize.x));
        transform.forward = centerPoint - transform.position;
        maxDistance = gridSize.magnitude;
        canZoom = true;
    }
    void Update()
    {
        if (!robotUIs.TrueForAll(item => !item.gameObject.activeSelf) || GameHUD.Instance.IsPausMenuOpen() || IsPointerOverUIObject())
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
        }
        if(Input.touchCount == 1)
        {
            transform.RotateAround(centerPoint, Vector3.up, Input.GetTouch(0).deltaPosition.x * Time.deltaTime * panSpeed);
            transform.RotateAround(centerPoint, transform.right, -Input.GetTouch(0).deltaPosition.y * Time.deltaTime * panSpeed);
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
            ZoomCamera(diff * Time.deltaTime);
        }
    }

    void ZoomCamera(float increment)
    {
        if (!canZoom)
            return;
        if (increment < 0 && (centerPoint - transform.position).magnitude > maxDistance)
            return;
        if (increment > 0 && (centerPoint - transform.position).magnitude < minDistance)
            return;
        Vector3 cameraMovement = (centerPoint - transform.position).normalized * increment;
        transform.position += Vector3.ClampMagnitude(cameraMovement, zoomSpeed);
    }

    void SetPlayersUI()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            robotUIs.Add(player.GetComponentInChildren<Canvas>());
        }
    }
    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        return results.Count > 0;
    }
}
