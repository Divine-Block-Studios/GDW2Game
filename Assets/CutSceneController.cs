using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class CutSceneController : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] private List<Sprite> assets;

    private Transform[] _children;

    [SerializeField] private Transform throwPoint;

    [SerializeField] private float timeForFirstImage;
    [SerializeField] private float fadeIOTime;
    [SerializeField] private int animDelay = 400;

    void Awake()
    {;
        _children = new Transform[transform.GetChild(0).childCount];
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
        {
            _children[i] = transform.GetChild(0).GetChild(i);
            int val = Random.Range(0, assets.Count);
            _children[i].GetComponent<Image>().sprite = assets[val];
            assets.RemoveAt(val);
        }
        Controller();
    }

    private async void Controller()
    {
        print("Controller: A");
        await Fade(true);
        print("Controller: B");
        await Task.Delay(animDelay);
        print("Controller: C");
        await ThrowIcons();
        print("Controller: D");
        await Task.Delay(animDelay);
        print("Controller: E");
        await Fade(false);
        print("Controller: F");
        Destroy(gameObject, 1);
    }

    private async Task ThrowIcons()
    {
        print("Debug: " + _children.Length);
        for (int i = 0; i < _children.Length; i++)
        {
            StaticHelpers.ThrowAt(_children[i], _children[i].position, throwPoint.position, 0, 1, 3);
            _children[i].GetComponent<Rigidbody2D>().simulated = true;
            int timeForThrow = (int)((_children.Length - i - 1)  / (_children.Length - 1) * timeForFirstImage) * 1000;
            await Task.Delay(timeForThrow);
        }
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
