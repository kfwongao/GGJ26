using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using Unity.VisualScripting;


public class HitAnim : MonoBehaviour
{
    public Text tips;

    public void Init(string text, Color color, Vector3 pos, int textSize = 60)
    {
        tips.text = text;
        tips.color = color;
        tips.fontSize = textSize;
        transform.position = pos;
        OnStart();
    }

    // Start is called before the first frame update
    void OnStart()
    {
        transform.DOScale(3f, 0.3f).OnComplete(() => { transform.DOScale(1, 0.8f); });
        
        transform.DOMove(transform.position + new Vector3(15f, 120f, 0f), 0.8f).OnComplete(() => { tips.DOFade(0.1f, 0.8f); });


        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
