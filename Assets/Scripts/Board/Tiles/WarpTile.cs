using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Board.Tiles
{
    public class WarpTile : Tile
    {
        // Start is called before the first frame update
        [SerializeField] private List<Tile> moveToTiles;
        [SerializeField] private GameObject arrow;
        [SerializeField] private GameObject highlight; // May make sense to have this as a material
        [SerializeField] private float distBetweenArrowSpawns = 10;
        [SerializeField] private int costToTravel;
        [SerializeField] private bool randomlyPicked;
    
        private List<GameObject> _arrows;
        private List<GameObject> _highlights;
        private Vector3 _origin;
        private void Start()
        {
            _arrows = new List<GameObject>();
            _highlights = new List<GameObject>();
            _costsMoveToPass = true;
            _origin = transform.position;
            _forcePlayerInteraction = true;
            moveToTiles.Add(nextTile.GetComponent<Tile>());
        }

        //TODO: Work through this weird issue.
        protected override void LandedOnFunctionality(BoardPlayer player)
        {
            if (randomlyPicked)
            {
                GameManager.gameManager.EndAction(moveToTiles[Random.Range(0,moveToTiles.Count)], _costsMoveToPass);
                return;
            }

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
                    _arrows[_arrows.Count-1].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt], player));
                }
            
                //Highlight all tiles that the player can move to (For Clarification purposes)
                _highlights.Add(Instantiate(highlight, moveToTiles[i].transform.position - new Vector3(0,0f,0.2f), Quaternion.identity));
                _highlights[i].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt], player));
            }
        }


        private void PressedBtn(Tile pressed, BoardPlayer player)
        {
            //if didnt do a  regular move.
            if (pressed != moveToTiles[moveToTiles.Count - 1])
            {
                //Take money.
                player.AddCoins(-costToTravel);
            }


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
