using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class AIPlayer : Player
{
    public AIPlayer(string name, Color color) : base(name, color) { }

    override public void TakeTurn()
    {
        if (GameController.Get().flagSetupPhase) // AI takes a setup claiming a single country
        {
            Wait.Start(1f, () => // wait 0 to 2 seconds to take a country so it looks like a real player
            {
                // IMPORTANT! the following line prevents resource wastage / crashing
                // do not randomly look for a country if not many are left unclaimed
                if (GameController.Get().populatedCountries >= GameController.Get().countryMap.Count - 5)
                {
                    foreach (var v in GameController.Get().countryMap)
                    {
                        if (v.Value.GetOwner() == null)
                        {
                            v.Value.SetOwner(this);
                            v.Value.ChangeTroops(1);
                            this.ChangeNumberOfTroops(-1);
                            GameController.Get().populatedCountries++;

                            Killfeed.Update($"{this.GetName()}: now owns {v.Value.GetName()}");

                            if (GameController.Get().populatedCountries == GameController.Get().countryMap.Count)
                            {
                                GameController.Get().currentPhase.text = "deploy phase";
                                GameController.Get().HandleObjectClick = GameController.Get().SetupDeployPhase;
                                GameController.Get().flagSetupPhase = false;
                                GameController.Get().flagSetupDeployPhase = true;
                                GameController.Get().ResetTurn();
                                return;
                            }

                            GameController.Get().NextTurn();
                            return;
                        }
                    }
                }
                else
                {
                    while (true) // loops until finding an unowned country
                    {
                        var kvp = GameController.Get().countryMap.ElementAt(Random.Range(0, GameController.Get().countryMap.Count - 1));

                        if (kvp.Value.GetOwner() == null)
                        {
                            kvp.Value.SetOwner(this);
                            kvp.Value.ChangeTroops(1);
                            this.ChangeNumberOfTroops(-1);
                            GameController.Get().populatedCountries++;

                            Killfeed.Update($"{this.GetName()}: now owns {kvp.Value.GetName()}");

                            GameController.Get().NextTurn();
                            return;
                        }
                    }
                }
            });
        }
        else if (GameController.Get().flagSetupDeployPhase) // AI takes a setup deploy turn
        {
            Wait.Start(Random.Range(2, 3), () => // wait 2 to 4 seconds to take a country so it looks like a real player
            {
                List<Country> ownedCountries = this.GetCountries();
                Country selected = ownedCountries.ElementAt(Random.Range(0, ownedCountries.Count - 1));

                int troops = this.GetNumberOfTroops();
                selected.ChangeTroops(troops);
                this.ChangeNumberOfTroops(-troops);

                Killfeed.Update($"{this.GetName()}: sent {troops} troop(s) to {selected.GetName()}");

                GameController.Get().NextTurn();

                if (GameController.Get().turnPlayer.GetNumberOfTroops() == 0) // next player has no troops to deploy
                {
                    GameController.Get().currentPhase.text = "draft phase";
                    GameController.Get().flagSetupDeployPhase = false;
                    GameController.Get().flagFinishedSetup = true;
                    GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
                    GameController.Get().ResetTurn(); // AI agent is never first player, do not worry about this

                    GameObject.Find("EndPhase").GetComponent<Image>().enabled = true;
                    GameObject.Find("EndPhase").GetComponent<Button>().enabled = true;
                }

                return;
            });
        }
        else // AI player takes a normal turn
        {
            GameController.Get().currentPhase.text = "draft phase";
            GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
            Wait.Start(Random.Range(2, 3), () => // wait 2 to 3 seconds to draft to a country so it looks like a real player
            {
                List<Country> ownedCountries = this.GetCountries();

                Country selected = ownedCountries.ElementAt(Random.Range(0, ownedCountries.Count - 1));
                int troops = this.GetNumberOfTroops();
                selected.ChangeTroops(troops);
                this.ChangeNumberOfTroops(-troops);

                Killfeed.Update($"{this.GetName()}: sent {troops} troop(s) to {selected.GetName()}");

                Killfeed.Update($"{this.GetName()}: attack and fortify not yet implemented");

                //
                // implement the other phases
                //

                GameController.Get().currentPhase.text = "draft phase";
                GameController.Get().HandleObjectClick = GameController.Get().DraftPhase;
                GameController.Get().NextTurn();
                return;
            });
        }
    }
}
