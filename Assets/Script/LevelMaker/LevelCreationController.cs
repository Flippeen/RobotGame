using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.Universal;
using UnityEditor;

public enum levelMode { playMode, editMode}
public enum editMode { placeMode, moveMode}

[ExecuteInEditMode]
public class LevelCreationController : MonoBehaviour
{
    [SerializeField] GameObject selectedObject;
    [SerializeField] Camera levelUICamera;

    public Transform objectToMove;
    GameHUD gameHUD;
    Vector2 firstPress;
    levelMode levelMode;
    editMode editMode;
    void Start()
    {
        LoadMap();
        var cameraData = Camera.main.GetUniversalAdditionalCameraData();
        cameraData.cameraStack.Add(levelUICamera);
        gameHUD = FindObjectOfType<GameHUD>().GetGameHUD();
    }

    void Update()
    {
        if (levelMode == levelMode.playMode)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            firstPress = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 secondPress = Input.mousePosition;

            float diff = (firstPress - secondPress).magnitude;

            if (diff > 4)
                return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100) && editMode == editMode.placeMode)
            {
                if (EventSystem.current.IsPointerOverGameObject() || IsPointerOverUIObject() || (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Began))
                    return;

                Vector3Int placementPosition = new Vector3Int((int)(hit.point.x + 0.5f) + (int)hit.normal.normalized.x, (int)(hit.point.y + 0.5f) + (int)hit.normal.normalized.y, (int)(hit.point.z + 0.5f) + (int)hit.normal.normalized.z);
                if(GridManager.Instance.GridPosIsNull(placementPosition))
                    Instantiate(CatalogueManager.Instance.GetSelectedGameobject(), placementPosition, Quaternion.identity);
            }
            else if(Physics.Raycast(ray, out hit, 100) && editMode == editMode.moveMode)
            {
                objectToMove = hit.transform;
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            firstPress = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(1))
        {
            Vector2 secondPress = Input.mousePosition;

            float diff = (firstPress - secondPress).magnitude;

            if (diff > 4)
                return;

            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100))
            {
                if (EventSystem.current.IsPointerOverGameObject() || IsPointerOverUIObject() || (Input.touchCount > 0 && Input.GetTouch(0).phase != TouchPhase.Began))
                    return;

                Vector3Int placementPosition = new Vector3Int(Mathf.RoundToInt(hit.point.x), Mathf.RoundToInt(hit.point.y), Mathf.RoundToInt(hit.point.z));

                if (!GridManager.Instance.GridPosIsNull(placementPosition))
                    GridManager.Instance.DeleteGridObject(placementPosition);
            }
        }
    }

        Vector3Int biggestPosition;
    void LoadMap()
    {
        SaveableLevel levelToLoad = JsonUtility.FromJson<SaveableLevel>(File.ReadAllText(Application.dataPath + "/level.JSON"));

        foreach (var gridObj in levelToLoad.savedObjects)
        {
            if (biggestPosition.x < gridObj.position[0])
                biggestPosition.x = gridObj.position[0];

            if (biggestPosition.y < gridObj.position[1])
                biggestPosition.y = gridObj.position[1];

            if (biggestPosition.z < gridObj.position[2])
                biggestPosition.z = gridObj.position[2];
        }

        biggestPosition += Vector3Int.one * 2;
        FindObjectOfType<GridManager>().SetGridSize(biggestPosition);

        foreach (var gridObj in levelToLoad.savedObjects)
        {
            CatalogueItem objToSpawn = CatalogueManager.Instance.GetCatalogueItemFromID(gridObj.objectID);
            GameObject spawnedObj = Instantiate(objToSpawn.catalogueObject, Vector3Int.zero, Quaternion.Euler(0, gridObj.rotation, 0));
            spawnedObj.transform.parent = transform;
            spawnedObj.GetComponent<GridObject>().SetPosition(new Vector3Int(gridObj.position[0], gridObj.position[1], gridObj.position[2]));
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
    public void ChangeGameHUDVisibility(bool show)
    {
        if (show)
            levelMode = levelMode.playMode;
        else
            levelMode = levelMode.editMode;
        gameHUD.ChangeHUDVisibility(show);
    }
    public void SetToPlaceMode(bool isPlaceMode)
    {
        if (isPlaceMode)
            editMode = editMode.placeMode;
        else
            editMode = editMode.moveMode;
    }
}
