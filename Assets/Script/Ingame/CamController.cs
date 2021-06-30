using UnityEngine;
using System.Collections;

public class CamController : MonoBehaviour
{
    public float ZoomVal = 0.5f;
    public float ZoomMax = 10;
    public float ZoomMin = -20;
    void Start()
    {

    }

    void Update()
    {
        Vector3 Pos = transform.position;
        if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            Pos.z += ZoomVal;
            if (Pos.z >= ZoomMax)
                Pos.z = ZoomMax;
        }
        if (Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            Pos.z -= ZoomVal;
            if (Pos.z <= ZoomMin)
                Pos.z = ZoomMin;
        }
        transform.position = Pos;
    }
}
