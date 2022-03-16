using System;
using System.Collections.Generic;
using UnityEngine;

namespace Board.Tiles
{
    public class EasterEggTile : Tile
    {
        //Using a list here incase we decide to make tiles have multiple properties. This may be usedful down the line.
        private List<Tile> _otherComp;
        void Awake()
        {
            if (enabled)
            {
                _forcePlayerInteraction = true;
                _otherComp = new List<Tile>();
                foreach (Tile comp in gameObject.GetComponents<Tile>())
                {
                    if (comp.GetType() != typeof(EasterEggTile))
                    {
                        _otherComp.Add(comp);
                        print(comp);
                        comp.enabled = false;
                    }
                }

                //if the player is currently activating the tile while this process is happening there will be a null ref.
                print("EE now Active");
            }
        }

        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            //Play mini game. When the minigame sends the players back, hopefully this function continues // TODO: Testing for errors here.
            
            //Re-enable the other components
            foreach (Tile t in _otherComp)
            {
                t.enabled = true;
            }
            //disable this component (only re-enabled if game manager says so.) This also effectively triggers the OnDisableMethod
            enabled = false;
        }

        private void OnDisable()
        {
            print("EE now Offline");
            //End turn
        }
    }
}
