using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Peaking : MonoBehaviour
{
    public string player;
    public string opponent;
    private bool isPeaking;
    private bool hasChosenCardToPeak;
    private CardPresenter chosenCard;

    AudioSource peakingAudio;

    public void Awake()
    {
        peakingAudio = GetComponent<AudioSource>();
        Events.OnTurnStart += IsPeaking;
    }
    void Start()
    {
        
    }

    void Update()
    {
        if (isPeaking && !hasChosenCardToPeak && PilesController.Instance.howManyAdded > 2)
        {
            if (Input.GetMouseButtonDown(0) && GameController.Instance.player == player)
            {
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                Collider2D[] colliders = Physics2D.OverlapPointAll(mousePos);
                Collider2D collider = null;
                foreach (Collider2D col in colliders) {

                    if (col.isTrigger && (col.gameObject.name.Contains("Pile") || col.gameObject.tag.Contains("Pile"))) {
                        collider = col; break;
                    }
                }
                if (collider != null && collider.isTrigger)
                {
                    hasChosenCardToPeak = true;
                    if (collider.gameObject.name.StartsWith("Pile"))
                    {
                        // get card from that pile
                        int pileNumber = int.Parse(collider.gameObject.name.Remove(0, 4));
                        CardPresenter card = PilesController.Instance.GetCardFromPileTop(opponent, pileNumber);
                        if (card != null)
                        {
                            peakingAudio.Play();
                            card.MakeVisible();
                            card.MakeBigger();
                            chosenCard = card;
                        }
                        else { hasChosenCardToPeak = false; }
                    }
                    else if (collider.gameObject.tag.StartsWith("DancerPile"))
                    {
                        hasChosenCardToPeak = true;
                        int pileNumber = int.Parse(collider.gameObject.name.Remove(0, 6));
                        if (pileNumber % 2 == 0)
                        {
                            hasChosenCardToPeak = false;
                        }
                        else
                        {
                            peakingAudio.Play();
                            CardPresenter card = PilesController.Instance.GetCardFromPileTop("Dancer", pileNumber);
                            card.MakeVisible();
                            card.MakeBigger();
                            chosenCard = card;
                        }
                    }
                }

            }
            else if(GameController.Instance.ai == player && chosenCard == null){// AI peaking
                Tuple<string,int> chosen = AIController.Instance.PeakACard();
                hasChosenCardToPeak = true;
                int pileNumber = chosen.Item2;
                string pileName = chosen.Item1;
                CardPresenter card = PilesController.Instance.GetCardFromPileTop(pileName, pileNumber);
                chosenCard = card;
                if (pileName != "Dancer")
                {
                    AIController.Instance.AddPreviouslyPeakedOpponentCard(card, pileNumber);
                }
                else {
                    AIController.Instance.AddPeakedDancers(card.data.Strength, pileNumber);
                }
            }
        }
    }

    public void OnDestroy()
    {
        Events.OnTurnStart -= IsPeaking;

    }

    public void IsPeaking(string playerName, int turnstage) {
        if (turnstage == 2 && playerName == player)
        {
            hasChosenCardToPeak = false;
            isPeaking = true;
        }
        else if (turnstage == 1 && playerName == GameController.Instance.ai)
        {
            if (chosenCard != null)
            {
                peakingAudio.Play();
                chosenCard.MakeInvisible();
                chosenCard.MakeSmaller();
                chosenCard = null;

            }
            isPeaking = false;
        }
        else {
            isPeaking = false;
            chosenCard = null;
        }
    }
}
