using System;
using System.Collections.Generic;
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
            _arrows = new List<GameObject>();
            _highlights = new List<GameObject>();
            _origin = transform.position;
            _costsMoveToPass = false;
            if(moveToTiles.Count > 0)
                _forcePlayerInteraction = true;
            else
                throw new Exception("TILE IS REDUNDANT: " + gameObject.name + " ADD OTHER MoveToTiles or change object script to Tile");
            moveToTiles.Add(nextTile.GetComponent<Tile>());
            //gameObject.AddComponent<Interactable>().ONClick.AddListener(()=> PressedBtn()); Why'd I have this?
        }

        //TODO: Work through this weird issue.
        public override void LandedOn(BoardPlayer player)
        {
            //Create Arrow Gameobjects
            for (int i = 0; i < moveToTiles.Count; i++)
            {
                int delegateInt = i;
                float distance = Vector3.Distance(_origin, moveToTiles[i].transform.position);
                Vector3 normal = (moveToTiles[i].transform.position - _origin) / distance;
                //This rotation only works on a TwoD plane
                Quaternion rotation = Quaternion.Euler(arrow.transform.eulerAngles.x, arrow.transform.eulerAngles.y,Mathf.Atan2(moveToTiles[i].transform.position.y - _origin.y, moveToTiles[i].transform.position.x - _origin.x) * Mathf.Rad2Deg -90);
                //print("Distance: " + distance + " Normal: " + normal + " Rotation: " + rotation);
                int reps = Mathf.CeilToInt(distance / distBetweenArrowSpawns);
            
                print("I: " + i + " Reps: " + reps);
            
                //
                for (int j = 0; j < reps; j++)
                {
                    Vector3 location = j * distBetweenArrowSpawns * normal + new Vector3(0,0,-3);
                
                    _arrows.Add(Instantiate(arrow, location, rotation));
                    _arrows[_arrows.Count-1].name = "Arrow: " + (_arrows.Count-1); // why would this not work??
                    print("Arrow: " + i + ":" + j + " - " + _arrows[_arrows.Count-1].name + ", " + _arrows[_arrows.Count-1].transform.position + ", " + _arrows[_arrows.Count-1].transform.eulerAngles);
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
            Debug.Log("If I could, I'd move to: " + pressed.gameObject.name);
            player.MoveToTile(pressed);
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
