using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Generator Fields")]
    public int rows = 10;
    public int columns = 10;
    public float scale = 1;
    public GameObject gridPrefab;
    public Vector3 startPos = Vector3.zero;

    void Start()
    {
        GenerateGrid();
    }

    void GenerateGrid()
    {
        for (int i = 0; i < rows; i++)
        {
            for(int j = 0; j < columns; j++)
            {
                GameObject _obj = Instantiate(gridPrefab,new Vector3(startPos.x + (scale * i),startPos.y,startPos.z + (scale * j)),Quaternion.identity);
                _obj.transform.SetParent(this.transform);
                _obj.GetComponent<Block>().xPos = i;
                _obj.GetComponent<Block>().yPos = j;
            }
        }
    }
}
