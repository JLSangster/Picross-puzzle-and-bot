using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    //Is this particularly important?
    private bool showWin = false;
    public int winWidth, winHeight;
    public GridManager gridManager;

    //private GUIStyle style = new GUIStyle();

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
        showWin = complete;
    }

    void OnGUI()
    {
        if (showWin)
        {
            Rect winRect = new Rect((Screen.width - winWidth)/2, (Screen.height - winHeight)/2, winWidth, winHeight);
            GUI.Box(winRect, "Puzzle Complete!");


            if (GUI.Button(new Rect(winRect.x + winRect.width - 170, winRect.y + winRect.height - 60, 150, 40), "Next Puzzle"))
            {
                showWin = false;
                gridManager.NewPuzzle();

            }

            if (GUI.Button(new Rect(winRect.x + 20, winRect.y + winRect.height - 60, 150, 40), "Results"))
            {
                //Will be for the test result summary once I put that in
                Debug.Log("OtherButtonpress");
            }
        }
    }
}
