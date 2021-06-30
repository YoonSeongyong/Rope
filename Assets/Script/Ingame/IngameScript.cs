using UnityEngine;
using System.Collections;

public class IngameScript : MonoBehaviour
{
    /*
     * 파일 로드시 필요한것들(로드레벨)
     * 구조물의 종류, 개수, 위치, 회전값
     * 로프 시작 위치
     * 로프 시작 길이(개수)
     * 각 로프의 위치와 회전값,
     * TAG 개수 및 종류
     * 각 TAG의 위치와 회전값,
     * 클리어 조건
     */

    private int RopeLenth;
    private Vector3[] RopePos;
    private Quaternion[] RopeRot;
    

    void Start()
    {
        LoadLevel();

    }

    void Update()
    {

    }

    void LoadLevel()
    {
        RopeLenth = 10;
        RopePos = new Vector3[RopeLenth];
        RopeRot = new Quaternion[RopeLenth];
    }
}
