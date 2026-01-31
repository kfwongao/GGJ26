using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageInfoHolder : MonoBehaviour
{
    public Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        if (!infoText)
            Debug.LogError("BufferInfoHolder : infoText == null");
    }

    public void Init(string value, Color color)
    {
        infoText.color = color;
        infoText.text = $"{value}";

        infoText.DOFade(1f, 0.1f);
        infoText.transform.DOScale(new Vector3(2, 2, 2), 0.1f);
        transform.DOLocalMoveY(100, 2f);
        infoText.transform.DOScale(new Vector3(1, 1, 1), 0.5f);
        infoText.DOFade(0.1f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
