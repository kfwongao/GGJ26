using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;


public class TipsAnim : MonoBehaviour
{
    public Text tips;

    public void Init(string text)
    {
        tips.text = text;
        OnStart();
    }

    // Start is called before the first frame update
    void OnStart()
    {
        transform.localScale = new Vector3(1, 0, 1);
        transform.DOScaleY(1, 0.5f).OnComplete(() => { transform.DOLocalMoveY(250f, 1.5f); });

        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
