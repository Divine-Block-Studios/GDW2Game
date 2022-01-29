using UnityEngine;
namespace Board
{
    [CreateAssetMenu]
    public class Item : AwardableEvents
    {
        public override void Init(BoardPlayer ply)
        {
            Debug.Log("Uhm yeah this will require more thought: " + awardName);
        }
    }
}
