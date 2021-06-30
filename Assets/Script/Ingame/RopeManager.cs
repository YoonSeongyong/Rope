using UnityEngine;
using System.Collections;

public class RopeManager : MonoBehaviour
{
    /*
     * 인자를 받아 로프를 특정 GameObject의 자식으로 생성하고,
     * 각 로프들의 위치와 회전값 배열 또한 받아 생성한뒤에 바로
     * 위치를 옮겨준다.
    */
    private float CollsHeight = 2.0f;

    private int lenth;
    public int RopeLenght
    {
        get
        {return lenth;}
    }
    public GameObject RopePrf;

    private GameObject[] RopeObj;
    private ConfigurableJoint[] Ropejoint;

    // Use this for initialization
    void Start()
    {
//        CreateRopeColls();
    }

    public void DeleteAllRope()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            GameObject DeleteRope = transform.GetChild(i).gameObject;
            GameObject.Destroy(DeleteRope);
        }

    }

    public void CreateRope(int RopeLenght, Vector3[] roppos, Quaternion[] roprot, int IgnoreRopeCount = 2)
    {
        CreateRope(RopeLenght, IgnoreRopeCount);

        for (int i = 0; i < lenth; i++)
        {
            RopeObj[i].transform.position = roppos[i];
            RopeObj[i].transform.rotation = roprot[i];
        }

        Ropejoint[0].connectedBody = null;
    }

    public void CreateRope(int RopeLenght, int IgnoreRopeCount)
    {
        DeleteAllRope();
        lenth = RopeLenght;

        RopeObj = new GameObject[lenth];
        Ropejoint = new ConfigurableJoint[lenth];
        for (int i = 0; i < lenth; i++)
        {
            RopeObj[i] = Instantiate(RopePrf) as GameObject;

            RopeObj[i].transform.parent = gameObject.transform;
            Quaternion rot = Quaternion.identity;
            rot.eulerAngles = new Vector3(0, 0, -90.0f);
            RopeObj[i].transform.localRotation = rot;
            RopeObj[i].transform.localPosition = new Vector3(i * (CollsHeight - 1), 0, 0);

            RopeObj[i].GetComponent<CapsuleCollider>().height = CollsHeight;

            RopeObj[i].name = "RopsColls" + i.ToString();
            RopeObj[i].layer = LayerMask.NameToLayer("Rope");
            RopeObj[i].tag = transform.tag;

        }
        #region InitJoint
        for (int i = 0; i <= lenth - 1; i++)
        {
            Ropejoint[i] = RopeObj[i].AddComponent<ConfigurableJoint>();

            Ropejoint[i].anchor = new Vector3(1, 0, 0);
            Ropejoint[i].connectedAnchor = new Vector3(0, -0.5f, 0);

            Ropejoint[i].xMotion = ConfigurableJointMotion.Locked;
            Ropejoint[i].yMotion = ConfigurableJointMotion.Locked;
            Ropejoint[i].zMotion = ConfigurableJointMotion.Locked;
            Ropejoint[i].angularXMotion = ConfigurableJointMotion.Limited;
            Ropejoint[i].angularYMotion = ConfigurableJointMotion.Limited;
            Ropejoint[i].angularZMotion = ConfigurableJointMotion.Limited;

            SoftJointLimit jointLimit = new SoftJointLimit();
            jointLimit.spring = 0.0f;
            jointLimit.damper = 0.0f;
            jointLimit.bounciness = 0.0f;

            jointLimit.limit = -30.0f;

            Ropejoint[i].lowAngularXLimit = jointLimit;

            jointLimit.limit = 30.0f;
            Ropejoint[i].highAngularXLimit = jointLimit;

            jointLimit.limit = 30.0f;
            Ropejoint[i].angularYLimit = jointLimit;

            jointLimit.limit = 30.0f;
            Ropejoint[i].angularZLimit = jointLimit;

            Ropejoint[i].anchor = new Vector3(0, (CollsHeight - 1) * 0.5f * -1, 0);

            if (i == 0)
                Ropejoint[i].connectedBody = null;
            else
                Ropejoint[i].connectedBody = RopeObj[i - 1].GetComponent<Rigidbody>();

        }
        #endregion

        for (int i = 0; i < IgnoreRopeCount; i++)
            RopeObj[i].layer = LayerMask.NameToLayer("Rope(Start)");
    }

    public Vector3[] GetRopeObjPos()
    {
        Vector3[] poss = new Vector3[lenth];
        for (int i = 0; i < lenth; i++)
            poss[i] = RopeObj[i].transform.position;
        return poss;
    }
    public Vector3[] GetRopeObjRot()
    {
        Vector3[] rots = new Vector3[lenth];
        for (int i = 0; i < lenth; i++)
            rots[i] = RopeObj[i].transform.rotation.eulerAngles;
        return rots;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(transform.position, 0.3f);
    }
}
