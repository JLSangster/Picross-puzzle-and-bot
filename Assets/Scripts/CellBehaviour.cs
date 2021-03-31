using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellBehaviour : MonoBehaviour
{
    //flag for if it *should* be right or wrong in the solution
    public bool correct;
    //vars for sprite changing
    public SpriteRenderer spriteRenderer;
    public Sprite emptySprite;
    public Sprite filledSprite;
    public Sprite markedSprite;
    public Sprite wrongSprite;
    public GridManager gridManager;

    private bool marked = false;
    private bool filled = false;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //on click, depending on the correct thing for the cell the sprite should change.
    void OnMouseDown()
    {
        //if the fill toggle is set to fill cells
        if (gridManager.GetFill())
        {
            //If the cell is not marked or already filled
            if (!marked)
            {
                if (correct) 
                {
                    //Fill the cell and ammend the current correct cells
                    spriteRenderer.sprite = filledSprite;
                    filled = true;
                    gridManager.AddCorrectCell();

                }
                //if the cell is incorrect
                else { spriteRenderer.sprite = wrongSprite; }
            }
        }
        //if it is set to mark (or cross) cells
        else 
        {
            if (!filled)
            {
                //Toggle if the cell is marked
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
}
