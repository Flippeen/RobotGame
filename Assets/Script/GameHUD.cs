using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameHUD : MonoBehaviour
{
    [SerializeField] Canvas gameCanvasHUD;
    [SerializeField] Canvas pausCanvas;
    List<Canvas> robotUIs = new List<Canvas>();
    bool showHUD = true;
    static GameHUD _instance;
    public static GameHUD Instance { get { return _instance; } }
    void Awake()
    {
        SetPlayersUI();

        if (Instance == null)
            _instance = this;
        else
            Destroy(gameObject);
    }

    void Update()
    {
        bool isHUDVisible = !pausCanvas.gameObject.activeSelf && robotUIs.TrueForAll(item => !item.gameObject.activeSelf) && showHUD;
        if (!isHUDVisible)
            return;
        gameCanvasHUD.gameObject.SetActive(isHUDVisible);
    }

    public bool IsPausMenuOpen()
    {
        return pausCanvas.gameObject.activeSelf;
    }
    void SetPlayersUI()
    {
        PlayerMovement[] players = FindObjectsOfType<PlayerMovement>();
        foreach (var player in players)
        {
            robotUIs.Add(player.GetComponentInChildren<Canvas>());
        }
    }
    public void ChangeHUDVisibility(bool visibility)
    {
        showHUD = visibility;
        gameCanvasHUD.gameObject.SetActive(visibility);
    }
    public GameHUD GetGameHUD()
    {
        return this;
    }
}
