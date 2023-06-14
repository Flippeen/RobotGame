using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GridManager : MonoBehaviour
{
    enum gridState { none, showOutline, showGrid}
    [SerializeField] gridState gridOption;
    static GridManager _instance;
    public static GridManager Instance { get { return _instance; } }

    [SerializeField] Vector3Int gridSize;
    GridObject[,,] grid = new GridObject[0, 0, 0];
    void Awake()
    {
        if (Instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        grid = new GridObject[gridSize.x, gridSize.y, gridSize.z];
        SceneManager.LoadScene("GameScene", LoadSceneMode.Additive);
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            List<SaveableObject> gridObjs = new List<SaveableObject>();
            foreach (var gridObject in grid)
            {
                if (gridObject != null)
                {
                    SaveableObject newObj = new SaveableObject();
                    newObj.position[0] = gridObject.GetGridPosition.x;
                    newObj.position[1] = gridObject.GetGridPosition.y;
                    newObj.position[2] = gridObject.GetGridPosition.z;
                    newObj.rotation = (int)gridObject.transform.eulerAngles.y;
                    newObj.objectID = (int)gridObject.objectID;
                    gridObjs.Add(newObj);
                }
            }
            SaveableLevel newLevel = new SaveableLevel();
            newLevel.savedObjects = new SaveableObject[gridObjs.Count];
            newLevel.savedObjects = gridObjs.ToArray();

            File.WriteAllText(Application.dataPath + "/level.JSON", JsonUtility.ToJson(newLevel, true));
            print(JsonUtility.ToJson(newLevel, true));
        }
    }
    public void SetGridSize(Vector3Int newGridSize)
    {
        gridSize = newGridSize;
        grid = new GridObject[gridSize.x, gridSize.y, gridSize.z];

    }
    public void SetStartPosition(GridObject gO)
    {
        Vector3Int pos = gO.GetGridPosition;
        try{
            if (grid[pos.x, pos.y, pos.z] == null)
                grid[pos.x, pos.y, pos.z] = gO;
        }
        catch (Exception e){
            Debug.Log("Outside Array");
            return;
        }
    }

    public bool MoveObject(Vector3Int startPos, Vector3Int nextPos)
    {
        try{
            if (grid[nextPos.x, nextPos.y, nextPos.z] != null && grid[nextPos.x, nextPos.y, nextPos.z].GetComponent<IInteractable>() != null)
            {
                if(grid[nextPos.x, nextPos.y, nextPos.z].GetComponent<IInteractable>().UseInteractable(startPos))
                    grid[nextPos.x, nextPos.y, nextPos.z] = null;
            }
            if (startPos.y == nextPos.y && grid[nextPos.x, nextPos.y, nextPos.z] != null && grid[nextPos.x, nextPos.y, nextPos.z].GetComponent<MoveObjectController>() != null)
            {
                if(grid[nextPos.x, nextPos.y, nextPos.z].GetComponent<MoveObjectController>().Pushable(startPos))
                    grid[nextPos.x, nextPos.y, nextPos.z] = null;
            }
            if(grid[nextPos.x, nextPos.y, nextPos.z] == null)
            {
                grid[nextPos.x, nextPos.y, nextPos.z] = grid[startPos.x, startPos.y, startPos.z];
                grid[startPos.x, startPos.y, startPos.z] = null;
                if (nextPos.y <= 0)
                    GetGridObject(nextPos).GetComponent<MoveObjectController>().Died();
                return true;
            }
        }
        catch (Exception e){
            Debug.Log("Outside Array! Start Position: " + startPos + " Next Position: " + nextPos);
            return false;
        }
        return false;
    }
    public void DeleteGridObject(Vector3Int objectPos)
    {
        try
        {
            Destroy(GetGridObject(objectPos).gameObject);
            grid[objectPos.x, objectPos.y, objectPos.z] = null;
        }
        catch (Exception e)
        {
            Debug.Log("Can't delete object! Position is: " + objectPos);
        }
    }

    public bool GridPosIsNull(Vector3Int positionToCheck)
    {
        try { return grid[positionToCheck.x, positionToCheck.y, positionToCheck.z] == null; }
        catch(Exception e){
            Debug.Log("Outside Array!");
            return false;
        }
    }
    public bool GridPosIsPushable(Vector3Int positionToCheck)
    {
        try { return grid[positionToCheck.x, positionToCheck.y, positionToCheck.z].GetComponent<MoveObjectController>() != null; }
        catch (Exception e)
        {
            Debug.Log("Outside Array!");
            return false;
        }
    }
    public bool GridPosIsGoal(Vector3Int positionToCheck)
    {
        try { return grid[positionToCheck.x, positionToCheck.y, positionToCheck.z].GetComponent<Goal>() != null; }
        catch (Exception e)
        {
            Debug.Log("Not goal!");
            return false;
        }
    }
    public GridObject GetGridObject(Vector3Int positionToGetFrom)
    {
        try { return grid[positionToGetFrom.x, positionToGetFrom.y, positionToGetFrom.z]; }
        catch (Exception e)
        {
            Debug.Log("No grid object found!");
            return null;
        }
    }

    public Vector3Int GetGridSize()
    {
        return gridSize;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        switch (gridOption)
        {
            case gridState.none:
                break;
            case gridState.showOutline:
                Vector3 cent = new Vector3(gridSize.x -1f, gridSize.y - 1f, gridSize.z - 1f) / 2;
                Gizmos.DrawWireCube(cent, gridSize);
                break;
            case gridState.showGrid:
                for (int x = 0; x < gridSize.x; x++)
                {
                    for (int y = 0; y < gridSize.y; y++)
                    {
                        for (int z = 0; z < gridSize.z; z++)
                        {
                            Vector3 center = new Vector3(x, y, z);
                            Gizmos.DrawWireCube(center, new Vector3(1, 1, 1));
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}
