using UnityEngine;
using System.Collections;

public class ChageSceneOnAnimation : MonoBehaviour
{
    public string SceneName;

    public void ChageScene()
    {
        Application.LoadLevel(SceneName);
    }
}
