using UnityEngine;
using System.Collections;

public class ButtonScriptByColor : MonoBehaviour
{
    public Camera cam;

    public Sprite Sprite;
    public Color DefColor = Color.white;
    public Color OnColor = Color.white;

    public GameObject MessageObj;
    public string Message;

    private SpriteRenderer sr;

    private bool StartOn;//안에서시작되었나
    private bool Down;

    public AudioClip ClickSound;

    // Use this for initialization
    void Start()
    {
        if (cam == null)
            cam = Camera.main;
        StartOn = false;
        Down = false;
        sr = gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer;
    }
    void Update()
    {
        if (SystemInfo.deviceType == DeviceType.Desktop)
        {
            Vector2 pos = Input.mousePosition;
            Vector2 hitPos = cam.ScreenToWorldPoint(pos);

            if (Physics2D.OverlapPoint(hitPos) == transform.collider2D)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    StartOn = true;
                    sr.color = OnColor;
                    Down = true;
                }
                if (StartOn && Input.GetMouseButton(0))
                {
                    sr.color = OnColor;
                    Down = true;
                }
                if (Input.GetMouseButtonUp(0))
                {
                    if (MessageObj != null && StartOn)
                    {
                        if (ClickSound != null)
                            AudioSource.PlayClipAtPoint(ClickSound, gameObject.transform.position);

                        MessageObj.SendMessage(Message);
                    }

                    sr.color = DefColor;
                    StartOn = false; Down = false;

                }
            }
            else if (Input.GetMouseButtonUp(0))
            {
                sr.color = DefColor;
                StartOn = false; Down = false;
            }
            else
            {
                sr.color = DefColor;
                Down = false;
            }
            return;
        }

        if (Input.touchCount > 0)
        {
            Vector2 pos = Input.mousePosition;
            Vector2 hitPos = cam.ScreenToWorldPoint(pos);

            if (Physics2D.OverlapPoint(hitPos) == transform.collider2D)
            {
                if (Input.GetTouch(0).phase == TouchPhase.Began)
                {
                    StartOn = true;
                    sr.color = OnColor;
                    Down = true;
                }
                if (StartOn)
                {
                    sr.color = OnColor;
                    Down = true;
                }

                if (Down && Input.GetTouch(0).phase == TouchPhase.Ended && MessageObj != null)
                    MessageObj.SendMessage(Message);
            }
            else//안에들어가있지도않으면 무조건 false
            {
                sr.color = DefColor;
                Down = false;
            }

        }
        else//터치가아예안된거면뭐걍..
        {
            sr.color = DefColor;
            StartOn = false; //안에서시작도리셋이지.
            Down = false;
        }
    }
}
