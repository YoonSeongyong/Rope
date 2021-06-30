using UnityEngine;
using System.Collections;

public class RoteteMoveIcon : MonoBehaviour
{
    public Camera UICamera;

    public GameObject MoveIcon;
    public GameObject RotateIcon;

    public GameObject TargetObj;
    public string Massage;

    private bool RotateMode;

    private int fingerid;
    private bool TouchOn;

    void Start()
    {
        RotateMode = true;

        MoveIcon.SetActive(!RotateMode);
        RotateIcon.SetActive(RotateMode);

        TouchOn = false;
    }

    void Update()
    {
        MoveIcon.transform.localScale = new Vector3(1, 1, 1);
        RotateIcon.transform.localScale = new Vector3(1, 1, 1);

        if (Input.touchCount <= 0)
            return;

        for (int i = 0; i < Input.touchCount; i++)
        {
            Touch t = Input.GetTouch(i);
            Vector2 touchPos = UICamera.ScreenToWorldPoint(t.position);

            if (t.phase == TouchPhase.Began)
            {
                if (!TouchOn && Physics2D.OverlapPoint(touchPos) == transform.collider2D)
                {
                    fingerid = t.fingerId;
                    TouchOn = true;
                }
            }
            else if (t.phase == TouchPhase.Ended)
            {
                if (TouchOn && fingerid == t.fingerId && Physics2D.OverlapPoint(touchPos) == transform.collider2D)
                {
                    RotateMode = !RotateMode;

                    MoveIcon.SetActive(!RotateMode);
                    RotateIcon.SetActive(RotateMode);

                    TargetObj.SendMessage("SetCamRotateMode", RotateMode);
                }
                TouchOn = false;
            }
            else
            {
                if (TouchOn && fingerid == t.fingerId && Physics2D.OverlapPoint(touchPos) == transform.collider2D)
                {
                    MoveIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
                    RotateIcon.transform.localScale = new Vector3(1.1f, 1.1f, 1.0f);
                }
            }
        }
    }
}
