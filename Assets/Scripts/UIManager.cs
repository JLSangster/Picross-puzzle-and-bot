using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System;
using UnityEngine;
using UnityEditor;
using CsvHelper;
using CsvHelper.TypeConversion;

public class ResultsRecord
{
    public int PuzzlesCompleted { get; set; }
    public int PuzzlesFailed { get; set; }
    public double AverageTime { get; set; }
    public double ATTF { get; set; }
}

public class UIManager : MonoBehaviour
{
    public bool showPuzWin = false;
    public bool showPuzLoss = false;
    public GridManager gridManager;

    private float windowRatio = 0.6f;
    private bool resultsSaved = false;
    private bool showSmall = false;
    private bool showSave = false;
    private bool showErr = false;
    private bool showMenu = false;
    private bool showDebug = false;
    private bool showFin = false;
    private string message = "";
    private string livesTxt = "Lives on";
    private string sizeTxt = "Random";

    // Start is called before the first frame update
    void Start()
    {
        showMenu = true;
    }

    // Update is called once per frame
    void Update() 
    {
        if (Input.GetKey(KeyCode.Escape)) { showSmall = true; }
        if (Input.GetKey(KeyCode.RightShift)) { showDebug = true; }
    }

    void OnGUI()
    {
        GUI.skin.box.wordWrap = true;

        //Window size
        Rect winRect = new Rect((Screen.width - Screen.width * windowRatio) / 2, (Screen.height - Screen.height * windowRatio) / 2, Screen.width * windowRatio, Screen.height * windowRatio);
        Rect smallWin = new Rect((Screen.width - Screen.width * (windowRatio / 2)) / 2, (Screen.height - Screen.height * (windowRatio / 2)) / 2, winRect.width / 2, winRect.height / 2);

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

        if (showDebug)
        {
            GUI.Box(winRect, "Debug");
            
            //skip the current puzzle
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 4), smallWin.y + (smallWin.height / 8), 70, 20), "Skip Puzzle")) { gridManager.NewPuzzle(); }

            //reset the current puzzle
            if (GUI.Button(new Rect(smallWin.x + (3 * smallWin.width / 4), smallWin.y + (smallWin.height / 8), 70, 20), "Reset Puzzle")) { gridManager.ResetPuzzle(); }

            //toggle lives
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 4), smallWin.y + (smallWin.height / 3), 70, 20), livesTxt))
            {
                gridManager.ToggleLives();
                switch(livesTxt)
                {
                    case ("Lives on"):
                        livesTxt = "Lives off";
                        break;
                    case ("Lives off"):
                        livesTxt = "Lives on";
                        break;
                }
            }

            //Chose puzzle size
            if (GUI.Button(new Rect(smallWin.x + (3 * smallWin.width / 4), smallWin.y + (smallWin.height / 3), 70, 20), sizeTxt))
            {
                switch(sizeTxt)
                {
                    case ("Random"):
                        gridManager.size = 5;
                        gridManager.NewPuzzle();
                        sizeTxt = "5 x 5";
                        break;
                    case ("5 x 5"):
                        gridManager.size = 10;
                        gridManager.NewPuzzle();
                        sizeTxt = "10 x 10";
                        break;
                    case ("10 x 10"):
                        gridManager.size = 0;
                        gridManager.NewPuzzle();
                        sizeTxt = "Random";
                        break;
                }
            }

            if (GUI.Button(new Rect(smallWin.x + smallWin.width - 30, smallWin.y + smallWin.height - 5, 70, 20), "Close")) { showDebug = false; }
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
                //Create the data record
                var results = new List<ResultsRecord>
                {
                    new ResultsRecord { PuzzlesCompleted = gridManager.wins, PuzzlesFailed = gridManager.losses, AverageTime = gridManager.timeAvg, ATTF = gridManager.attf }
                };

                // the file name should be Picross_results.csv in the program file
                string path = ".\\Picross_results.csv";

                try
                {
                    //If the file doesn't exist
                    if (!File.Exists(path))
                    {
                        //Create the file 
                        using (var writer = new StreamWriter(path))
                        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
                        {
                            csv.WriteRecords(results);
                        }
                        message = AppDomain.CurrentDomain.BaseDirectory + path;
                    }
                    //If the file does exist
                    else
                    {
                        //Append the re cord to the existing file
                        var csvConfig = new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = false };

                        using (var stream = File.Open(path, FileMode.Append))
                        using (var writer = new StreamWriter(stream))
                        using (var csv = new CsvWriter(writer, csvConfig)) { csv.WriteRecords(results); }
                        message = AppDomain.CurrentDomain.BaseDirectory + path;
                    }

                    resultsSaved = true;
                    showSave = true;
                }
                catch (Exception e)
                {
                    message = e.Message;
                    showDebug = true;
                }
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

        //Show results save confirmation window
        if (showSave)
        {
            GUI.Box(smallWin, "Results saved at " + message);
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 2), smallWin.y + smallWin.height - 30, 70, 20), "Ok")) { showSave = false; }
        }

        if (showErr)
        {
            GUI.Box(smallWin, message);
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 2), smallWin.y + smallWin.height - 30, 70, 20), "Ok")) { showErr = false; }
        }
    }
}
