using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    //these should change to be set elsewhere sometime
    private int rows = 5;
    private int cols = 5;
    private float tileSize = 1;
    private bool[] corrects = { true, true, true, true, true, false, true, true, true, false, false, false, true, false, false, false, true, true, true, false, true, true, true, true, true };

    // Start is called before the first frame update
    void Start()
    {
        genGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void genGrid()
    {
        GameObject cellRef = (GameObject)Instantiate(Resources.Load("Cell"));
        //this will be replaced at some point with loading in the list from elsewhere probably.
        

        int i = 0;
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = (GameObject)Instantiate(cellRef, transform);
                CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
                cellBehaviour.correct = corrects[i];

                float posX, posY;
                posX = c * tileSize;
                posY = r * tileSize;

                cell.transform.position = new Vector2(posX, posY);
                i++;
            }
        }

        Destroy(cellRef);
        Debug.Log("loaded");
    }
}
