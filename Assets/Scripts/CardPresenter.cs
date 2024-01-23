using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;
using UnityEngine.UI;

public class CardPresenter : MonoBehaviour
{
    public Card data;
    public Image image;

    public Sprite GodBack;
    public Sprite DevilBack;

    public Sprite DancerBack;

    private string characterName;

    private Vector3 originalScale;
    private int originalSort;

    public void Awake()
    {
        image = GetComponent<Image>();
    }
    public void init(Card data, bool isVisible, string character) 
    { 
        this.data = data;
        characterName = character;
        if (isVisible)
        {
            GetComponent<SpriteRenderer>().sprite = data.Image;
        }
        else {
            if (character == "God")
            {
                GetComponent<SpriteRenderer>().sprite = GodBack;
            }
            else if(character == "Devil"){
                GetComponent<SpriteRenderer>().sprite = DevilBack;

            }else if (character == "Dancer"){
                GetComponent<SpriteRenderer>().sprite = DancerBack;
            }
        }
    }

    public void MakeVisible() {
        originalScale = transform.localScale;
        GetComponent<SpriteRenderer>().sprite = data.Image;
        originalSort = GetComponent<SpriteRenderer>().sortingOrder;
        GetComponent<SpriteRenderer>().sortingOrder = 10;
    }

    public void MakeInvisible() {
        if (characterName == "God" && GameController.Instance.player != "God")
        {
            GetComponent<SpriteRenderer>().sprite = GodBack;
        }
        else if (characterName == "Devil" && GameController.Instance.player != "Devil")
        {
            GetComponent<SpriteRenderer>().sprite = DevilBack;

        }
        else if (characterName == "Dancer")
        {
            GetComponent<SpriteRenderer>().sprite = DancerBack;
        }
        GetComponent<SpriteRenderer>().sortingOrder = originalSort;

    }

    public void MakeBigger() {
        transform.localScale = new Vector3(originalScale.x*1.3f, originalScale.y * 1.3f, originalScale.z * 1.3f);

    }

    public void MakeSmaller() {
        transform.localScale = originalScale;
    }
}
