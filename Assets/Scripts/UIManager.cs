using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using UnityEditor;
using CsvHelper;

public class ResultsRecord
{
    public int PuzzlesCompleted { get; set; }
    public int PuzzlesFailed { get; set; }
    public double AverageTime { get; set; }
    public double ATTF { get; set; }
}

public class UIManager : MonoBehaviour
{
    public bool showMenu = false;
    public bool showPuzWin = false;
    public bool showFin = false;
    public bool showPuzLoss = false;
    public GridManager gridManager;

    private float windowRatio = 0.6f;
    private bool resultsSaved = false;
    private bool showSmall = false;

    // Start is called before the first frame update
    void Start()
    {
        showMenu = true;
    }

    // Update is called once per frame
    void Update() 
    {
        if (Input.GetKey("escape")) { showSmall = true; }
    }

    void OnGUI()
    {
        //Window size
        Rect winRect = new Rect((Screen.width - Screen.width * windowRatio) / 2, (Screen.height - Screen.height * windowRatio) / 2, Screen.width * windowRatio, Screen.height * windowRatio);

        //Content of the main menu
        if (showMenu)
        {
            GUI.Box(winRect, "Picross");

            //Play button to start puzzles
            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Play"))
            {
                showMenu = false;
                gridManager.NewPuzzle();
            }
        } 

        //Content of the win screen
        if (showPuzWin)
        {
            GUI.Box(winRect, "Puzzle Complete! \n" + "Time: " + gridManager.timer.ToString());

            //Next puzzle button
            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Next Puzzle"))
            {
                showPuzWin = false;
                gridManager.NewPuzzle();
                resultsSaved = false;
            }

            //Results button
            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Results"))
            {
                //Close this screen, open final screen
                showPuzWin = false;
                showFin = true;
            }
        }

        //Content of the loss screen
        if (showPuzLoss)
        {
            GUI.Box(winRect, "Puzzle Failed.");

            //Next puzzle button
            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Next Puzzle"))
            {
                showPuzLoss = false;
                gridManager.NewPuzzle();
                resultsSaved = false;
            }

            //Results button
            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Results"))
            {
                showPuzLoss = false;
                showFin = true;
            }
        }

        //Final window, lists number of wins, losses and average time to both.
        if (showFin)
        {
            GUI.Box(winRect, "Results \n" + "Puzzles completed: " + gridManager.wins.ToString() + "\nPuzzles failed: " + gridManager.losses.ToString() + " \nAverage Time: " + gridManager.timeAvg + " \nATTF: " + gridManager.attf);

            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Save"))
            {
                //change this into a list of one.
                var results = new List<ResultsRecord>
                {
                    new ResultsRecord { PuzzlesCompleted = gridManager.wins, PuzzlesFailed = gridManager.losses, AverageTime = gridManager.timeAvg, ATTF = gridManager.attf }
                };

                //I want this to write to a csv file.
                //so first it needs to check if the file exists
                //then if it doesn't, make it
                // if it does, there's the datetime, and all of the data thats displayed on this screen, as fields, appeneded to it.

                // the file name should be picross_results.csv in the parent folder to the game
                string path = "..\\Picross_results.csv";
                if (!File.Exists(path))
                {
                    Debug.Log("File doesn't exist");
                    using (var writer = new StreamWriter(path))
                    using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                    {
                        csv.WriteRecords(results);
                    }
                }
                else
                {
                    Debug.Log("File does exist");
                }
                resultsSaved = true;
            }
            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Quit"))
            {
                //resultsSaved is always false until saving is implemented
                if (!resultsSaved) { showSmall = true; }
                else 
                {
                    //For debugging/developing purposes, check if how its running and close that
#if UNITY_EDITOR
                        if (UnityEditor.EditorApplication.isPlaying) { UnityEditor.EditorApplication.ExitPlaymode(); }
                        else { Application.Quit(); }
#else
                    Application.Quit();
#endif
                }
            }
        }

        //Show small is for confirming quitting the application without saving results.
        if (showSmall)
        {
            Rect smallWin = new Rect((Screen.width - Screen.width * (windowRatio / 2)) / 2, (Screen.height - Screen.height * (windowRatio / 2)) / 2, winRect.width / 2, winRect.height / 2);
            GUI.Box(smallWin, "Results have not been saved, Quit anyway?");
            if (GUI.Button(new Rect(smallWin.x + 10, smallWin.y + smallWin.height - 30, 70, 20), "Yes"))
            {
                //For debugging/developing purposes, check if how its running and close that
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying) { UnityEditor.EditorApplication.ExitPlaymode(); }
                else { Application.Quit(); }
#else
                Application.Quit();
#endif
            }
            if (GUI.Button(new Rect(smallWin.x + smallWin.width - 80, smallWin.y + smallWin.height - 30, 70, 20), "No"))
            {
                showSmall = false;
            }
        }
    }
}
