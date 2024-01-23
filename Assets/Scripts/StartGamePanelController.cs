using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGamePanelController : MonoBehaviour
{
    public GameObject StartPanel;
    public GameObject ChooseCharacterPanel;

    public GameObject NextRule;
    public TextMeshProUGUI NexRuleText;

    public Image Background;
    public Sprite[] RulesImages;
    public Sprite BasicBackground;
    private int ruleNumber = 0;


    void Start()
    {
        ruleNumber = 0;
        StartPanel.SetActive(true);
        NextRule.SetActive(false);
        ChooseCharacterPanel.SetActive(false);
        Background.sprite = BasicBackground;
    }

    void Update()
    {
        
    }
    public void StartGamePressed() 
    {
        StartPanel.SetActive(false);
        ChooseCharacterPanel.SetActive(true);
    }

    public void RulesPressed() {
        StartPanel.SetActive(false);
        ChooseCharacterPanel.SetActive(false);
        NextRule.SetActive(true);
        NextRulePressed();
    }

    private Sprite GetNextRule() {
        if (ruleNumber < RulesImages.Length) {
            Sprite image = RulesImages[ruleNumber];
            ruleNumber++;
            return image;
        }
        else { return null; }
    }

    public void NextRulePressed() {
        Sprite image = GetNextRule();
        if (image != null)
        {
            Background.sprite = image;
            NexRuleText.text = "Next rule";
            if (ruleNumber >= RulesImages.Length)
            {
                NexRuleText.text = "Back to Start Menu";
            }
        }
        else {
            ruleNumber = 0;
            Start();
            //SceneManager.LoadScene(0);
        }
    }


    public void GodCharacterPressed() 
    {
        StartGame.Instance.SetCharacters("God", "Devil");
    }

    public void DevilCharacterPressed()
    {
        StartGame.Instance.SetCharacters("Devil", "God");

    }
}
