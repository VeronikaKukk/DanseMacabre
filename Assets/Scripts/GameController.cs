using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameController : MonoBehaviour
{
    public static GameController Instance;
    public GameObject Devil;
    public GameObject God;

    public string player;
    public string ai;

    public GameObject EndScreenCanvas;
    public GameObject WinScreen;
    public GameObject LoseScreen;

    public GameObject StartGameButton;
    public GameObject EndTurnButtonGod;
    public GameObject EndTurnButtonDevil;

    public TextMeshProUGUI InfoText;

    public void Awake()
    {
        Instance = this;
        Events.OnEndGame += OnEndGame;
        Events.OnTurnStart += OnTurnStart;
        Events.OnStartCountingPoints += OnStartCountingPoints;

        SetCharacters();
    }
    void Start()
    {
        SetCharacterIcons();
        EndTurnButtonGod.SetActive(false);
        EndTurnButtonDevil.SetActive(false);
        InfoText.text = "";
    }

    public void OnDestroy()
    {
        Events.OnEndGame -= OnEndGame;
        Events.OnTurnStart -= OnTurnStart;
        Events.OnStartCountingPoints -= OnStartCountingPoints;
    }

    private void SetCharacterIcons() 
    {
        GameObject deviltoken = GameObject.Find("deviltoken");
        GameObject angeltoken = GameObject.Find("angeltoken");
        if (player == "God")
        {
            angeltoken.transform.localPosition = new Vector3(-330f, -60f, 0);
            deviltoken.transform.localPosition = new Vector3(-330f, 60f, 0);
        }
        else {
            deviltoken.transform.localPosition = new Vector3(-330f, -60f, 0);
            angeltoken.transform.localPosition = new Vector3(-330f, 60f, 0);
        }
    }

    private void SetCharacters() {
        if (PlayerPrefs.HasKey("PlayerCharacter"))
        {
            player = PlayerPrefs.GetString("PlayerCharacter");
        }

        if (PlayerPrefs.HasKey("AICharacter"))
        {
            ai = PlayerPrefs.GetString("AICharacter");
        }

        if (player == "") {
            player = "God";
            ai = "Devil";
        }

        SetTable();
    }

    private void SetTable() 
    {
        if (player == "God")
        {
            God.transform.position = new Vector3(1.75f, -4f, 0f);
            Devil.transform.position = new Vector3(1.75f, 4f, 0f);
            Transform devilcan = Devil.transform.Find("Canvas");
            Transform godcan = God.transform.Find("Canvas");

            GameObject snappoint = devilcan.Find("PlayerSnapPoints").gameObject;
            snappoint.transform.localPosition = new Vector3(-2f, 1f, 0f);

            devilcan.transform.Find("Panel").transform.localPosition = new Vector3(0f,-0.25f,0f);
            godcan.transform.Find("Panel").transform.localPosition = new Vector3(0f, 0.25f, 0f);


        }
        else {
            Devil.transform.position = new Vector3(1.75f, -4f, 0f);
            God.transform.position = new Vector3(1.75f, 4f, 0f);
            Transform godcan = God.transform.Find("Canvas");
            Transform devilcan = Devil.transform.Find("Canvas");

            GameObject snappoint = godcan.Find("PlayerSnapPoints").gameObject;
            snappoint.transform.localPosition = new Vector3(-2f, 1f, 0f);

            godcan.transform.Find("Panel").transform.localPosition = new Vector3(0f, -0.25f, 0f);
            devilcan.transform.Find("Panel").transform.localPosition = new Vector3(0f, 0.25f, 0f);

        }
    }

    public void BeginGame() 
    {
        Events.TurnStart("God", 1);
        StartGameButton.SetActive(false);
    }
    public void PlayAgain() {
        SceneManager.LoadScene(0);
    }

    public void GoToSite() {
        Application.OpenURL("https://revaler.ee/toode/danse-macabre/");
    }

    public void OnTurnStart(string playerName, int turnStage) {
        if (!PilesController.Instance.isCountingTime)
        {
            //Debug.Log("current player " + playerName + " stage is " + turnStage);

            if (playerName == "God" && player == playerName)
            {
                InfoText.text = "Your turn";
                EndTurnButtonGod.SetActive(true);
                EndTurnButtonDevil.SetActive(false);
            }
            else if (playerName == "Devil" && player == playerName)
            {
                InfoText.text = "Your turn";
                EndTurnButtonGod.SetActive(false);
                EndTurnButtonDevil.SetActive(true);
            }
            else
            {
                InfoText.text = "Opponent's turn";
                EndTurnButtonGod.SetActive(false);
                EndTurnButtonDevil.SetActive(false);
            }
        }
    }

    public void OnEndGame(string winner) {
        bool isWin = false;
        if(winner == player) { isWin = true; }

        EndScreenCanvas.SetActive(true);
        if (isWin)
        {
            WinScreen.SetActive(true);
            LoseScreen.SetActive(false);
        }
        else {
            LoseScreen.SetActive(true);
            WinScreen.SetActive(false);
        }
    }

    private void OnStartCountingPoints() {
        EndTurnButtonDevil.SetActive(false);
        EndTurnButtonGod.SetActive(false);
        InfoText.text = "Counting points";
        GameObject deviltoken = GameObject.Find("deviltoken");
        deviltoken.GetComponent<Image>().enabled = true;
        GameObject angeltoken = GameObject.Find("angeltoken");
        angeltoken.GetComponent<Image>().enabled = true;

    }
}
