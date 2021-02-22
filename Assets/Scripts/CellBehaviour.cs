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
    public Sprite filledSprite;
    public Sprite markedSprite;
    public Sprite wrongSprite;
    public GridManager gridManager;

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
            Debug.Log("Fill true");
            if (correct) { spriteRenderer.sprite = filledSprite; } //This function should also be do soemthing to indicate to the wider grid that the thing is correct so it could check if the puzzle is complete.
            else { spriteRenderer.sprite = wrongSprite; }
        }
        else 
        {
            Debug.Log("Fill false");
            spriteRenderer.sprite = markedSprite;
        }
    }   
}
