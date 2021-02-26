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
    private bool marked;
    //some variable to indicate that the cell has been correctly marked

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
        gridManager.GetFill();
        if (gridManager.GetFill())
        {
            if (!marked)
            {
                if (correct) 
                {
                    spriteRenderer.sprite = filledSprite;
                    gridManager.AddCorrectCell();

                } //This function should also be do soemthing to indicate to the wider grid that the thing is correct so it could check if the puzzle is complete.
                else { spriteRenderer.sprite = wrongSprite; }
            }
        }
        else 
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
