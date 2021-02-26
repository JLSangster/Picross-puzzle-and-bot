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
    //flag for if puzzle is complete, might need to be changed to public, haven't decided what happens when its done yet.
    private bool complete;
    //int counting the current correct cell number
    private int correctCount;
    //dummy var, gets changed once i've figured out how to do the actual puzzle bit
    private int completeCount;

    // Start is called before the first frame update
    void Start()
    {
        fill = (fillToggle.GetComponent<Toggle>().isOn);
        completeCount = 17;
        correctCount = 0;
        GenGrid();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool GetFill()
    {
        return (fill);
    }

    //Change fill bool when toggled
    public void TogClick(bool tog)
    {
        fill = tog;
    }

    public void AddCorrectCell()
    {
        correctCount += 1;
        Debug.Log(correctCount);
        //check if that makes it the same as the complete count
        if (correctCount == completeCount) { PuzzleComplete(); }
    }

    //Might not stay public
    public void PuzzleComplete()
    {
        Debug.Log("Puzzle complete!");
        //Do Something once the puzzle is complete.
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

