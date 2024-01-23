using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIController : MonoBehaviour
{
    public static AIController Instance;
    public string Owner;
    public string Opponent;

    public GameObject[] DevilPiles;
    public GameObject[] GodPiles;

    public bool MoveCompleted;
    public Color LastMoveColor;

    private CardPresenter cardToMove;
    private Transform pileToMoveTo;

    private Dictionary<int, List<CardPresenter>> ownerCardPiles;
    private Dictionary<int,int> peakedDancers;
    private Dictionary<int, List<CardPresenter>> previouslyPeakedOpponentCards;
    private List<int> invisibleDancerValues;

    AudioSource snapToPileSound;
    public void Awake()
    {
        snapToPileSound = GetComponent<AudioSource>();
        Instance = this;
    }
    void Start()
    {
        Owner = GameController.Instance.ai;
        Opponent = GameController.Instance.player;
        // create empty lists
        previouslyPeakedOpponentCards = new Dictionary<int, List<CardPresenter>>();
        ownerCardPiles = new Dictionary<int, List<CardPresenter>>();
        peakedDancers = new Dictionary<int, int>
        {
            // add public dancers
            { 2, 6 },
            { 4, 5 },
            { 6, 4 },
            { 8, 3 },
            { 10, 2 }
        };
        invisibleDancerValues = new List<int>
        {
            0, 5, 7, 6, 4, 8
        };


        if (Owner == "God")
        {
            foreach (GameObject pile in GodPiles)
            {
                pile.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
            }
        }
        else {
            foreach (GameObject pile in DevilPiles)
            {
                pile.GetComponent<BoxCollider2D>().size = new Vector2(1f, 1f);
            }
        }
    }
    void Update()
    {
        if (cardToMove != null && !MoveCompleted) 
        {
            if (Vector3.Distance(cardToMove.transform.position, pileToMoveTo.position) < 0.001f)
            {
                cardToMove.transform.SetParent(pileToMoveTo.transform);
                cardToMove = null;
                pileToMoveTo = null;
                MoveCompleted = true;
                snapToPileSound.Play();
            }
            else {
                cardToMove.transform.position = Vector3.MoveTowards(cardToMove.transform.position, pileToMoveTo.position, 8.0f * Time.deltaTime);
            }
        }

    }

    public void PlayCardToPile(List<CardPresenter> cards) 
    {
        Tuple<int, int> chosenPileAndCard = null;
        
        chosenPileAndCard = ChooseBest(cards);

        if (chosenPileAndCard == null)
        {
            int randPile = Random.Range(0, GodPiles.Length);
            int randCard = Random.Range(0, cards.Count);
            chosenPileAndCard = new Tuple<int,int>(randPile, randCard);
        }

        int chosenPileIdx = chosenPileAndCard.Item1;
        CardPresenter card = cards[chosenPileAndCard.Item2];

        GameObject pile = null;
        PilesController.Instance.lastMoved = card;
        Draggable drag = card.GetComponent<Draggable>();

        if (Owner == "God")
        {
            pile = GodPiles[chosenPileIdx];
        }
        else {
            pile = DevilPiles[chosenPileIdx];
        }

        drag.SetCardPileNumber(int.Parse(pile.name.Substring(4)));
        drag.isInPile = true;
        pileToMoveTo = pile.transform;
        cardToMove = card;
        card.GetComponent<SpriteRenderer>().color = LastMoveColor;

        List<CardPresenter> cardsInPile = null;
        if (ownerCardPiles.ContainsKey(drag.cardPileNumber))
        {
            cardsInPile = ownerCardPiles[drag.cardPileNumber];
        }
        else {
            cardsInPile = new List<CardPresenter>();
        }
        cardsInPile.Add(card);
        ownerCardPiles.Remove(drag.cardPileNumber);// remove previuous 
        ownerCardPiles.Add(drag.cardPileNumber, cardsInPile); // add updated
    }

    public Tuple<int, int> ChooseBest(List<CardPresenter> cards) {
        List<List<CardPresenter>> opponentUnseenPiles = PilesController.Instance.DevilPileDecks;
        if (Opponent == "God") {
            opponentUnseenPiles = PilesController.Instance.GodPileDecks;
        }
        List<(float Score, int PileNumber, int CardNumber)> goodTurns = new List<(float, int, int)>();
        
        for (int i = 0; i < cards.Count; i++) {
            CardPresenter observableCard = cards[i];
            for (int j = 0; j < GodPiles.Length; j++) {
                int pileNumber = j + 1;

                int opponentUnseenPileSize = opponentUnseenPiles[j].Count;
                int opponentSeenPileSize = 0;
                if (previouslyPeakedOpponentCards.ContainsKey(pileNumber)) {
                    opponentSeenPileSize = previouslyPeakedOpponentCards[pileNumber].Count;
                }
                
                float score = 1;

                if (ownerCardPiles.ContainsKey(pileNumber))
                {
                    if (ownerCardPiles[pileNumber].Count >= 1 && PilesController.Instance.howManyAdded < Random.Range(10, 15))
                    {
                        score -= 0.7f;
                    }
                    else if (ownerCardPiles[pileNumber].Count >= 2 && PilesController.Instance.howManyAdded < Random.Range(22, 28))
                    {
                        score -= 0.8f;
                    }
                    
                    
                }

                int pilePoints = 0;

                if (peakedDancers.ContainsKey(pileNumber)) {
                    pilePoints = peakedDancers[pileNumber];
                }

                score -= (0.8f - ((float)pilePoints) / 10);

                // check that you dont put 3 strength card to low value pile
                if (observableCard.data.Strength > 2 && pilePoints < 6) {
                    score -= 0.5f;
                }

                if (pilePoints == 0)
                {
                    score -= 10f;
                }
                
                bool hasFlood = false;
                bool hasTwister = false;
                bool hasTripod = false;
                int opponentAirStrength = 0;
                int ownAirStrength = 0;
                int opponentFireStrength = 0;
                int ownFireStrength = 0;

                if (previouslyPeakedOpponentCards.ContainsKey(pileNumber))
                {
                    foreach (CardPresenter card in previouslyPeakedOpponentCards[pileNumber])
                    {
                        if (card.data.Name == "Angel" || card.data.Name == "Devil")
                        {
                            opponentAirStrength += card.data.Strength;
                        }
                        else if (card.data.Name == "Flood")
                        {
                            hasFlood = true;
                        }
                        else if (card.data.Name == "Twister")
                        {
                            hasTwister = true;
                        }
                        else if (card.data.Name == "Kettle" || card.data.Name == "Lightning")
                        {
                            opponentFireStrength += card.data.Strength;
                        }
                        else if (card.data.Name == "Tripod")
                        {
                            hasTripod = true;
                        }
                    }
                }

                if (ownerCardPiles.ContainsKey(pileNumber))
                {
                    foreach (CardPresenter card in ownerCardPiles[pileNumber])
                    {
                        if (card.data.Name == "Angel" || card.data.Name == "Devil")
                        {
                            ownAirStrength += card.data.Strength;
                        }
                        else if (card.data.Name == "Flood")
                        {
                            hasFlood = true;
                        }
                        else if (card.data.Name == "Twister")
                        {
                            hasTwister = true;
                        }
                        else if (card.data.Name == "Kettle" || card.data.Name == "Lightning")
                        {
                            ownFireStrength += card.data.Strength;
                        }
                        else if (card.data.Name == "Tripod")
                        {
                            hasTripod = true;
                        }
                    }
                }

                if (hasFlood) {
                    ownFireStrength = 0;
                    opponentFireStrength = 0;
                }
                if (hasTwister) {
                    ownAirStrength = 0;
                    opponentAirStrength = 0;
                }

                if (observableCard.data.Name == "Angel" || observableCard.data.Name == "Devil")
                {
                    if (hasTwister)
                    {
                        score -= 10f;
                    }
                    if (hasTripod)
                    {
                        score -= 10f;
                    }

                    if (opponentUnseenPileSize == 0 && observableCard.data.Strength == 1 && pilePoints != 0)
                    {
                        score += 2f;
                    }
                    else if (opponentUnseenPileSize == 0 && observableCard.data.Strength == 2 && pilePoints != 0) {
                        score += 1f;
                    }

                    if (opponentUnseenPileSize - opponentSeenPileSize > 1) {
                        score -= 0.3f;
                    }

                    if (opponentAirStrength - ownAirStrength > observableCard.data.Strength && pilePoints < 4)
                    {
                        score -= 0.6f;
                    } 
                    else if (opponentAirStrength < ownAirStrength)
                    {// if it is already winning, no need to add
                        score -= 0.7f;
                    }
                    else if (opponentAirStrength == ownAirStrength + observableCard.data.Strength && pilePoints < 5)
                    {
                        score -= 0.6f;
                    }
                    else if (ownAirStrength - opponentAirStrength > 0) {
                        score -= 0.6f;
                    }
                    
                }
                else if (observableCard.data.Name == "Kettle" || observableCard.data.Name == "Lightning")
                {
                    if (hasFlood)
                    {
                        score -= 10f;
                        
                    }
                    if (hasTripod)
                    {
                        score -= 10f;
                        
                    }

                    if (opponentUnseenPileSize == 0 && observableCard.data.Strength == 1 && pilePoints != 0)
                    {
                        score += 2f;
                    }
                    else if (opponentUnseenPileSize == 0 && observableCard.data.Strength == 2 && pilePoints != 0)
                    {
                        score += 1f;
                    }

                    if (opponentUnseenPileSize - opponentSeenPileSize >1)
                    {
                        score -= 0.3f;
                    }

                    if (opponentFireStrength - ownFireStrength > observableCard.data.Strength && pilePoints < 4)// if adding card wont help be stronger
                    {
                        score -= 0.6f;
                    }
                    else if (opponentFireStrength < ownFireStrength)
                    {// if it is already winning, no need to add
                        score -= 0.7f;
                    }
                    else if (opponentFireStrength == ownFireStrength + observableCard.data.Strength && pilePoints < 5)
                    {
                        score -= 0.6f;
                    }
                    else if (ownFireStrength - opponentFireStrength > 0)
                    {
                        score -= 0.6f;
                    }
                }
                else if (observableCard.data.Name == "Twister")
                {
                    if (hasTwister)
                    {
                        score -= 10f;
                    }
                    if (hasTripod)
                    {
                        score -= 10f;
                    }

                    if (ownAirStrength > opponentAirStrength)
                    {
                        score -= 0.9f;
                    }
                    if (opponentAirStrength < 2)
                    {
                        score -= 0.7f;
                    }

                    if (PilesController.Instance.howManyAdded < 15) {
                        score -= 2f;
                    }

                    if (opponentUnseenPileSize - opponentSeenPileSize > 1)
                    {
                        score -= 0.7f;
                    }

                }
                else if (observableCard.data.Name == "Flood")
                {
                    if (hasFlood)
                    {
                        score -= 1f;
                    }
                    if (hasTripod)
                    {
                        score -= 1f;
              
                    }

                    if (ownFireStrength > opponentFireStrength)
                    {
                        score -= 0.9f;
                    }
                    if (opponentFireStrength < 2)
                    {
                        score -= 0.8f;
                    }
                    if (PilesController.Instance.howManyAdded < 15)
                    {
                        score -= 2f;
                    }

                    if (opponentUnseenPileSize - opponentSeenPileSize > 2) {
                        score -= 0.7f;
                    }
                }
                else if (observableCard.data.Name == "Tripod") {
                    if (hasFlood && hasTwister) {
                        score -= 10f;
                 
                    }
                    if (hasFlood) {
                        score -= 0.7f;
                    }
                    if (hasTwister) {
                        score -= 0.7f;
                    }
                    if (hasTripod) {
                        score -= 10f;
                    }

                    if (opponentUnseenPileSize - opponentSeenPileSize > 0)
                    {
                        score -= 0.4f;
                    }

                    if (opponentAirStrength + opponentFireStrength < 2) {
                        score -= 0.8f;
                    }

                    if (PilesController.Instance.howManyAdded < 20)
                    {
                        score -= 1f;
                    }
                    if (pilePoints < 3) {
                        score -= 0.6f;
                    }
                    
                }

                
                goodTurns.Add((score, j, i));
            }
        }


        goodTurns.Sort((x, y) => y.Score.CompareTo(x.Score));
        List<Tuple<int, int>> bestPiles = new List<Tuple<int, int>>();
        float biggestScore = goodTurns[0].Score;
        for (int k = 0; k < goodTurns.Count; k++)
        {
            if (goodTurns[k].Score >= biggestScore)
            {
                bestPiles.Add(new Tuple<int,int>(goodTurns[k].PileNumber, goodTurns[k].CardNumber));
            }
        }

        if (bestPiles.Count > 0)
        {
            int randomNumber = Random.Range(0, bestPiles.Count);
            Tuple<int, int> best = bestPiles[randomNumber];
            return bestPiles[randomNumber];
        }
        return null;
    }


    public Tuple<string,int> PeakACard() {
        // randomly choose to either peak a dancer or opponent cards, piles are chosen based on whether
        // you have already looked at them and if there is any new info, if all dancers are peaked
        // then always choose opponent cards to peak
        List<List<CardPresenter>> opponentPiles = null;
        List<CardPresenter> dancerPiles = null;
        float minValue = 0f;
        if (PilesController.Instance.howManyAdded < 14) {
            minValue = 0.3f;
        }

        float whichPileToLook = Random.Range(minValue, 1f);
        if(peakedDancers.Count >= 11)
        {
            whichPileToLook = 0.1f;
        }
        string chosenSideName = "";

        if (Owner == "God")
        {
            opponentPiles = PilesController.Instance.DevilPileDecks;

        }
        else
        {
            opponentPiles = PilesController.Instance.GodPileDecks;
        }

        if (whichPileToLook < 0.5f)
        {
            chosenSideName = Opponent;
        }
        else {

            dancerPiles = PilesController.Instance.GetDancers();
            chosenSideName = "Dancer";
        }

        List<(float Score, int PileNumber)> pileChoices = new List<(float, int)>();

        if (chosenSideName == Opponent)
        {
            for (int i = 0; i < opponentPiles.Count; i++)
            {
                float score = 0;
                if (opponentPiles[i].Count > 0)// if there are more than 0 cards
                {
                    if (peakedDancers.ContainsKey(i + 1))
                    {
                        score += peakedDancers[i+1] / 10;
                    }

                    if (previouslyPeakedOpponentCards.ContainsKey(i + 1) && previouslyPeakedOpponentCards[i + 1].Count < opponentPiles[i].Count)
                    {
                        score += (opponentPiles[i].Count - previouslyPeakedOpponentCards[i + 1].Count)/10;
                    }
                    else if (!previouslyPeakedOpponentCards.ContainsKey(i + 1))
                    {
                        score += 0.4f;
                    }

                    pileChoices.Add((score,i));
                }
            }
        }
        else if (chosenSideName == "Dancer")
        {
            for (int i = 0; i < dancerPiles.Count; i++)
            {
                float score = 0;

                if (!peakedDancers.ContainsKey(i + 1) && opponentPiles[i].Count > 3)
                {
                    score += 0.9f;
                }
                else if (!peakedDancers.ContainsKey(i + 1) && opponentPiles[i].Count > 2)
                {
                    score += 0.8f;
                }
                else if (!peakedDancers.ContainsKey(i + 1) && opponentPiles[i].Count > 1) {
                    score += 0.7f;
                }
                else if (!peakedDancers.ContainsKey(i + 1))
                {
                    score += 0.6f;
                }

                if (!peakedDancers.ContainsKey(i + 1)) {
                    pileChoices.Add((score, i));
                }
            }
        }

        if (pileChoices.Count == 0) {
            chosenSideName = Opponent;
            for (int i = 0; i < opponentPiles.Count; i++)
            {
                if (opponentPiles[i].Count > 0) { pileChoices.Add((0.1f, i)); }
            }
        }

        pileChoices.Sort((x, y) => y.Score.CompareTo(x.Score));
        List<int> bestPiles = new List<int>();
        float biggestScore = pileChoices[0].Score;
        for(int i = 0; i < pileChoices.Count; i++) {
            if (pileChoices[i].Score >= biggestScore) {
                bestPiles.Add(pileChoices[i].PileNumber);
            }
        }

        int randomPileIdx = Random.Range(0,bestPiles.Count);
        int Idx = bestPiles[randomPileIdx] + 1;
        MoveCompleted = true;
        return new Tuple<string, int>(chosenSideName, Idx);

    }

    public void AddPreviouslyPeakedOpponentCard(CardPresenter peakedCard, int pileNumber) {
        List<CardPresenter> cardsInPile = null;
        if (previouslyPeakedOpponentCards.ContainsKey(pileNumber))
        {
            cardsInPile = previouslyPeakedOpponentCards[pileNumber];
        }
        else {
            cardsInPile = new List<CardPresenter>();
        }
        cardsInPile.Add(peakedCard);
        previouslyPeakedOpponentCards.Remove(pileNumber);
        previouslyPeakedOpponentCards.Add(pileNumber,cardsInPile);
    }
    public void AddPeakedDancers(int points, int pileNumber) {
        invisibleDancerValues.Remove(points);
        peakedDancers.Add(pileNumber, points);
        if(peakedDancers.Count == 10) {
            for (int i = 0; i < 11; i++) {
                if(!peakedDancers.ContainsKey(i+1)) {
                    peakedDancers.Add(i + 1, invisibleDancerValues[0]);
                    break;
                }
            }
        }
    }
    
}
