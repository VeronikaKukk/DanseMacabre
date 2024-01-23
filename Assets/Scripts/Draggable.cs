using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Draggable : MonoBehaviour
{
    public CardPresenter card;

    public bool isDraggable = false;
    private bool isDragged = false;
    public bool isInPile = false;
    private Vector3 mouseDragStartPos;
    private Vector3 spriteDragStartPos;

    private Transform originalParent;
    public Transform pileToSnapTo;
    public int cardPileNumber;

    AudioSource snapToPileAudio;

    public void Start()
    {
        snapToPileAudio = GetComponent<AudioSource>();
        card = GetComponent<CardPresenter>();
    }
    private int pileNumber;
    private void OnMouseDown()
    {
        if (isDraggable && !isInPile)
        {
            isDragged = true;
            mouseDragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            spriteDragStartPos = transform.localPosition;
            GetComponent<SpriteRenderer>().sortingOrder = 10;
            originalParent = transform.parent;
            
        }
    }
    private void OnMouseUp()
    {
        if (isDraggable && isDragged && !isInPile)
        {
            isDragged = false;

            if (pileToSnapTo != null)
            {
                snapToPileAudio.Play();
                GetComponent<SpriteRenderer>().sortingOrder = 5;
                transform.position = new Vector3(pileToSnapTo.position.x+0.3f, pileToSnapTo.position.y+0.3f, pileToSnapTo.position.z);
                isInPile = true;
                transform.SetParent(pileToSnapTo);
                CardPresenter last = PilesController.Instance.lastMoved;
                if (last == null)
                {
                    PilesController.Instance.lastMoved = card;
                }
                else {
                    last.GetComponent<Draggable>().ChangeBackToDefault();
                    PilesController.Instance.lastMoved = card;
                }
            }
            else
            {
                transform.localPosition = spriteDragStartPos;
                transform.SetParent(originalParent);
            }
        }

    }

    public void ChangeBackToDefault() 
    {
        isInPile = false;
        transform.SetParent(originalParent);
        transform.localPosition = spriteDragStartPos;
        pileToSnapTo = null;
        cardPileNumber = -1;
        GetComponent<SpriteRenderer>().sortingOrder = 0;
    }

    private void OnMouseDrag()
    {
        if(isDraggable && isDragged && !isInPile)
        {
            transform.localPosition = spriteDragStartPos + (Camera.main.ScreenToWorldPoint(Input.mousePosition) - mouseDragStartPos);
        }
    }

     void OnTriggerEnter2D(Collider2D col)
    {
        Collider2D pile = col;
        
        if (pile != null && pile.tag == "CardPile")
        {
                pileToSnapTo = pile.transform;
                cardPileNumber = int.Parse(pile.name.Substring(4));
        }
    }

     void OnTriggerExit2D(Collider2D col)
    {
        Collider2D pile = col;
        if (pile != null && pileToSnapTo == pile)
        {
            pileToSnapTo = null;
            cardPileNumber = -1;
        }
    }

    public void SetCardPileNumber(int number) {
        cardPileNumber = number;
    }

    public int GetCardPileNumber() {  return cardPileNumber; }
}
