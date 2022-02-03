using UnityEngine;
namespace Board
{
    [CreateAssetMenu]
    public class Item : AwardableEvents
    {
        [SerializeField] private int cost;

        public int Cost => cost; 
        public override void Init(BoardPlayer ply)
        {
            Debug.Log("Uhm yeah this will require more thought: " + awardName);
        }
    }
}
