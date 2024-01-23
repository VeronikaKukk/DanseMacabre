using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PilesController : MonoBehaviour
{
    public static PilesController Instance;
    public List<List<CardPresenter>> DevilPileDecks;
    public List<List<CardPresenter>> GodPileDecks;

    private List<CardPresenter> Dancers;

    public int howManyAdded = 0;

    public CardPresenter lastMoved;

    public List<GameObject> DevilPiles;
    public List<GameObject> GodPiles;

    private int GodPoints;
    private int DevilPoints;

    public bool isCountingTime;
    public bool isCountingPointsOver;
    public bool isGameOver;

    public float AnimationDelay = 2f;
    private float NextAnimationTime;
    private int currentAnimationPile = 0;

    public TextMeshProUGUI DevilPointsText;
    public TextMeshProUGUI GodPointsText;

    public Color GetPointsColor;

    public void Awake()
    {
        Instance = this;
        Events.OnTurnStart += OnStartTurn;
    }
    public void OnDestroy()
    {
        Events.OnTurnStart -= OnStartTurn;
    }

    void Start()
    {
        CreateEmptyPiles();
    }

    void Update()
    {
        if (!isCountingTime && howManyAdded == 32)//32 is full deck
        {
            Events.StartCountingPoints();
            foreach (GameObject pile in GodPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
                
                    foreach (SpriteRenderer obj in pile.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (obj.name.StartsWith("Card"))
                        {
                            obj.color = Color.white;
                        }
                    }
                
            }

            foreach (GameObject pile in DevilPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
               
                    foreach (SpriteRenderer obj in pile.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (obj.name.StartsWith("Card"))
                        {
                            obj.color = Color.white;
                        }
                    }
                
            }
            isCountingTime = true;
            NextAnimationTime = Time.time + AnimationDelay;
            DevilPointsText.text = "0";
            GodPointsText.text = "0";
        }

        if (isCountingTime && NextAnimationTime < Time.time && currentAnimationPile < 11) {
            AnimatePoints();
            currentAnimationPile++;
            NextAnimationTime = Time.time + AnimationDelay;
        }

        if (NextAnimationTime < Time.time && currentAnimationPile > 10) {
            isCountingPointsOver = true;
        }

        if (isCountingPointsOver && !isGameOver) {
            isGameOver = true;
            if (GodPoints > DevilPoints)
            {
                Events.EndGame("God");

            }
            else
            {
                Events.EndGame("Devil");
            }
        }
    }

    public void OnStartTurn(string nextPlayerName, int turnStageNumber) 
    {
        // dont let to place cards in wrong pile side
        if (GameController.Instance.player == "God" && nextPlayerName == "God" && turnStageNumber == 1 || GameController.Instance.player == "Devil" && nextPlayerName == "Devil" && turnStageNumber == 2)
        {
            foreach (GameObject pile in GodPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = true;
            }

            foreach (GameObject pile in DevilPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
            }
        }
        else if (GameController.Instance.player == "Devil" && nextPlayerName == "Devil" && turnStageNumber == 1 || GameController.Instance.player == "God" && nextPlayerName == "God" && turnStageNumber == 2)
        {
            foreach (GameObject pile in GodPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
            }

            foreach (GameObject pile in DevilPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
        else {
            foreach (GameObject pile in GodPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
                if (turnStageNumber == 1)
                {
                    foreach (SpriteRenderer obj in pile.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (obj.name.StartsWith("Card"))
                        {
                            obj.color = Color.white;
                        }
                    }
                }
            }

            foreach (GameObject pile in DevilPiles)
            {
                pile.GetComponent<BoxCollider2D>().isTrigger = false;
                if (turnStageNumber == 1)
                {
                    foreach (SpriteRenderer obj in pile.GetComponentsInChildren<SpriteRenderer>())
                    {
                        if (obj.name.StartsWith("Card"))
                        {
                            obj.color = Color.white;
                        }
                    }
                }
            }
        }

        if (turnStageNumber == 2)
        {
            foreach (CardPresenter presenter in Dancers)
            {
                presenter.GetComponent<BoxCollider2D>().isTrigger = true;
            }
        }
        else {
            foreach(CardPresenter presenter in Dancers)
            {
                presenter.GetComponent<BoxCollider2D>().isTrigger = false;
            }

        }

    }

    public void AddDancers(List<CardPresenter> presenters) {
        Dancers = presenters;
        foreach (CardPresenter presenter in Dancers) {
            presenter.tag = "DancerPile";
        }
    }
    public List<CardPresenter> GetDancers() {
        return Dancers;
    }

    public void CreateEmptyPiles() {
        DevilPileDecks = new List<List<CardPresenter>>();
        GodPileDecks = new List<List<CardPresenter>>();
        for (int i = 0; i < 11; i++)
        {
            List<CardPresenter> temp1 = new List<CardPresenter>();
            List<CardPresenter> temp2 = new List<CardPresenter>();

            DevilPileDecks.Add(temp1);
            GodPileDecks.Add(temp2);
        }
    }

    public int AddCardToPile(string sideToAddTo, CardPresenter card, int pileNumber)
    {
        lastMoved = null;
        howManyAdded += 1;
        if (sideToAddTo == "God")
        {
            GodPileDecks[pileNumber-1].Add(card);
            return GodPileDecks[pileNumber-1].Count - 1;
        }
        else {
            DevilPileDecks[pileNumber-1].Add(card);
            return DevilPileDecks[pileNumber-1].Count - 1;
        }
    }

    public CardPresenter GetCardFromPileTop(string sideToGetCardFrom, int pileNumber) 
    {
        CardPresenter card = null;
        try
        {
            if (sideToGetCardFrom == "God")
            {
                card = GodPileDecks[pileNumber-1][GodPileDecks[pileNumber-1].Count - 1];
            }
            else if (sideToGetCardFrom == "Devil")
            {
                card = DevilPileDecks[pileNumber-1][DevilPileDecks[pileNumber-1].Count - 1];
            }
            else if (sideToGetCardFrom == "Dancer")
            {
                card = Dancers[pileNumber-1];
            }
            //Debug.Log("Peaked card is " + card.data.Name + card.data.Strength);
            return card;
        }
        catch (ArgumentOutOfRangeException ex) {
            return null;
        }
    }

    public void CountPoints() {
        int pointsToGet = Dancers[currentAnimationPile].data.Strength;
        List<CardPresenter> devil = DevilPileDecks[currentAnimationPile];
        List<CardPresenter> god = GodPileDecks[currentAnimationPile];
        int forcesOfAirGod = 0;
        int forcesOfAirDevil = 0;
        int forcesOfFireGod = 0;
        int forcesOfFireDevil = 0;
        int twisterGod = 0;
        int twisterDevil = 0;
        int floodGod = 0;
        int floodDevil = 0;
        int tripodGod = 0;
        int tripodDevil = 0;
        foreach (CardPresenter card in devil)
        {
            int strength = card.data.Strength;
            if (card.data.Name == "Devil")
            {
                forcesOfAirDevil += strength;
            }
            else if (card.data.Name == "Kettle")
            {
                forcesOfFireDevil += strength;
            }
            else if (card.data.Name == "Twister")
            {
                twisterDevil += strength;
            }
            else if (card.data.Name == "Flood")
            {
                floodDevil += strength;
            }
            else if (card.data.Name == "Tripod") {
                tripodDevil += strength;
            }

        }

        foreach (CardPresenter card in god)
        {
            int strength = card.data.Strength;
            if (card.data.Name == "Angel")
            {
                forcesOfAirGod += strength;
            }
            else if (card.data.Name == "Lightning")
            {
                forcesOfFireGod += strength;
            }
            else if (card.data.Name == "Twister")
            {
                twisterGod += strength;
            }
            else if (card.data.Name == "Flood")
            {
                floodGod += strength;
            }
            else if (card.data.Name == "Tripod")
            {
                tripodGod += strength;
            }

        }

        if (twisterDevil > 0 || twisterGod > 0) {
            // cancels force of air
            forcesOfAirDevil = 0;
            forcesOfAirGod = 0;
        }

        if (floodDevil > 0 || floodGod > 0)
        {
            // cancels force of fire
            forcesOfFireDevil = 0;
            forcesOfFireGod = 0;
        }
        if (tripodDevil != tripodGod) {
            pointsToGet = 0;
        }

        if (pointsToGet > 0)
        {
            //print("points to get "+pointsToGet+" "+forcesOfAirGod + " " + forcesOfFireGod + " god, devil" + forcesOfAirDevil + " " + forcesOfFireDevil);

            if ((forcesOfAirGod + forcesOfFireGod) > (forcesOfAirDevil + forcesOfFireDevil))
            {
                GodPoints += pointsToGet;
                GodPointsText.color = GetPointsColor;
                DevilPointsText.color = Color.black;
            }
            else if ((forcesOfAirGod + forcesOfFireGod) < (forcesOfAirDevil + forcesOfFireDevil))
            {
                DevilPoints += pointsToGet;
                DevilPointsText.color = GetPointsColor;
                GodPointsText.color = Color.black;
            }
            else
            {
                // noone gets points right now for equal bets
                DevilPointsText.color = Color.black;
                GodPointsText.color = Color.black;
            }
        }
        else
        {
            // noone gets points right now for equal bets
            DevilPointsText.color = Color.black;
            GodPointsText.color = Color.black;
        }
    }

    private void AnimatePoints() {
        Dancers[currentAnimationPile].MakeVisible();
        foreach (CardPresenter card in DevilPileDecks[currentAnimationPile]) {
            card.MakeVisible();
        }
        foreach (CardPresenter card in GodPileDecks[currentAnimationPile])
        {
            card.MakeVisible();
        }
        CountPoints();
        GodPointsText.text = GodPoints.ToString();
        DevilPointsText.text = DevilPoints.ToString();


    }

}
