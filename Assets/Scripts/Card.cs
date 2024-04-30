using UnityEngine;
public class Card
{
    Country parent;
    string type;
    Sprite sprite;

    public Card(Country c, string card_type, Sprite s)
    {
        parent = c;
        type = card_type;
        sprite = s;
    }

    public string GetCardType()
    {
        return type;
    }

    public Sprite GetSprite()
    {
        return sprite;
    }
}
