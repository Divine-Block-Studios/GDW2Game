using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Board.Tiles
{
    public class TurnTile : Tile
    {
        [SerializeField] private List<Tile> moveToTiles;
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject highlight; // May make sense to have this as a material
        [SerializeField] private float distBetweenArrowSpawns = 10;
    
        private List<GameObject> _arrows;
        private List<GameObject> _highlights;
        private Vector3 _origin;
        private void Start()
        {
            
            _costsMoveToPass = false;
            _origin = transform.position;
            _forcePlayerInteraction = true;
            if(moveToTiles.Count == 0)
                throw new Exception("TILE IS REDUNDANT: " + gameObject.name + " ADD OTHER MoveToTiles or change object script to Tile");
            moveToTiles.Add(nextTile.GetComponent<Tile>());
        }

        //TODO: Work through this weird issue.
        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            _arrows = new List<GameObject>();
            _highlights = new List<GameObject>();
            //Create Arrow Gameobjects
            player.currentTile = this;
            for (int i = 0; i < moveToTiles.Count; i++)
            {
                int delegateInt = i;
                float distance = Vector3.Distance(_origin, moveToTiles[i].transform.position);
                Vector3 normal = (moveToTiles[i].transform.position - _origin) / distance;
                
                //This working makes me sad
                //There's a tilt sometimes which is an issue
                Quaternion rotation = Quaternion.Euler(Quaternion.FromToRotation(Vector3.up, normal).eulerAngles - new Vector3(0,0,180));
                int reps = Mathf.CeilToInt(distance / distBetweenArrowSpawns);

                //
                for (int j = 0; j < reps; j++)
                {
                    Vector3 location = (j == 0)
                        ? 0.2f * distBetweenArrowSpawns * normal + transform.position: j * distBetweenArrowSpawns * normal + transform.position;
                    _arrows.Add(Instantiate(arrow, location, rotation));
                        _arrows[_arrows.Count-1].name = "Arrow: " + (_arrows.Count-1); // why would this not work??
                    
                    //Add a listener to the arrow
                    _arrows[_arrows.Count-1].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt]));
                }
            
                //Highlight all tiles that the player can move to (For Clarification purposes)
                _highlights.Add(Instantiate(highlight, moveToTiles[i].transform.position - new Vector3(0,0f,0.2f), Quaternion.identity));
                _highlights[i].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt]));
            }
        }


        private void PressedBtn(Tile pressed)
        {
            foreach (GameObject go in _arrows)
            {
                //Remove the arrow
                Destroy(go);
            }

            foreach (GameObject go in _highlights)
            {
                //Apparently it's recommended to destroy the listners after..
                Destroy(go);
            }
            GameManager.gameManager.EndAction(pressed, _costsMoveToPass);
        }
    }
}
