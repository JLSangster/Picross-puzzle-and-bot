﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    private int rows = 5;
    private int cols = 5;
    private float tileSize = 32;
    public GameObject fillToggle;
    private bool fill;
    //flag for if puzzle is complete, might need to be changed to public, haven't decided what happens when its done yet.
    private bool complete;
    //int counting the current correct cell number
    private int correctCount;
    //
    private int completeCount;
    //solution matrix
    private bool[,] solMat;
    private string[,] clues;
    public UIManager uIManager;
    public int wins = 0;
    public float timer;
    public Text timerRead;
    private float timeTot;
    public float timeAvg;
    private bool timerActive;

    // Start is called before the first frame update
    void Start()
    {
        fill = (fillToggle.GetComponent<Toggle>().isOn);
        completeCount = 0;
        correctCount = 0;
        NewPuzzle();
    }

    // Update is called once per frame
    void Update()
    {
        if (timerActive)
        {
            timer += Time.deltaTime;
            timerRead.text = timer.ToString();
        }
    }

    public void NewPuzzle()
    {
        //Destroying all children from the previous puzzle
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
        GenPuzzle();
        GenGrid();
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
        timerActive = false;
        uIManager.setShowWin(true);
        wins += 1;
        timeTot += timer;
        //this should probably change to wins + fails once I decide on a fail perameter
        timeAvg = timeTot / wins;               
    }

    //Generate the grid of given dimensions
    void GenGrid()
    {
        uIManager.setShowWin(false);
        GameObject cellRef = (GameObject)Instantiate(Resources.Load("Cell")); 
        GameObject rowLabelRef = (GameObject)Instantiate(Resources.Load("RowLabel"));
        GameObject colLabelRef = (GameObject)Instantiate(Resources.Load("ColLabel"));

        for (int r = 0; r < rows; r++)
        {
            //print row label first
            GameObject rowLabel = (GameObject)Instantiate(rowLabelRef, transform);
            rowLabel.GetComponent<TextMeshProUGUI>().text = clues[r, 0].ToString();

            float posX, posY;
            //mathematically simplified equations - not neccesarily computationally more efficient
            //posX = (0 * tileSize) - (cols/2 * tileSize) - 16;
            posX = -(((cols * tileSize) + tileSize) / 2);
            //posY = ((r * tileSize) - (rows/2 * tileSize) + tileSize);
            posY = tileSize * (r - (rows / 2) + 1);

            rowLabel.transform.localPosition = new Vector3(posX, posY, 0);

            for (int c = 0; c < cols; c++)
            {
                GameObject cell = (GameObject)Instantiate(cellRef, transform);
                CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
                cellBehaviour.correct = solMat[r,c];
                cellBehaviour.gridManager = this;

                posX = (c * 1) - (cols/2 * 1);
                posY = (r * 1) - (rows/2 * 1);

                cell.transform.position = new Vector3(posX, posY, 0);
            }
        }

        for (int c = 0; c < cols; c++)
        {
            GameObject colLabel = (GameObject)Instantiate(colLabelRef, transform);
            colLabel.GetComponent<TextMeshProUGUI>().text = clues[c, 1].ToString();

            float posX, posY;
            //again with the simpliefied equations being used. This positioning could be improved here, QoL ideally the text is displayed from the bottom up
            //posX = (c * tileSize) - (cols / 2 * tileSize);
            posX = tileSize * (c - (cols / 2));
            //posY = ((0 * tileSize) + (rows / 2 * tileSize)) - 16;
            posY = tileSize * ((rows - 1)/ 2);

            colLabel.transform.localPosition = new Vector3(posX, posY, 0);
        }

        Destroy(cellRef);
        Destroy(rowLabelRef);
        Destroy(colLabelRef);

        //reset the timer to zero.
        timer = 0.0f;
        timerActive = true;
    }

    void GenPuzzle()
    {
        completeCount = 0;
        correctCount = 0;
        //randomize the size of the puzzle
        rows =  5 * Random.Range(1, 2);
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
            for (int r = (rows - 1); r >= 0; r--)
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
            if (clues[c, 1] == "" || clueCounter != 0) { clues[c, 1] = clues[c, 1] + clueCounter.ToString(); }
        }
    }

}

