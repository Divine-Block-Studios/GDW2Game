using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CutSceneController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private List<Sprite> assets;

    private Transform[] _children;
    private List<Rigidbody2D> simulating;

    [SerializeField] private Transform throwPoint;

    [SerializeField] private float timeForFirstImage;
    [SerializeField] private float fadeIOTime;
    [SerializeField] private int animDelay = 400;

    [SerializeField] private Transform SLleft;
    [SerializeField] private Transform SLright;

    private Vector2 midPoint;
    void Awake()
    {
        _children = new Transform[transform.GetChild(0).childCount];
        simulating = new List<Rigidbody2D>();
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            _children[i] = transform.GetChild(0).GetChild(i);
            //Second half
            if ( i / (_children.Length / 2) == 1)
            {
                print(Screen.width);
                _children[i].position = new Vector3(Screen.width - 140 - 140 * (i % 2), Screen.height - 40 - 140 * ((int)(i/2) % 4 + 1));
            }
            else
            {
                _children[i].position = new Vector3(100 + 140 * (i % 2), Screen.height - 40 - 140 * ((int)(i/2) % 4 + 1));
            }
            simulating.Add(_children[i].GetComponent<Rigidbody2D>());
            int val = Random.Range(0, assets.Count);
            _children[i].GetComponent<Image>().sprite = assets[val];
            simulating[i].simulated = false;
            assets.RemoveAt(val);
        }
        Controller();
        midPoint = new Vector2(Screen.width/2, Screen.height/2);
    }

    private async void Controller()
    {
        await Fade(true);
        await Task.Delay(animDelay);
        await ThrowIcons();
        await Update();
        await Task.Delay(animDelay);
        await Fade(false);
        Destroy(gameObject, 1);
    }

    private async Task Update()
    {
        while (simulating.Count > 0)
        {
            for (int i = 0; i < simulating.Count; i++)
            {
                Vector2 pos = simulating[i].position;

                if (pos.x < midPoint.x - 70 && simulating[i].velocity.x < 0)
                {
                    simulating[i].velocity = new Vector2(-simulating[i].velocity.x, simulating[i].velocity.y + 0.2f);
                }

                if (pos.x > 70 + midPoint.x && simulating[i].velocity.x > 0)
                {
                    simulating[i].velocity = new Vector2(-simulating[i].velocity.x, simulating[i].velocity.y + 0.2f);
                }
                
                if (pos.y < midPoint.y - 200 && simulating[i].simulated)
                {
                    simulating[i].GetComponent<Rigidbody2D>().simulated = false;
                    simulating.RemoveAt(i);
                }
            }
            await Task.Yield();
        }
    }

    private async Task ThrowIcons()
    {
        StaticHelpers.Shuffle(_children);
        for (int i = 0; i < _children.Length; i++)
        {
            Vector3 endPos = _children[i].position.x < midPoint.x? SLleft.position : SLright.position;
            int temp = i;
            StaticHelpers.MoveSlerp(_children[i], _children[i].position, endPos, 300,
                () => { _children[temp].GetComponent<Rigidbody2D>().simulated = true;});
            float ySpeed = Random.Range(-100, -40);
            _children[i].GetComponent<Rigidbody2D>().velocity = _children[i].position.x < midPoint.x?new Vector2(150,ySpeed):new Vector2(-150,ySpeed);
            float throwDelay = (float)(_children.Length - i - 1) / (_children.Length - 1) * timeForFirstImage;
            while (throwDelay > 0)
            {
                throwDelay -= Time.deltaTime;
                await Task.Yield();
            }
        }
    }
    
    private void throwItem(int i) {
        StaticHelpers.ThrowAt2D(_children[i], _children[i].position, throwPoint.position, 0, 1, 20);
    }

    //Move to static helpers?
    private async Task Fade(bool fadeIN)
    {
        float alphaVal = fadeIN?0:1;
        float curTime = fadeIOTime;
        List<Image> images = new List<Image>();

        for (int i = 0; i < _children.Length; i++)
        {
            images.Add(_children[i].GetComponent<Image>());
        }

        images.Add(transform.GetChild(0).GetComponent<Image>());
        images.Add(transform.GetChild(1).GetComponent<Image>());
        
        while (curTime > 0)
        {
            for (int i = 0; i < images.Count; i++)
            {
                images[i].color = new Color(1f,1f,1f,alphaVal);
            }

            alphaVal = fadeIN?1 - curTime / fadeIOTime:curTime / fadeIOTime;
            curTime -= Time.deltaTime;

            await Task.Yield();
        }
    }
}
