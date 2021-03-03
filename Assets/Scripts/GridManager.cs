using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GridManager : MonoBehaviour
{
    private int rows = 5;
    private int cols = 5;
    private float tileSize = 1;
    public GameObject fillToggle;
    private bool fill;
    //flag for if puzzle is complete, might need to be changed to public, haven't decided what happens when its done yet.
    private bool complete;
    //int counting the current correct cell number
    private int correctCount;
    //dummy var, gets changed once i've figured out how to do the actual puzzle bit
    private int completeCount;
    //solution matrix
    private bool[,] solMat;
    private string[,] clues;

    // Start is called before the first frame update
    void Start()
    {
        fill = (fillToggle.GetComponent<Toggle>().isOn);
        completeCount = 17;
        correctCount = 0;
        GenPuzzle();
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
        GameObject labelRef = (GameObject)Instantiate(Resources.Load("RowLabel"));
        //this will be replaced at some point with loading in the list from elsewhere probably.

        //GameObject label = (GameObject)Instantiate(labelRef, transform);

        for (int r = 0; r < rows; r++)
        {
            //print clue first
            GameObject label = (GameObject)Instantiate(labelRef, transform);
            label.GetComponent<UnityEngine.UI.Text>().text = clues[r, 0].ToString();

            float posX, posY;
            posX = (-cols / 2 - 1) * tileSize;
            posY = (r * tileSize) - (rows / 2 * tileSize);

            label.transform.position = new Vector2(posX, posY);
            
            for (int c = 0; c < cols; c++)
            {
                GameObject cell = (GameObject)Instantiate(cellRef, transform);
                CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
                cellBehaviour.correct = solMat[r,c];
                cellBehaviour.gridManager = this;

                //float posX, posY;
                posX = (c * tileSize) - (cols/2 * tileSize);
                posY = (r * tileSize) - (rows/2 * tileSize);

                cell.transform.position = new Vector2(posX, posY);
            }
        }

        Destroy(cellRef);
        Destroy(labelRef);
    }

    void GenPuzzle()
    {
        //randomize the rows
        rows = 5 * Random.Range(1, 3);
        cols = rows;

        //init the mat
        solMat = new bool[rows, cols];
        clues = new string[rows, 2];

        //populate the mat
        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                solMat[r, c] = Random.Range(0, 2) == 0;
                if (solMat[r, c] == true)
                {
                    completeCount += 1;
                }
            }
        }

        for (int r = 0; r < rows; r++)
        {
            int clueCounter = 0;
            clues[r, 0] = "";
            for (int c = 0; c < cols; c++)
            {
                if (solMat[r, c] == true) { clueCounter += 1; }
                else
                {
                    if (clueCounter != 0)
                    {
                        clues[r, 0] = clues[r, 0] + clueCounter.ToString() + " ";
                        clueCounter = 0;
                    }
                }
            }
            if (clues[r,0] == "" || clueCounter != 0) { clues[r, 0] = clues[r, 0] + clueCounter.ToString(); }
        }

        for (int c = 0; c < cols; c++)
        {
            int clueCounter = 0;
            clues[c, 1] = "";
            for (int r = 0; r < rows; r++)
            {
                if (solMat[r, c] == true) { clueCounter += 1; }
                else
                {
                    if (clueCounter != 0)
                    {
                        clues[c, 1] = clues[c, 1] + clueCounter.ToString() + " ";
                        clueCounter = 0;
                    }
                }
            }
            if (clues[c, 1] != "" || clueCounter != 0) { clues[c, 1] = clues[c, 1] + clueCounter.ToString(); }
        }
    }

}

