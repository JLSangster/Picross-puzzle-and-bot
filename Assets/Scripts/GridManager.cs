using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GridManager : MonoBehaviour
{
    //Grid dimensions
    public int size = 0;
    private int rows;
    private int cols;

    //debug settings
    public bool livesOn;

    public UIManager uIManager;
    public GameObject fillToggle;
    private bool fill;
    public Text timerRead;

    private bool complete; //flag for if puzzle is complete
    private int correctCount; //int counting the current correct cell number
    private int completeCount; //total number of correct cells for puzzle

    private int clueNum; //max possible number of clues
    private int maxMistakes; //max number of 'lives'
    private int mistakes; //current mistakes made

    public bool puzzleGen; //specifies if the puzzle has already been generated
    private bool[,] solMat; //Solution matrix
    private int[,,] clueTileGrid; //array of clues
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
        livesOn = true;
        puzzleGen = false;
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

    public void ToggleLives()
    {
        livesOn = !livesOn;
    }

    public void NewPuzzle()
    {
        print(puzzleGen);
        //Destroy all children from the previous puzzle
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //init puzzle variables
        fill = (fillToggle.GetComponent<Toggle>().isOn);
        completeCount = 0;
        correctCount = 0;
        mistakes = 0;

        //generate and display the puzzle
        if (!puzzleGen)
        {
            GenPuzzle();
        }
        CalcClues();
        GenGrid();

        puzzleGen = false;
    }    

    //fill get for interacting with the cells
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
        if (livesOn == true)
        {
            mistakes += 1;
            Destroy(mistakeSprites[mistakes - 1]);
            //if the mistakes are equal to the max mistakes, puzzle failed
            if (mistakes >= maxMistakes) { PuzzleLost(); }
        }
    }

    //puzzle complete
    private void PuzzleComplete()
    {
        timerActive = false; //stop timer
        wins += 1;
        uIManager.showPuzWin = true; //show screen

        //calc new average time
        timeTot += timer;
        timeAvg = timeTot / wins;
    }

    //puzzle lost
    private void PuzzleLost()
    {
        timerActive = false; //stop timer
        losses += 1;
        uIManager.showPuzLoss = true; //show screen

        //calc the new average time to fail puzzle
        lossTot += timer;
        attf = lossTot / losses;
    }

    public void ResetPuzzle()
    {
        //keep the same puzzle, but reset the lives to the original number and set every cell back to empty

        GameObject mistakeRef = (GameObject)Instantiate(Resources.Load("MistakeMarker"));
        float posX, posY;

        //reset the lives
        for (int i = 0; i <= mistakes; i++)
        {
            GameObject mistakeMarker = (GameObject)Instantiate(mistakeRef, transform);

            posX = cols;
            posY = i + 1.3f;

            mistakeMarker.transform.position = new Vector3(posX, posY, 0);
            mistakeSprites[i] = mistakeMarker;
        }

        Destroy(mistakeRef);

        mistakes = 0;
        correctCount = 0;

        //set every cell back to empty
        foreach (Transform child in transform)
        {
            if (child.TryGetComponent(out CellBehaviour cellBehaviour))
            {
                cellBehaviour.ResetCell();
            }
        }
    }

    //Generate the grid of given dimensions
    void GenGrid()
    {
        //Reference objects
        GameObject cellRef = (GameObject)Instantiate(Resources.Load("Cell"));
        GameObject mistakeRef = (GameObject)Instantiate(Resources.Load("MistakeMarker"));
        GameObject clueTileRef = (GameObject)Instantiate(Resources.Load("ClueGrid"));

        float posX, posY;
        //grid is generated from the bottom up
        for (int r = (rows + clueNum - 1); r >= 0; r--)
        {
            int rowShift = 0; //row counter for skipping empty clues
            for (int c = (cols - 1); c >= -(clueNum); c--)
            {
                int colShift = 0; //col counter for skipping empty clues
                if (r < rows)
                {
                    if (c >= 0)
                    {
                        //instantiate cell
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
                            //instantiate non-zero clue, or final clue if there are no non-zero clues
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
                            //instantiate non-zero clue, or final clue if there are no non-zero clues
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
            //display mistake markers
            GameObject mistakeMarker = (GameObject)Instantiate(mistakeRef, transform);

            posX = cols;
            posY = i + 1.3f;

            mistakeMarker.transform.position = new Vector3(posX, posY, 0);
            mistakeSprites[i] = mistakeMarker;
        }

        //destroy reference objects
        Destroy(cellRef);
        Destroy(mistakeRef);
        Destroy(clueTileRef);

        //reset the timer to zero.
        timer = 0.0f;
        timerActive = true;
    }



    public void SetPuzzle(int puzSize)
    {
        //Sets the puzzle to a specific solution
        
        puzzleGen = true;
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        //set the puzzle size to the size passed through from the ui
        size = puzSize;
        rows = size;
        cols = rows;
        solMat = new bool[rows, cols];

        //generate a grid of the other cells of that size
        GameObject cellSelRef = (GameObject)Instantiate(Resources.Load("CellSelector"));

        float posX, posY;
        //grid is generated from the bottom up
        for (int r = rows; r > 0; r--)
        {
            for (int c = cols; c > 0; c--)
            {
                //instantiate cell
                GameObject cell = (GameObject)Instantiate(cellSelRef, transform);
                CellSelectorBehaviour cellSelectorBehaviour = cell.GetComponent<CellSelectorBehaviour>();
                cellSelectorBehaviour.correct = false;
                cellSelectorBehaviour.gridManager = this;
                cellSelectorBehaviour.r = size - r;
                cellSelectorBehaviour.c = c - 1;
                solMat[r - 1, c - 1] = false;

                posX = c - (cols / 2);
                posY = r - (rows / 2);

                cell.transform.position = new Vector3(posX, posY, 0);
            }
        }

        Destroy(cellSelRef);
    }

    public void SetSol(int r, int c, bool correct)
    {
        print("updating solution");
        solMat[r, c] = correct;
    }

    void CalcClues()
    {
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
                //if the cell is correct, iterate the counter
                {
                    clueTileGrid[r, 0, tile] += 1;
                }
                else
                {
                    //don't count
                    if (clueTileGrid[r,0,tile] != 0)
                    //if there were correct cells previous to this incorrect cell
                    {
                        //move to the next tile
                        tile++;
                    }
                }
            }
        }

        //IMPORTANT, for compatibility with the grid being generated from the BOTTOM UP rather than top down, the clues are listed from the bottom up
        //for each column
        for (int c = 0; c < cols; c++)
        {
            //initialise the clue values to 0
            for (int i = 0; i < clueNum; i++)
            {
                clueTileGrid[c, 1, i] = 0;
            }

            int tile = 0;

            for (int r = 0; r < rows; r++)
            {
                if (solMat[r, c] == true)
                //if the cell is correct, iterate the counter
                {
                    clueTileGrid[c, 1, tile] += 1;
                }
                else
                {
                    if(clueTileGrid[c,1,tile] != 0)
                    //if there were correct cells previous to this incorrect cell
                    {
                        //move to the next tile
                        tile++;
                    }
                }
            }
        }

    }

    void GenPuzzle()
    {
        if (size == 0)
        {
                //randomize the size of the puzzle
                rows = 5 * Random.Range(1, 3);
        }
        else
        {
            rows = size;
        }
        //rows = size;
        cols = rows;
        maxMistakes = rows / 2;
        clueNum = rows / 2 + 1;
        mistakeSprites = new GameObject[maxMistakes];

        //init the mat
        solMat = new bool[rows, cols];

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
    }
}

