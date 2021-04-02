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

    public GUIStyle style;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnGUI()
    {
        Rect winRect = new Rect((Screen.width - winWidth) / 2, (Screen.height - winHeight) / 2, winWidth, winHeight);

        if (showMenu)
        { }

        if (showPuzWin)
        {
            GUI.Box(winRect, "Puzzle Complete! \n" + "Time: " + gridManager.timer.ToString(), style);


            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Next Puzzle"))
            {
                showPuzWin = false;
                gridManager.NewPuzzle();
            }

            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Results"))
            {
                showPuzWin = false;
                showFin = true;
            }
        }

        if (showPuzLoss)
        {
            GUI.Box(winRect, "Puzzle Failed.", style);


            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Next Puzzle"))
            {
                showPuzLoss = false;
                gridManager.NewPuzzle();
            }

            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Results"))
            {
                showPuzLoss = false;
                showFin = true;
            }
        }

        if (showFin)
        {
            GUI.Box(winRect, "Results \n" + "Puzzles completed: " + gridManager.wins.ToString() + "\nPuzzles failed: " + gridManager.losses.ToString() + " \nAverage Time: " + gridManager.timeAvg, style);
        }
    }
}
