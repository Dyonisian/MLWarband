using UnityEngine;
using System.Collections;

[System.Serializable]
public class ArrayLayout
{

    [System.Serializable]
    public struct rowData
    {
        public float[] row;
    }

    public rowData[] rows = new rowData[26]; //Grid of 7x7
}
