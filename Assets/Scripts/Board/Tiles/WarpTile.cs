using System;
using System.Collections.Generic;
using Photon.Pun;
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
                Vector3 dist = moveToTiles[i].transform.position - _origin;
                Vector3 normal = (dist) / distance;
                
                //This working makes me sad
                //There's a tilt sometimes which is an issue

                //print(Quaternion.FromToRotation(Vector3.right, normal).eulerAngles - new Vector3(0,0,180));
                Quaternion rotation = Quaternion.FromToRotation(dist,Vector3.up);
                int reps = Mathf.CeilToInt(distance / distBetweenArrowSpawns);

                //Quaternion rotation;
                
               // rotation = Quaternion.Euler(Vector3.Angle(_origin, moveToTiles[i].transform.position));
                

                //
                for (int j = 0; j < reps; j++)
                {
                    Vector3 location = (j == 0)
                        ? 0.2f * distBetweenArrowSpawns * normal + transform.position: j * distBetweenArrowSpawns * normal + transform.position;
                    _arrows.Add(PhotonNetwork.Instantiate("Prefabs/Instantiated Assets/" + arrow.name, location, rotation));
                        _arrows[_arrows.Count-1].name = "Arrow: " + (_arrows.Count-1); // why would this not work??
                    
                    //Add a listener to the arrow
                    if(GameManager.gameManager.GetCurrentPlayer == GameManager.gameManager.MyPlayer)
                        _arrows[_arrows.Count-1].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt], player));
                }
            
                //Highlight all tiles that the player can move to (For Clarification purposes)
                _highlights.Add(PhotonNetwork.Instantiate("Prefabs/Instantiated Assets/" + highlight.name, moveToTiles[i].transform.position - new Vector3(0,-0.2f,0f), Quaternion.Euler(new Vector3(-90,0,0))));
                if(GameManager.gameManager.GetCurrentPlayer == GameManager.gameManager.MyPlayer)
                    _highlights[i].GetComponent<Interactable>().ONClick.AddListener(() => PressedBtn(moveToTiles[delegateInt], player));
            }
        }


        private void PressedBtn(Tile pressed, BoardPlayer player)
        {
            //if didnt do a  regular move.
            if (pressed != moveToTiles[moveToTiles.Count - 1])
            {
                //Take money.
                player.UpdateCoins(-costToTravel);
            }


            foreach (GameObject go in _arrows)
            {
                //Remove the arrow
                PhotonNetwork.Destroy(go);
            }

            foreach (GameObject go in _highlights)
            {
                //Apparently it's recommended to destroy the listners after..
                PhotonNetwork.Destroy(go);
            }
            GameManager.gameManager.EndAction(pressed, _costsMoveToPass);
        }
    }
}
