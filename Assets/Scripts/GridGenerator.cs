using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GridGenerator : MonoBehaviour
{
    [Header("Generator Fields")]
    public bool findDistance = false;
    public int rows = 10;
    public int columns = 10;
    public float scale = 1;
    public GameObject gridPrefab;
    public Vector3 startPos = Vector3.zero;
    public GameObject[,] gridArray;
    public int startX = 0;
    public int startY = 0;
    public int endX = 2;
    public int endY = 2;
    public List<GameObject> path = new List<GameObject>();


    void Start()
    {
        gridArray = new GameObject[columns,rows];
        GenerateGrid();
        InitialSetUp();
    }

    private void Update()
    {
        if(findDistance)
        {
            SetDistance();
            SetPath();
            findDistance = false;
        }
    }

    void GenerateGrid()
    {
        for (int i = 0; i < columns; i++)
        {
            for(int j = 0; j < rows; j++)
            {
                GameObject _obj = Instantiate(gridPrefab,new Vector3(startPos.x + (scale * i),startPos.y,startPos.z + (scale * j)),Quaternion.identity);
                _obj.transform.SetParent(this.transform);
                _obj.GetComponent<Block>().xPos = i;
                _obj.GetComponent<Block>().yPos = j;
                gridArray[i,j] = _obj;
            }
        }
    }

    void InitialSetUp()
    {
        foreach (GameObject _block in gridArray)
        {
            _block.GetComponent<Block>().reached = -1;
        }

        gridArray[startX,startY].GetComponent<Block>().reached = 0;
    }

    void SetDistance()
    {
        InitialSetUp();
        int x = startX;
        int y = startY;
        int[] testArray = new int[rows * columns];
        for (int step = 0;step < columns * rows;step++)
        {
            foreach (GameObject _obj in gridArray)
            {
                if(_obj && _obj.GetComponent<Block>().reached == step-1)
                {
                    TestFourDirections(_obj.GetComponent<Block>().xPos, _obj.GetComponent<Block>().yPos, step);
                }
            }
        }
    }

    void SetPath()
    {
        int step;
        int x = endX;
        int y = endY;
        List<GameObject> tempList = new List<GameObject>();
        path.Clear();
        if (gridArray[endX, endY] && gridArray[endX, endY].GetComponent<Block>().reached > 0)
        {
            path.Add(gridArray[x, y]);
            step = gridArray[x, y].GetComponent<Block>().reached - 1;
        }
        else
        {
            Debug.Log("Cant reach the desired location");
            return;
        }

        for (int i = step; step > -1; step--)
        {
            if((TestDirection(x,y,step,1)))
                tempList.Add(gridArray[x, y+1]);
            if ((TestDirection(x, y, step, 2)))
                tempList.Add(gridArray[x+1, y]);
            if ((TestDirection(x, y, step, 3)))
                tempList.Add(gridArray[x, y - 1]);
            if ((TestDirection(x, y, step, 4)))
                tempList.Add(gridArray[x-1, y]);

            GameObject tempObj = FindClosest(gridArray[endX, endY].transform, tempList);
            path.Add(tempObj);
            x = tempObj.GetComponent<Block>().xPos;
            y = tempObj.GetComponent<Block>().yPos;
            tempList.Clear();
        }

    }

    void TestFourDirections(int x, int y, int step)
    {
        if(TestDirection(x,y,-1,1))
            SetReached(x,y+1,step);
        if(TestDirection(x,y,-1,2))
            SetReached(x+1,y,step);
        if(TestDirection(x,y,-1,3))
            SetReached(x,y-1,step);
        if(TestDirection(x,y,-1,4))
            SetReached(x-1,y,step);
    }

    bool TestDirection(int x,int y, int step,int direction)
    {
        switch (direction)
        {
            case 4:
                if (x - 1 > -1 && gridArray[x - 1, y] && gridArray[x - 1, y].GetComponent<Block>().reached == step)
                    return true;
                else
                    return false;
            case 3:
                if (y - 1 > -1 && gridArray[x, y - 1] && gridArray[x, y - 1].GetComponent<Block>().reached == step)
                    return true;
                else
                    return false;
            case 2:
                if (x + 1 < columns && gridArray[x + 1, y] && gridArray[x + 1, y].GetComponent<Block>().reached == step)
                    return true;
                else
                    return false;
            case 1:
                if(y + 1 < rows && gridArray[x,y+1] && gridArray[x,y+1].GetComponent<Block>().reached == step)
                    return true;
                else
                    return false;
        }
        return false;
    }

    void SetReached(int x,int y,int step)
    {
        if (gridArray[x,y])
            gridArray[x,y].GetComponent<Block>().reached = step;
    }

    GameObject FindClosest(Transform targetLocation,List<GameObject> list)
    {
        float currentDistance = scale * rows * columns;
        int indexNumber = 0;
        for (int i = 0; i < list.Count; i++)
        {
            if (Vector3.Distance(targetLocation.position, list[i].transform.position) < currentDistance)
            {
                currentDistance = Vector3.Distance(targetLocation.position, list[i].transform.position);
                indexNumber = i;
            }
        }
        return list[indexNumber];
    }
}
