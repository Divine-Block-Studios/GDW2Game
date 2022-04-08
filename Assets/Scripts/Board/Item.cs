using System;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Board
{
    [CreateAssetMenu]
    public class Item : AwardableEvents
    {

        [Header("Event")] 
        [SerializeField] private UnityEvent events;


        private BoardPlayer _ply;
        public Action action
        {
            get => action ?? events.Invoke;
            set => action = value;
        }


        public override void Init(BoardPlayer ply)
        {
            _ply = ply;
            if (instantlyUsed)
            {
                events.Invoke();
            }
            else
            {
                ply.Item = this;
            }
        }
        public void RollDiceTwice()
        {
            //When activated, roll two dice.
            Debug.Log("Rolling two dice on next roll");
            
        }

        public void MultiplyDiceVal()
        {
            //When activated, roll dice, set multiply next val to true.
            //If activated during turn, just multiply remaining vals.
            Debug.Log("Doubling value of next roll");
        }

        public void DivideDiceVal()
        {
            //When activated, roll dice, set multiply next val to true.
            //If activated during turn, just multiply remaining vals. (Round up to avoid issues?)
            Debug.Log("Dividing value of next roll");
        }

        public void TpToRngPly()
        {
            //When activated, Tp the player IMEDIATELY to a random player that is not themselves
            _ply.currentTile = GameManager.gameManager.GetRandomPlayer(_ply).currentTile;
        }

        public void BringAllToPly()
        {
            BoardPlayer[] players = GameManager.gameManager.players;
            //When activated, move all players that aren't the current player to the same tile
            for (int i =0; i < players.Length; i++)
            {
                if (players[i] == _ply)
                    continue;
                players[i].currentTile = _ply.currentTile;
            }
        }

        public void SwapCoinsRngPly()
        {
            //When activated [Cannot be activated manually on rounds before shred], swap coins with randomly selected player.
            //StaticHelpers.Swap(ref _ply.coins, ref GameManager.gameManager.GetRandomPlayer(_ply).coins);
        }

        public void StealCoinsFromRngPly()
        {
            //When activated [Cannot be activated manually on rounds before shred], steal a small amount of coins from a random player
            //int coinsStolen = RollDice
            //Boardplayer ply = GameManager.gameManager.GetRandomPlayer(_ply);
            //int diff = Mathf.max(0, ply.coins - coinsStolen)
            //ply.coins -= diff;
            //_ply.coins += diff;
            
            
        }
    }
}
