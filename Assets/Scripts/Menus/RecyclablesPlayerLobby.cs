using Mirror;
using UnityEngine;

namespace Menus
{
    public class RecyclablesPlayerLobby : NetworkRoomPlayer
    {

        public Vector3 pos;
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            transform.SetParent(GameObject.Find("Canvas").transform);
        }
    }
}
