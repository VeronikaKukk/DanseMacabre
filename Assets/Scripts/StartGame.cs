using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartGame : MonoBehaviour
{
    public static StartGame Instance;
    public string PlayerCharacter;
    public string AICharacter;

    private void Awake()
    {
        Instance = this; 
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void SetCharacters(string player, string ai) 
    {
        PlayerPrefs.SetString("PlayerCharacter", player);
        //Debug.Log("Player is " + PlayerPrefs.GetString("PlayerCharacter"));
        PlayerPrefs.SetString("AICharacter", ai);
        LoadGame();
    }
    public void LoadGame() 
    {
        SceneManager.LoadScene(1);
    }
}
