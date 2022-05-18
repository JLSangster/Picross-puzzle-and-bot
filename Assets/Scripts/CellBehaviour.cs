using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellBehaviour : CellSelectorBehaviour
{
    //vars for sprite changing
    public Sprite markedSprite;
    public Sprite wrongSprite;

    //flags for if the cell is filled or marked
    private bool marked = false;
    private bool filled = false;

    // Start is called before the first frame update
    void Start() {    }

    // Update is called once per frame
    void Update() {    }

    //on click, the sprite will change
    void OnMouseDown()
    {
        //if the fill toggle is set to fill cells
        if (gridManager.GetFill())
        {
            //If the cell is not marked or already filled
            if (!(marked || filled))
            {
                if (correct) 
                {
                    //Fill the cell and ammend the current correct cells
                    spriteRenderer.sprite = filledSprite;
                    filled = true;
                    gridManager.AddCorrectCell();

                }
                //if the cell is incorrect
                else {
                    //Change the sprite and ammend current mistakes
                    spriteRenderer.sprite = wrongSprite;
                    gridManager.AddMistake();
                }
            }
        }

        //if it is set to mark (or cross) cells
        else
        {
            //Toggle if the cell is marked
            if (!filled)
            {
                if (!marked)
                {
                    spriteRenderer.sprite = markedSprite;
                    marked = true;
                }
                else
                {
                    spriteRenderer.sprite = emptySprite;
                    marked = false;
                }
            }
        }
    }

    public void ResetCell()
    {
        filled = false;
        marked = false;

        spriteRenderer.sprite = emptySprite;
    }
}
