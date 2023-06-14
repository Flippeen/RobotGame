using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueManager : MonoBehaviour
{
    [SerializeField] RectTransform contentObj;
    [SerializeField] GameObject parentItem;
    List<CatalogueItem> catalogue = new List<CatalogueItem>();
    List<GameObject> catalogueUIList = new List<GameObject>();
    CatalogueEnums selectedObjectId;

    static CatalogueManager _instance;
    public static CatalogueManager Instance { get { return _instance; } }

    private void Awake()
    {
        if (Instance == null)
            _instance = this;
        else
            Destroy(gameObject);

        foreach (var item in transform.GetComponentsInChildren<CatalogueItem>())
        {
            catalogue.Add(item);
        }
    }
    void Start()
    {
        LoadCatalogueItems();
    }
    void LoadCatalogueItems()
    {
        foreach (var catalogueItem in catalogue)
        {
            GameObject newParentObj = Instantiate(parentItem, Vector3.zero, Quaternion.identity);
            newParentObj.transform.SetParent(contentObj.transform);
            newParentObj.transform.localPosition = Vector3.zero;
            newParentObj.GetComponent<CatalogueItemParent>().objectID = catalogueItem.objectID;
            GameObject newObj = Instantiate(catalogueItem.catalogueObject, Vector3.zero, Quaternion.identity);
            newObj.transform.SetParent(newParentObj.transform.transform);
            newObj.transform.localScale = Vector3.one;
            newObj.transform.localRotation = Quaternion.Euler(-10,0,-10);
            newObj.transform.localPosition = Vector3.zero;
            newObj.layer = 5;
            Destroy(newObj.GetComponent<GridObject>());
            if(newObj.transform.childCount > 1)
            {
                int childCount = newObj.transform.childCount;
                for (int i = childCount; i > 1; i--)
                    Destroy(newObj.transform.GetChild(i-1).gameObject);
            }
            SetLayerRecursively(newObj.transform);
            contentObj.sizeDelta = new Vector2(catalogueUIList.Count * 400, contentObj.sizeDelta.y);
            newParentObj.transform.localPosition = new Vector3((catalogue.Count - catalogueUIList.Count) * 200, contentObj.sizeDelta.y/ 2, 0);
            catalogueUIList.Add(newParentObj);
        }
        contentObj.sizeDelta += new Vector2(800, 0);
    }
    void SetLayerRecursively(Transform obj)
    {
        obj.gameObject.layer = 5;

        for(int i = 0; i < obj.childCount; i++)
        {
            SetLayerRecursively(obj.GetChild(i));
        }
    }
    public void SetSelectedObject(CatalogueEnums objectID)
    {
        selectedObjectId = objectID;
    }
    public GameObject GetSelectedGameobject()
    {
        return catalogue.Find(x => x.objectID == selectedObjectId).catalogueObject;
    }
    public CatalogueItem GetCatalogueItemFromID(int id)
    {
        return catalogue.Find(x => (int)x.objectID == id);
    }

    public int GetIDFromCatalogueItem(GameObject catalogueItem)
    {
        return (int)catalogue.Find(x => x.catalogueObject.gameObject.name.Split(' ')[0] == catalogueItem.gameObject.name.Split(' ')[0]).objectID;
    }

    public List<CatalogueItem> GetCatalogue()
    {
        List<CatalogueItem> newCatalogue = new List<CatalogueItem>();
        newCatalogue.AddRange(catalogue);
        return newCatalogue;
    }
}
