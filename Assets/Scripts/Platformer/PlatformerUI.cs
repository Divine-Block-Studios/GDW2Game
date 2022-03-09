using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlatformerUI : MonoBehaviour
{
    public PlayerMovement player;

    public Text completion;
    public GameObject panel;

    bool dashed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateCompletion();
        if(dashed)
        {
            updateDash();
        }
    }

    void updateCompletion()
    {
        float goal = 13.535f;
        float curHeight = player.transform.position.y;
        int percentage = (int)(Mathf.RoundToInt((curHeight / goal) * 100));

        if (percentage < 0)
        {
            percentage = 0;
        }
        else if (percentage > 100)
        {
            percentage = 100;
        }

        completion.text = percentage.ToString() + "%";
    }

    public void checkDash()
    {
        dashed = true;
    }

    void updateDash()
    {
        if(player.elapsedFrames < 50)
        {
            RectTransform rt = panel.GetComponent<RectTransform>();
            int top = 200 - (player.elapsedFrames * 4);

            rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
        }
        else
        {
            dashed = false;
        }
    }
}
