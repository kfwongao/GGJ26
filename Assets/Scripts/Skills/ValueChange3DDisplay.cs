using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class ValueChange3DDisplay : MonoBehaviour
{
    public TextMeshProUGUI TMPText;
    public float destroyTime = 3f;
    private Camera main;

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - main.transform.position);
    }

    public void SetText(int value, bool isCritical, Color color)
    {
        string txt = value.ToString();
        if (isCritical)
        {
            txt = txt + " (Crit)";
        }

        TMPText.text = txt;
        TMPText.color = color;

        TMPText.DOFade(0.3f, destroyTime);
        transform.DOLocalMoveY(3.5f, destroyTime);

        Destroy(gameObject, destroyTime);
    }

    public void SetText(string value, Color color)
    {
        TMPText.text = value;
        TMPText.color = color;

        TMPText.DOFade(0.3f, destroyTime);
        transform.DOLocalMoveY(3.5f, destroyTime);

        Destroy(gameObject, destroyTime);
    }
}
