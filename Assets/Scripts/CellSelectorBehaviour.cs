using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellSelectorBehaviour : MonoBehaviour
{
    //flag for if it should be right or wrong in the solution
    public bool correct;

    //vars for sprite changing
    public SpriteRenderer spriteRenderer;
    public Sprite emptySprite;
    public Sprite filledSprite;

    public GridManager gridManager;

    public int r;
    public int c;

    // Start is called before the first frame update
    void Start() {    }

    // Update is called once per frame
    void Update() {    }

    void OnMouseDown()
    {
        //When the cell is clicked, it toggles if it is correct, and fills or clears the cell.
        if (correct)
        {
            print("correct");
            spriteRenderer.sprite = emptySprite;
            correct = false;
            gridManager.SetSol(r, c, correct);
        }
        else
        {
            spriteRenderer.sprite = filledSprite;
            correct = true;
            gridManager.SetSol(r, c, correct);
            print("updated");
        }
    }
}
