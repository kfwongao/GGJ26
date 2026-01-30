using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyPressed : MonoBehaviour
{
    public Image image;
    public Text text;
    public KeyCode keyCode;
    public KeyCode secondKeyCode;
    public Color keyPressedColor;
    public Color keyNotpressedColor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool isPressed = Input.GetKey(keyCode) || Input.GetKey(secondKeyCode);
        
        if(isPressed)
        {
            image.color = keyPressedColor;
            text.color = keyPressedColor;
        }
        else
        {
            image.color = keyNotpressedColor;
            text.color = keyNotpressedColor;
        }
    }
}
