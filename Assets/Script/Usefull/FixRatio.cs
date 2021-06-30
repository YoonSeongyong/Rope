using UnityEngine;
using System.Collections;

public class FixRatio : MonoBehaviour
{
    public int Width;
    public int Height;
    void Awake()
    {
        Screen.SetResolution(Screen.width, (Screen.width / Width) * Height, true);
    }
}
