using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class SaveableObject
{
    public int[] position = new int[3];
    public int rotation;
    public int objectID;
}
