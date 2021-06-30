using UnityEngine;
using System.Collections;

public class AndroidInputBox : MonoBehaviour
{
    public Camera cam;

    public TouchScreenKeyboardType KeyboardType;
    public TextMesh textmesh;


    private bool StartOn;//안에서시작되었나
    private bool Down;

    private TouchScreenKeyboard keyboard;
    private string inputstring;

    void Start()
    {
        StartOn = false;
        Down = false;
        inputstring = "";
    }

    void KeyboardOpen()
    {
        keyboard = TouchScreenKeyboard.Open(inputstring, KeyboardType);
    }

    void Update()
    {
        if (Input.touchCount > 0)
        {
            Vector2 pos = Input.mousePosition;
            Vector2 hitPos = cam.ScreenToWorldPoint(pos);

            if (Physics2D.OverlapPoint(hitPos) == transform.collider2D)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    StartOn = true;
                    Down = true;
                }
                if (StartOn)
                    Down = true;

                KeyboardOpen();
            }
            else
                Down = false;
        }
        else
        {
            StartOn = false;
            Down = false;
        }

        if (keyboard != null)
        {
            if (keyboard.done)
            {
                keyboard = null;
                TouchScreenKeyboard.hideInput = false;
            }
            if (inputstring != keyboard.text)
            {
                inputstring = keyboard.text;
                textmesh.text = inputstring;
            }
        }
    }
}

