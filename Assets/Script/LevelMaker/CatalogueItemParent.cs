using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatalogueItemParent : MonoBehaviour
{
    public CatalogueEnums objectID;
    void FixedUpdate()
    {
        transform.GetChild(0).Rotate(Vector3.up, Space.World);
    }

    public void ObjectClicked()
    {
        print(objectID);
        CatalogueManager.Instance.SetSelectedObject(objectID);
    }
}
