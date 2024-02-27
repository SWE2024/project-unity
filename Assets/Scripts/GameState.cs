
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.UI;


public enum Country_State {
    none,
    highlighted,
    considered,
}
public class GameState 
{
    List<Country> list_of_countries;
    public static System.Random random = new System.Random();
    Country highlighted = null;
    List<Country> considered = null;
    UnityEngine.Color turn_color;
    SpriteRenderer square;
    List<UnityEngine.Color> turns_order;
    int turn_index = 0;

    public GameState(List<Country> list) {
        this.list_of_countries = list;
        this.turns_order = GameState.create_turns();        
        this.turn_color = this.turns_order[0];
        this.square = GameObject.Find("Square").GetComponent<SpriteRenderer>();
        this.square.color = turn_color;
    }

    private static List<Color> create_turns() {
        List<int> list = new List<int> {0 , 1, 2};

        List<int> randomized = new List<int>();

        while (list.Count > 0) {
            int index = GameState.random.Next(list.Count);
            randomized.Add(list[index]);
            list.RemoveAt(index);
        }

        List<Color> output = new List<Color>();

        foreach (int index in randomized) {
            output.Add(GameState.int_to_color(index));
        }

        return output;
    }




    public static Color int_to_color(int num) 
    {
        switch (num) 
        {
            case 0:
                return Color.green;
            case 1:
                return Color.blue;
            case 2:
                return Color.red;
            default:
                return Color.white;
        }
    }



    public void take_country_click(Country country) {
        //this handles highlighting (if nothing is highlighted)
        if (this.highlighted == null) {
            // this handles the case that the clicked country's owner doesn't much turn's player
            if (this.turn_color != country.color) return;
            this.highlight(country);
            return;
        }  

        //from here onwards is when smth is highlighted

        //check if the country being clicked is attackable, positive index is true, else is false
        int index = this.considered.IndexOf(country);

        if (index < 0) {
            this.unhighlight();
            return;
        }


        this.attack(index);

        this.next_turn();

        return;
    }

    public void highlight(Country country) {
        this.highlighted = country;
        this.considered = country.highlight();
        return;
    }

    public void unhighlight() {
        this.highlighted.reverse_color_change();

        foreach (Country country in this.considered) {
            country.reverse_color_change();
        }

        this.highlighted = null;
        this.considered = null;
    }

    public void attack(int index) {
        Country attacked = this.considered[index];
        //remove the attacked country from considered 
        this.considered.RemoveAt(index);
        
        //change the attacked country color
        attacked.change_country_color(this.turn_color);

        //unhighlight the rest
        this.unhighlight();
    }

    public void next_turn() {

        this.turn_index++;

        if (this.turn_index > (this.turns_order.Count - 1)) {
            this.turn_index = 0;
        }

        Debug.Log($"the turn now is {this.turn_index}");

        this.turn_color = this.turns_order[this.turn_index];

        this.square.GetComponent<SpriteRenderer>().color = this.turn_color;
    }
}