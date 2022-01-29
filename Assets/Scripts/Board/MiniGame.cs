using UnityEngine;

namespace Board
{
    [CreateAssetMenu]
    public class MiniGame : AwardableEvents
    {
        [SerializeField] private string gameSceneName;
        
        public override void Init(BoardPlayer ply)
        {
            GameManager.gameManager.LoadMiniGame(gameSceneName);
        }
    }
}
