using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/CardData")]
public class Card : ScriptableObject
{
    public string Name;
    public int Strength;
    public Sprite Image;
}
