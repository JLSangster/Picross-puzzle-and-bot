using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    private bool showMenuWin = false;
    private bool showPuzWin = false;
    private bool showFinWin = false;
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

    //Level transistion screen, grid manager passes a flag up to this
    public void setShowWin(bool complete)
    {
        showPuzWin = complete;
    }

    void OnGUI()
    {
        Rect winRect = new Rect((Screen.width - winWidth) / 2, (Screen.height - winHeight) / 2, winWidth, winHeight);

        if (showMenuWin)
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
                showFinWin = true;
            }
        }

        if (showFinWin)
        {
            GUI.Box(winRect, "Results \n" + "Puzzles completed: " + gridManager.wins.ToString() + " \nAverage Time: " + gridManager.timeAvg, style);
        }
    }
}
