using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public bool showMenu = false;
    public bool showPuzWin = false;
    public bool showFin = false;
    public bool showPuzLoss = false;
    public int winWidth, winHeight;
    public GridManager gridManager;

    // Start is called before the first frame update
    void Start()
    {
        showMenu = true;
    }

    // Update is called once per frame
    void Update() {    }

    void OnGUI()
    {
        //Window size
        Rect winRect = new Rect((Screen.width - winWidth) / 2, (Screen.height - winHeight) / 2, winWidth, winHeight);

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
        }
    }
}
