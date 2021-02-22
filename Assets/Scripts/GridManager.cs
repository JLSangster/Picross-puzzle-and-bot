using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    private int rows = 5;
    private int cols = 5;
    private float tileSize = 1;
    private bool[] corrects = { true, true, true, true, true, false, true, true, true, false, false, false, true, false, false, false, true, true, true, false, true, true, true, true, true };
    public GameObject fillToggle;
    private bool fill;

    // Start is called before the first frame update
    void Start()
    {
        fill = (fillToggle.GetComponent<Toggle>().isOn);
        GenGrid();
        GetFill();
    }

    // Update is called once per frame
    void Update()
    {
      
    }

    public bool GetFill()
    {
        Debug.Log(fill);
        return (fill);
    }

    //Change fill bool when toggled
    public void TogClick(bool tog)
    {
        fill = tog;
    }

    //Generate the grid of given dimensions
    void GenGrid()
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
                //get if correct cell - currently from a dummy list
                cellBehaviour.correct = corrects[i];
                cellBehaviour.gridManager = this;

                float posX, posY;
                posX = c * tileSize;
                posY = r * tileSize;

                cell.transform.position = new Vector2(posX, posY);
                i++;
            }
        }

        Destroy(cellRef);
    }
}

