using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MuteController : MonoBehaviour
{
    public Sprite mute;
    public Sprite unmute;

    public void Start()
    {
        if (AudioManagment.Instance.isMuted)
        {
            gameObject.GetComponent<Image>().sprite = unmute;
        }
        else
        {
            gameObject.GetComponent<Image>().sprite = mute;
        }
    }

    // Start is called before the first frame update
    public void ToggleMute(Button button)
    {
        bool isMuted = !AudioManagment.Instance.isMuted;
        AudioManagment.Instance.isMuted = isMuted;
        AudioSource[] audioSources = FindObjectsOfType<AudioSource>();
        foreach (AudioSource audioSource in audioSources)
        {
            if (audioSource.gameObject.name == "BackGroundMusic") {
                audioSource.mute = isMuted;
            }
        }

        // Change the button text to reflect the state.
        if (isMuted)
        {
            button.GetComponent<Image>().sprite = unmute;
            //button.GetComponentInChildren<TextMeshProUGUI>().text = "Unmute";
        }
        else
        {
            button.GetComponent<Image>().sprite = mute;
            //button.GetComponentInChildren<TextMeshProUGUI>().text = "Mute";
        }
    }
}
