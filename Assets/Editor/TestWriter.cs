using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnumCatalogue))]
public class TestWriter : Editor
{
    EnumCatalogue myScrip;
    string filePath = "Assets/Script/LevelMaker/";
    string fileName = "CatalogueEnums";

    private void OnEnable()
    {
        myScrip = (EnumCatalogue)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        filePath = EditorGUILayout.TextField("Path", filePath);
        fileName = EditorGUILayout.TextField("Name", fileName);
        if (GUILayout.Button("Save"))
        {
            EdiorMethods.WriteToEnum(filePath, fileName, myScrip.itemName);
        }
    }
}