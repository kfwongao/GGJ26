using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndCombatInfoHolder : MonoBehaviour
{
    public Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        if (!infoText)
            Debug.LogError("EndCombatInfoHolder : infoText == null");
    }

    public void Init(string value, Color color)
    {
        infoText.color = color;
        infoText.text = $"{value}";

        infoText.DOFade(1f, 0.3f);
        infoText.DOFade(0.05f, 4f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
