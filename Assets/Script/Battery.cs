using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Battery : MonoBehaviour, IInteractable
{
    [SerializeField] int batteryValue;
    [SerializeField] float bounceMultiplier, bounceSpeedMultiplier, rotationSpeedMultiplier;
    TextMeshProUGUI[] texts;
    void Awake()
    {
        texts = GetComponentsInChildren<TextMeshProUGUI>();
        foreach (var text in texts)
        {
            text.text = batteryValue.ToString();
        }
    }
    void FixedUpdate()
    {
        transform.RotateAround(transform.position, Vector3.up, rotationSpeedMultiplier * Time.fixedDeltaTime);
        transform.position += new Vector3(0, -Mathf.Sin(Time.time * bounceSpeedMultiplier) * 0.01f * bounceMultiplier, 0);
    }

    public bool UseInteractable(Vector3Int objUsing)
    {
        GridManager.Instance.GetGridObject(objUsing).GetComponent<PlayerMovement>().ChangeBatteryValue(batteryValue);
        Destroy(gameObject, 0.2f);
        return true;
    }
}
