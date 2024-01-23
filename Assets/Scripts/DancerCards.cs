using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DancerCards : MonoBehaviour
{
    public List<Card> HiddenDancers;
    public List<Card> VisibleDancers;

    public CardPresenter CardPresenterPrefab;
    public GameObject CardPanel;

    public List<CardPresenter> presenters;
    void Start()
    {
        CardPanel.GetComponent<HorizontalLayoutGroup>().spacing = 0.44f;
        ShuffleHiddenDancers();
        presenters = new List<CardPresenter>();
        int j = 0;
        int k = 0;
        for (int i = 0; i < HiddenDancers.Count + VisibleDancers.Count; i++) {
            if (i % 2 == 0)
            {
                bool isVisible = false;
                CardPresenter presenter = Instantiate<CardPresenter>(CardPresenterPrefab, CardPanel.transform);
                presenter.name = "Dancer" + (i+1);
                presenter.init(HiddenDancers[j], isVisible, "Dancer");
                j++;
                presenters.Add(presenter);
            }
            else {
                bool isVisible = true;
                CardPresenter presenter = Instantiate<CardPresenter>(CardPresenterPrefab, CardPanel.transform);
                presenter.name = "Dancer" + (i + 1);
                presenter.init(VisibleDancers[k], isVisible, "Dancer");
                k++;
                presenters.Add(presenter);
            }
        }

        PilesController.Instance.AddDancers(presenters);
    }

    void Update()
    {
        
    }

    // chatgpt helped with shuffle
    public void ShuffleHiddenDancers()
    {
        for (int i = HiddenDancers.Count - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            Card temp = HiddenDancers[i];
            HiddenDancers[i] = HiddenDancers[j];
            HiddenDancers[j] = temp;
        }
    }
}
