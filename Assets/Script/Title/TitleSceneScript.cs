using UnityEngine;
using System.Collections;

public class TitleSceneScript : MonoBehaviour
{
    public Animator MainCameraAnim;

    void Update()
    {
        if (Input.GetMouseButtonUp(0))
            MainCameraAnim.SetBool("GameStart", true);
    }
}
