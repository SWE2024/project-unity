using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Country
{
    string Name;
    public Button Pointer; // reference to the pointer
    List<Country> Neighbors;
    Player Owner = null; // may not reflect the button's color
    int Troops = 0;

    public Country(Button button, string name)
    {
        this.Pointer = button;
        this.Name = name;
        this.Pointer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{this.Troops}";
        this.Pointer.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = this.Name;
    }

    public void SetNeighbors(List<Country> list)
    {
        if (Neighbors != null) return;
        Neighbors = list;
    }

    public List<Country> GetNeighbors() => Neighbors;

    public string GetName() => this.Name;

    public Player GetOwner() => this.Owner;

    public Color GetColor() => Owner.GetColor();

    public int GetTroops() => this.Troops;

    public void SetName(string name)
    {
        this.Name = name;
        this.Pointer.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = this.Name;
    }

    public void SetOwner(Player player)
    {
        if (Owner != null) Owner.RemoveCountry(this);
        this.Owner = player;
        player.AddCountry(this);

        Pointer.GetComponent<Image>().color = Owner.GetColor();
    }

    public void ChangeTroops(int offset)
    {
        this.Troops += offset;
        this.Pointer.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = $"{this.Troops}";
    }

    // this is for changing button color for Highlighting either to black or white
    private void TempColorChange(Color color)
    {
        Pointer.GetComponent<Image>().color = color;
    }

    // this is to undo the Highlighting so change to the Owner color from either black or white
    public void ReverseColorChange()
    {
        Pointer.GetComponent<Image>().color = this.Owner.GetColor();
    }

    // Highlights the color to grey 
    // also Highlights the Attackable countries which are neighboring countries that do not have the same color as itself
    // returns this Attackable countries in a list for the gameobject instance to handle states
    public List<Country> HighlightEnemyNeighbours()
    {
        TempColorChange(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in GetNeighbors())
        {
            if (neighbor.Owner == this.Owner) continue;

            neighbor.TempColorChange(Color.white);
            output.Add(neighbor);
        }
        return output;
    }

    public List<Country> HighlightFriendlyNeighbours()
    {
        TempColorChange(Color.grey);

        List<Country> output = new List<Country>();

        foreach (Country neighbor in GetNeighbors())
        {
            if (neighbor.Owner != this.Owner) continue;

            neighbor.TempColorChange(Color.white);
            output.Add(neighbor);
        }
        return output;
    }
}