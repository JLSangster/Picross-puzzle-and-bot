using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    //Grid dimensions
    private int rows;
    private int cols;
    private int clueNum;

    public UIManager uIManager;
    public GameObject fillToggle;
    private bool fill;
    public Text timerRead;

    private bool complete; //flag for if puzzle is complete
    private int correctCount; //int counting the current correct cell number
    private int completeCount; //total number of correct cells for puzzle

    private int maxMistakes; //max number of 'lives'
    private int mistakes; //current mistakes made

    private bool[,] solMat; //Solution matrix
    private string[,] clues;
    private int[,,] clueTileGrid;
    private GameObject[] mistakeSprites; //Array for mistake markers

    //results variables
    public int wins = 0;
    public int losses = 0;
    public float timer;
    public float timeAvg;
    public float attf;

    //Timer vars
    private float timeTot;
    private bool timerActive;
    private float lossTot;

    // Start is called before the first frame update
    void Start()
    {
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

        fill = (fillToggle.GetComponent<Toggle>().isOn);
        completeCount = 0;
        correctCount = 0;
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

    public void AddMistake()
    {
        mistakes += 1;
        Destroy(mistakeSprites[mistakes - 1]);
        if (mistakes >= maxMistakes) { PuzzleLost(); }
    }

    //Might not stay public
    public void PuzzleComplete()
    {
        timerActive = false;
        uIManager.showPuzWin = true;
        wins += 1;
        timeTot += timer;
        timeAvg = timeTot / wins;
    }

    public void PuzzleLost()
    {
        timerActive = false;
        losses += 1;
        uIManager.showPuzLoss = true;
        lossTot += timer;
        attf = lossTot / losses;
    }

    //Generate the grid of given dimensions
    void GenGrid()
    {
        GameObject cellRef = (GameObject)Instantiate(Resources.Load("Cell"));
        GameObject mistakeRef = (GameObject)Instantiate(Resources.Load("MistakeMarker"));
        GameObject clueTileRef = (GameObject)Instantiate(Resources.Load("ClueGrid"));

        float posX, posY;
        for (int r = (rows + clueNum - 1); r >= 0; r--)
        {
            int rowShift = 0;
            for (int c = (cols - 1); c >= -(clueNum); c--)
            {
                int colShift = 0;
                if (r < rows)
                {
                    if (c >= 0)
                    {
                        GameObject cell = (GameObject)Instantiate(cellRef, transform);
                        CellBehaviour cellBehaviour = cell.GetComponent<CellBehaviour>();
                        cellBehaviour.correct = solMat[r, c];
                        cellBehaviour.gridManager = this;

                        posX = c - (cols / 2);
                        posY = r - (rows / 2);

                        cell.transform.position = new Vector3(posX, posY, 0);
                    }
                    else
                    {
                        if (clueTileGrid[r,0, (c + clueNum)] != 0  || (clueTileGrid[r,0,0] == 0 && clueTileGrid[r,0,1] == 0) && (c + clueNum == 0))
                        {
                            GameObject clueTile = (GameObject)Instantiate(clueTileRef, transform);
                            clueTile.GetComponent<TextMeshPro>().text = clueTileGrid[r, 0, (c + clueNum)].ToString();

                            posX = (c + rowShift) - (cols / 2);
                            posY = r - (rows / 2);

                            clueTile.transform.position = new Vector3(posX, posY, 0);
                        }
                        else { rowShift++; }
                    }
                }
                else
                {
                    if (c >= 0)
                    {
                        if (clueTileGrid[c,1,(r - rows)] != 0 || (clueTileGrid[c,1,0] == 0 && clueTileGrid[c,1,1] == 0) && (r - rows == 0))
                        {
                            GameObject clueTile = (GameObject)Instantiate(clueTileRef, transform);
                            clueTile.GetComponent<TextMeshPro>().text = clueTileGrid[c,1,(r-rows)].ToString();

                            posX = c - (cols / 2);
                            posY = (r - colShift) - (rows / 2);

                            clueTile.transform.position = new Vector3(posX, posY, 0);
                        }
                        else { colShift++; }
                    }
                }
            }
        }

        for (int i = 0; i < maxMistakes; i++)
        {
            GameObject mistakeMarker = (GameObject)Instantiate(mistakeRef, transform);

            posX = cols;
            posY = i + 1.3f;

            mistakeMarker.transform.position = new Vector3(posX, posY, 0);
            mistakeSprites[i] = mistakeMarker;
        }

        Destroy(cellRef);
        Destroy(mistakeRef);
        Destroy(clueTileRef);

        //reset the timer to zero.
        timer = 0.0f;
        timerActive = true;
    }

    void GenPuzzle()
    {
        completeCount = 0;
        correctCount = 0;
        mistakes = 0;
        //randomize the size of the puzzle
        rows = 5 * Random.Range(1, 3);
        cols = rows;
        maxMistakes = rows / 2;
        clueNum = rows / 2 + 1;
        mistakeSprites = new GameObject[maxMistakes];

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

        //Calculating the clues
        clueTileGrid = new int[rows, 2, clueNum];

        //for each row
        for (int r = 0; r < rows;  r++)
        {
            //initialise the clue values to 0
            for (int i = 0; i < clueNum; i++)
            {
                clueTileGrid[r, 0, i] = 0;
            }

            int tile = 0;

            for (int c = 0; c < cols; c++)
            {
                if (solMat[r, c] == true)
                //if the cell is correct, it the counter
                {
                    clueTileGrid[r, 0, tile] += 1;
                }
                else
                //if the cell is incorrect
                {
                    //don't count
                    if (clueTileGrid[r,0,tile] != 0)
                    //if there were correct cells previous to this incorrect cell
                    {
                        //move to the next tile
                        tile += 1;
                    }
                }
            }
        }

        //IMPORTANT, for compatibility with the grid being generated from the BOTTOM UP rather than top down, the clues are listed from the bottom up
        for (int c = 0; c < cols; c++)
        {
            for (int i = 0; i < clueNum; i++)
            {
                clueTileGrid[c, 1, i] = 0;
            }
            int tile = 0;
            for (int r = 0; r < rows; r++)
            {
                if (solMat[r, c] == true)
                {
                    clueTileGrid[c, 1, tile] += 1;
                }
                else
                {
                    if(clueTileGrid[c,1,tile] != 0)
                    {
                        tile++;
                    }
                }
            }
        }
    }

}

