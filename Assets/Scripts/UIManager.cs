﻿using System.Collections;
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
    private bool showCustom = false;
    private bool showFin = false;
    private string message = "";
    private string livesTxt = "Lives on";
    private string sizeTxt = "Random";
    private string sizeSetTxt = "5 x 5";

    // Start is called before the first frame update
    void Start()
    {
        showMenu = true;
    }

    // Update is called once per frame
    void Update() 
    {
        if (Input.GetKey(KeyCode.Escape)) { showSmall = true; }
        if (Input.GetKey(KeyCode.D)) { showDebug = true; }
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
            GUI.Box(winRect, "\n Debug");
            
            //skip the current puzzle
            if (GUI.Button(new Rect(winRect.x + (winRect.width / 4), winRect.y + (winRect.height / 8) + 20, winRect.width / 6, winRect.height / 8), "Skip Puzzle")) { gridManager.NewPuzzle(); }

            //reset the current puzzle
            if (GUI.Button(new Rect(winRect.x + (winRect.width / 4), winRect.y + (winRect.height / 4) + 25, winRect.width / 6, winRect.height / 8), "Reset Puzzle")) { gridManager.ResetPuzzle(); }

            //toggle lives
            if (GUI.Button(new Rect(winRect.x + (winRect.width / 2), winRect.y + (winRect.height / 8) + 20, winRect.width / 6, winRect.height / 8), livesTxt))
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
            if (GUI.Button(new Rect(winRect.x + (winRect.width / 2), winRect.y + (winRect.height / 4) + 25, winRect.width / 6, winRect.height / 8), sizeTxt))
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

            if (GUI.Button(new Rect(winRect.x + (winRect.width / 2), winRect.y + (winRect.height / 2) + 30, winRect.width / 6, winRect.height / 8), "Custom Solution")) 
            {
                showCustom = true;
                showDebug = false;
                gridManager.SetPuzzle(5);
            }

            if (GUI.Button(new Rect(winRect.x + winRect.width - (winRect.width / 6) - 20, winRect.y + winRect.height - winRect.height / 8 - 40, winRect.width / 6, winRect.height / 8), "Close")) { showDebug = false; }
        }

        //Screen for creating a custom solution
        if (showCustom)
        {
            Rect sideWin = new Rect((Screen.width - Screen.width * (windowRatio / 4)) / 8, (Screen.height - Screen.height * (windowRatio / 2)) / 2, winRect.width / 4, (2 * (winRect.height / 3)));
            GUI.Box(sideWin, "Custom puzzle");

            //toggle for 5x5 / 10x10
            if (GUI.Button(new Rect(sideWin.x + (sideWin.width / 5), sideWin.y + (sideWin.height / 4), winRect.width / 6, winRect.height / 8), sizeSetTxt))
            {
                
                switch(sizeSetTxt)
                {
                    case ("5 x 5"):
                        gridManager.size = 10;
                        gridManager.SetPuzzle(10);
                        sizeSetTxt = "10 x 10";
                        break;
                    case ("10 x 10"):
                        gridManager.size = 5;
                        gridManager.SetPuzzle(5);
                        sizeSetTxt = "5 x 5";
                        break;
                }
            }

            //button to init that puzzle, the corrects are put into a matrix that is passed through to the grid manager...
            //and that is how the cells are spawned and the clues are calculated
            if (GUI.Button(new Rect(sideWin.x + (sideWin.width / 5), sideWin.y + (sideWin.height / 2), winRect.width / 6, winRect.height / 8), "Set")) 
            { 
                gridManager.NewPuzzle(); 
                showCustom = false;
                sizeSetTxt = "5 x 5";
            }

            if (GUI.Button(new Rect(sideWin.x + (sideWin.width / 5), sideWin.y + ( 3 * sideWin.height / 4), winRect.width / 6, winRect.height / 8), "Close")) 
            { 
                showCustom = false;
                gridManager.puzzleGen = false;
                gridManager.NewPuzzle();
                sizeSetTxt = "5 x 5";
            }
        }

        //Content of the win screen
        if (showPuzWin)
        {
            GUI.Box(smallWin, "\n Puzzle Complete! \n" + "Time: " + gridManager.timer.ToString());

            //Next puzzle button
            if (GUI.Button(new Rect(smallWin.x + smallWin.width - (winRect.width / 6) - 10, smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Next Puzzle"))
            {
                showPuzWin = false;
                gridManager.NewPuzzle();
                resultsSaved = false;
            }

            //Results button
            if (GUI.Button(new Rect(smallWin.x + 10, smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Results"))
            {
                //Close this screen, open final screen
                showPuzWin = false;
                showFin = true;
            }
        }

        //Content of the loss screen
        if (showPuzLoss)
        {
            GUI.Box(smallWin, "\n Puzzle Failed.");

            //Next puzzle button
            if (GUI.Button(new Rect(smallWin.x + smallWin.width - (winRect.width / 6) - 10, smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Next Puzzle"))
            {
                showPuzLoss = false;
                gridManager.NewPuzzle();
                resultsSaved = false;
            }

            //Results button
            if (GUI.Button(new Rect(smallWin.x + 10, smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Results"))
            {
                showPuzLoss = false;
                showFin = true;
            }
        }

        //Final window, lists number of wins, losses and average time to both.
        if (showFin)
        {
            GUI.Box(winRect, "\n Results \n \n" + "Puzzles completed: " + gridManager.wins.ToString() + "\nPuzzles failed: " + gridManager.losses.ToString() + " \nAverage Time: " + gridManager.timeAvg + " \nATTF: " + gridManager.attf);

            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - winRect.height / 8 - 40, winRect.width / 6, winRect.height / 8), "Save"))
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
            if (GUI.Button(new Rect(winRect.x + winRect.width - (winRect.width / 6) - 20, winRect.y + winRect.height - winRect.height / 8 - 40, winRect.width / 6, winRect.height / 8), "Quit"))
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
            GUI.Box(smallWin, "\n Results have not been saved, Quit anyway?");
            if (GUI.Button(new Rect(smallWin.x + 10, smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Yes"))
            {
                //For debugging/developing purposes, check if how its running and close that
#if UNITY_EDITOR
                if (UnityEditor.EditorApplication.isPlaying) { UnityEditor.EditorApplication.ExitPlaymode(); }
                else { Application.Quit(); }
#else
                Application.Quit();
#endif
            }
            if (GUI.Button(new Rect(smallWin.x + smallWin.width - (winRect.width / 6) - 10 , smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "No"))
            {
                showSmall = false;
            }
        }

        //Show results save confirmation window
        if (showSave)
        {
            GUI.Box(smallWin, "\n Results saved at \n" + message);
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 2) - (winRect.width / 12), smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Ok")) { showSave = false; }
        }

        if (showErr)
        {
            GUI.Box(smallWin, message);
            if (GUI.Button(new Rect(smallWin.x + (smallWin.width / 2) - (winRect.width / 12), smallWin.y + smallWin.height - winRect.height / 10 - 30, winRect.width / 6, winRect.height / 10), "Ok")) { showErr = false; }
        }
    }
}
