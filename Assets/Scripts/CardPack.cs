using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardPack : MonoBehaviour
{
    public List<Card> Cards;
    public CardPresenter CardPresenterPrefab;
    public GameObject CardPanel;
    public List<CardPresenter> presenters;

    public Button EndTurnButton;
    public TextMeshProUGUI EndTurnButtonText;

    public string Owner;
    public string Opponent;

    private int turnStage;
    private bool isPeaked;

    public void Awake()
    {
        Events.OnTurnStart += OnStartTurn;
    }
    private void Start()
    {
        turnStage = -1;
        ShuffleCards();

        bool isVisible = true;
        if (GameController.Instance.player == Opponent)
        {
            isVisible = false;
        }

        foreach (Card card in Cards)
        {
            CardPresenter presenter = Instantiate<CardPresenter>(CardPresenterPrefab, CardPanel.transform);
            presenter.init(card, isVisible, Owner);
            presenters.Add(presenter);

        }
    }
    public void ShuffleCards()
    {
        for (int i = Cards.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = Cards[i];
            Cards[i] = Cards[j];
            Cards[j] = temp;
        }
    }
    public void Update()
    {
        if (turnStage == 2 || (turnStage == 1 && PilesController.Instance.lastMoved != null))
        {
            EndTurnButton.interactable = true;
        }
        else
        {
            EndTurnButton.interactable = false;
        }

        if (GameController.Instance.ai == Owner && AIController.Instance.MoveCompleted) {
            AIController.Instance.MoveCompleted = false;
            Invoke("EndTurn",0.5f);
        }
    }

    public void OnStartTurn(string nextPlayerName, int turnStageNumber)
    {
        turnStage = turnStageNumber;
        if (GameController.Instance.ai != Owner)
        {
            if (nextPlayerName == Owner && turnStage == 1) // placing card on board
            {
                foreach (CardPresenter presenter in presenters)
                {
                    var dragging = presenter.GetComponent<Draggable>();
                    dragging.isDraggable = true;
                }
                EndTurnButtonText.text = "Confirm the Card placement";

            }
            else if (nextPlayerName == Owner && turnStage == 2) // peaking
            {
                if (PilesController.Instance.howManyAdded > 2)
                {
                    //Debug.Log(gameObject.name + " is peaking");
                    EndTurnButtonText.text = "End peaking";
                }
                else
                {
                    EndTurnButtonText.text = "End turn";
                }
            }
            else
            {
                turnStage = -1;
            }
        }
        else {
            if (nextPlayerName == Owner && turnStage == 1) // placing card on board
            {
                EndTurnButtonText.text = "Confirm the Card placement";
                AIController.Instance.PlayCardToPile(presenters);
            }
            else if (nextPlayerName == Owner && turnStage == 2) // peaking
            {
                if (PilesController.Instance.howManyAdded > 2)
                {
                    //Debug.Log(gameObject.name + " is peaking");
                    EndTurnButtonText.text = "End peaking";
                    //AIController.Instance.PeakACard();
                }
                else
                {
                    EndTurnButtonText.text = "End turn";
                    Invoke("EndTurn",0.5f);
                }
            }
            else
            {
                turnStage = -1;
            }

        }

    }
    public void EndTurn()
    {
        foreach (CardPresenter presenter in presenters)
        {
            var dragging = presenter.GetComponent<Draggable>();
            dragging.isDraggable = false;
        }

        if (turnStage == 1)
        {
            CardPresenter card = PilesController.Instance.lastMoved;
            card.GetComponent<SpriteRenderer>().sortingOrder = 0;
            int pileNumber = card.GetComponent<Draggable>().GetCardPileNumber();
            float offset = PilesController.Instance.AddCardToPile(Owner, card, pileNumber);
            presenters.Remove(card);
            float offsety = offset * 0.2f;
            float offsetx = offset * 0.1f;
            if (Owner == GameController.Instance.ai) {
                offsetx = -offsetx;
                offsety = -offsety;
            }
            if (GameController.Instance.player == Owner) {
                card.transform.position = card.GetComponent<Draggable>().pileToSnapTo.position;
            }
            card.transform.localPosition = new Vector3(card.transform.localPosition.x - offsetx, card.transform.localPosition.y - offsety, card.transform.localPosition.z - offset);
            Events.TurnStart(Owner, 2);
        }
        else
        {
            Events.TurnStart(Opponent, 1);
        }

    }

    public void OnDestroy()
    {
        Events.OnTurnStart -= OnStartTurn;
    }
}
