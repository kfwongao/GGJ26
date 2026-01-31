using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CombatInfoHolder : MonoBehaviour
{
    public Text infoText;

    // Start is called before the first frame update
    void Start()
    {
        if (!infoText)
            Debug.LogError("CombatInfoHolder : infoText == null");
    }

    public void Init(string value, Color color, int direction = 1) // direction == 1 : up, -1: down
    {
        infoText.color = color;
        infoText.text = $"{value}";

        infoText.DOFade(1f, 0.1f);
        transform.DOLocalMoveY(400 * direction, 2f);
        infoText.DOFade(0.05f, 0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
