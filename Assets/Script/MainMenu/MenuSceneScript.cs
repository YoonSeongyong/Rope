using UnityEngine;
using System.Collections;

public class MenuSceneScript : MonoBehaviour
{
    public Animator FadeOut;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            FadeOut.SetBool("FadeOut", true);
    }
}
