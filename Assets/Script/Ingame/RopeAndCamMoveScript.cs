using UnityEngine;
using System.Collections;

public class RopeAndCamMoveScript : MonoBehaviour
{
    //줄 바깥을 드래그하면 회전,
    //오른쪽 아래 조이패드를 드래그함으로써 시야 상하좌우 조절,
    //업,다운으로 확대 축소,

    //드래그 멀티터치 개수
    public int DragableCount = 3;

    public string RopeTag;

    public Camera GameCamera;
    public GameObject CamOrbit;
    public float CamRotSpeed = 0.3f;
    public float CamZoomSpeed = 1.5f;
    public float CamMoveSpeed = 0.1f;

    public float CamMaxZ;
    public float CamMinZ;
    public Vector2 CamMaxMovePos;
    public Vector2 CamMinMovePos;
    public float MaxCamRotateXValue;
    public float MinCamRotateXValue;

    public GameObject GrabParticlePrefeb;
    public GameObject MoveParticlePrefeb;

    public Material DragRopeMaterial;


    private ArrayList movers;


    private bool RotateCam;                 //캠회전 ON Off 여부
    private int RotateCamFingerID;          //캠회전에 사용할 FingerID

    private bool ZoomCam;                   //캠확대 On Off 여부
    private float FirstZoomFingerDistance;  //캠확대할때 처음 두 손가락 사이 거리
    private float ZoomDefZ;                 //캠확대할때 최초 Z축 값

    private int ZoomCamFingerID;          //캠확대에 사용할 SubFingerID

    private bool CamRotateMode;

    public void SetCamRotateMode(bool rot)
    {
        CamRotateMode = rot;
    }

    void Start()
    {
        RotateCam = false;
        CamRotateMode = true;
        movers = new ArrayList();

    }

    float GetTouchesDistaceByZPos(Vector2 pos1, Vector2 pos2, float z)
    {
        Vector3 spos1 = pos1;
        spos1.z = z;
        Vector3 spos2 = pos2;
        spos2.z = z;
        spos1 = GameCamera.ScreenToWorldPoint(spos1);
        spos2 = GameCamera.ScreenToWorldPoint(spos2);

        return Vector2.Distance(spos1, spos2);
    }

    void CreateMover(int Fingerid, Rigidbody ConnectedBody, Vector3 TouchPos)
    {
        Mover move = new Mover();
        move.init(GameCamera, MoveParticlePrefeb, GrabParticlePrefeb, DragRopeMaterial, Fingerid, ConnectedBody, TouchPos);
        movers.Add(move);
    }

    public struct Mover
    {
        private GameObject obj;             //움직여지는 오브젝트
        private GameObject moveParticle;    //움직여질파티클
        private GameObject grabParticle;    //로프에있을파티클
        private SpringJoint joint;          //Spring joint
        private int fingerid;               //fingerid;
        private Camera GameCamera;
        private LineRenderer linerdr;       //linerenderer

        public void init(Camera GC, GameObject mpp, GameObject gpp, Material DragRopeMaterial, int fid, Rigidbody ConnectedBody, Vector3 TouchPos)
        {
            //오브젝트 생성
            obj = new GameObject();
            obj.transform.name = "RopeMover";
            obj.transform.position = ConnectedBody.transform.position;

            moveParticle = GameObject.Instantiate(mpp) as GameObject;
            moveParticle.transform.parent = obj.transform;
            moveParticle.transform.localPosition = Vector3.zero;

            grabParticle = GameObject.Instantiate(gpp) as GameObject;
            grabParticle.transform.parent = ConnectedBody.transform;
            grabParticle.transform.localPosition = Vector3.zero;

            //카메라의 자식으로
            GameCamera = GC;
            obj.transform.parent = GameCamera.transform;


            //obj는 직접적인 물리적영향을 받지않음.
            obj.AddComponent<Rigidbody>().isKinematic = true;

            //SpringJoint 초기화
            joint = obj.AddComponent<SpringJoint>();
            joint.connectedBody = null;
            joint.anchor = new Vector3(0, 0, 0);
            joint.connectedAnchor = new Vector3(0, 0, 0);
            joint.spring = 100.0f;
            joint.damper = 0.0f;
            joint.minDistance = 0.0f;
            joint.maxDistance = 0.0f;
            joint.breakForce = 20.0f;
            joint.breakTorque = Mathf.Infinity;

            fingerid = fid;
            joint.connectedBody = ConnectedBody;

            //LineRDR
            linerdr = obj.AddComponent<LineRenderer>();
            linerdr.SetWidth(0.1f, 0.5f);
            linerdr.material = DragRopeMaterial;

            Update(TouchPos);
        }

        public void Update(Vector3 TouchPos)
        {
            Vector3 screenPoint = TouchPos;
            screenPoint.z = obj.transform.localPosition.z;
            obj.transform.position = GameCamera.ScreenToWorldPoint(screenPoint);

            linerdr.SetPosition(0, moveParticle.transform.position);
            linerdr.SetPosition(1, grabParticle.transform.position);
        }

        public int GetFingetId()
        {
            return fingerid;
        }

        public void Destroy()
        {
            GameObject.Destroy(obj);
            GameObject.Destroy(grabParticle);
        }
    }

    void Update()
    {
        #region PCTEST
        /*
        if (Application.platform == RuntimePlatform.WindowsEditor)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit hit = new RaycastHit();
                Ray ray = GameCamera.ScreenPointToRay(Input.mousePosition);
                if (Physics.Raycast(ray.origin, ray.direction, out hit))
                {
                    //로프를 클릭했을경우
                    if (hit.collider.tag == RopeTag)
                    {
                        //JointObj(움직일기준점) 초기화.
                        JointObj.transform.LookAt(GameCamera.transform.position);
                        JointObj.transform.position = hit.transform.position;
                        //ConnectedBody 넣어줌.
                        joint.connectedBody = hit.collider.GetComponent<Rigidbody>();
                        DragOn = true;
                    }
                    //CAM
                    else
                        CamOn = true;
                }
                else
                    CamOn = true;

                StartMousePos = Input.mousePosition;
                StartCamPos = CamOrbit.transform.position;
            }
            //up
            else if (Input.GetMouseButtonUp(0))
            {
                joint.connectedBody = null;
                DragOn = false;
                CamOn = false;
            }
            //drag
            else if (Input.GetMouseButton(0))
            {
                if (DragOn)
                {
                    //드래그일경우. 움직여줌.
                    JointObj.transform.position = GetMousePosition3D(JointObj.transform.localPosition.z);
                }
                else if (CamOn)
                {
                    Vector3 vpos = GameCamera.ScreenToViewportPoint(Input.mousePosition - StartMousePos);

                    CamOrbit.transform.Translate(Vector3.up * -vpos.y * CamDragSpeed);
                    CamOrbit.transform.Translate(Vector3.right * -vpos.x * CamDragSpeed);

                    StartMousePos = Input.mousePosition;

                    Vector3 CamPos = CamOrbit.transform.position;

                    if (CamPos.x > CamMaxMoveVector.x)
                        CamPos.x = CamMaxMoveVector.x;
                    else if (CamPos.x < CamMinMoveVector.x)
                        CamPos.x = CamMinMoveVector.x;

                    if (CamPos.y > CamMaxMoveVector.y)
                        CamPos.y = CamMaxMoveVector.y;
                    else if (CamPos.y < CamMinMoveVector.y)
                        CamPos.y = CamMinMoveVector.y;

                    if (CamPos.z > CamMaxMoveVector.z)
                        CamPos.z = CamMaxMoveVector.z;
                    else if (CamPos.z < CamMinMoveVector.z)
                        CamPos.z = CamMinMoveVector.z;

                    CamOrbit.transform.position = CamPos;

                }
            }

            if (joint.connectedBody == null)
            {
                GrapRopeParticle.SetActive(false);
                MoverParticle.SetActive(false);
            }
            else
            {
                GrapRopeParticle.transform.position = joint.connectedBody.transform.position;
                GrapRopeParticle.SetActive(true);
                MoverParticle.SetActive(true);
            }

            return;
        }
         */
        #endregion


        //터치시작됬을때 만약 rope바깥을터치했으면
        //화면을 돌아가는것으로 인식,
        //CamOn을 True로, fingerid를 넣어서
        //해당 fingerid의 좌표 기준으로 움직이게해준다

        //만약 CamOn이든 뭐든 손가락 두개가 바깥에서 터치되었으면
        //확대,축소로 인식하고 움직이진 않고 확대,축소만 해준다.


        bool[] stillon = new bool[movers.Count];
        for (int i = 0; i < movers.Count; i++)
            stillon[i] = false;
        bool stillRotateCam = false;
        bool stillZoomCam = false;

        Vector2 ZoomTouchpos1 = Vector2.zero;
        Vector2 ZoomTouchpos2 = Vector2.zero;

        #region Touch
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                Touch t = Input.GetTouch(i);
                if (RotateCam && RotateCamFingerID == t.fingerId)
                    stillRotateCam = true;
                if (ZoomCam && ZoomCamFingerID == t.fingerId)
                    stillZoomCam = true;

                //터치 시작
                if (t.phase == TouchPhase.Began)
                {
                    //터치 RAY를 쏘아 hit, Collider를 가져옴.
                    RaycastHit hit = new RaycastHit();
                    Ray ray = GameCamera.ScreenPointToRay(t.position);

                    bool CamRotateStart = false;
                    if (Physics.Raycast(ray.origin, ray.direction, out hit))
                    {
                        //태그가 rope이며 mover를 생성할 자리가 있다면,
                        //mover 생성해줌.
                        if (hit.collider.tag == RopeTag)
                        {
                            if (movers.Count < DragableCount)
                                CreateMover(t.fingerId, hit.collider.rigidbody, t.position);
                        }
                        else
                            CamRotateStart = true;
                    }
                    else
                        CamRotateStart = true;

                    if (CamRotateStart)
                    {
                        //회전 안켜져있으면 회전켜줌
                        if (!RotateCam)
                        {
                            RotateCam = true;
                            stillRotateCam = true;
                            RotateCamFingerID = t.fingerId;
                        }
                        //만약에 회전켜져있고 줌 안켜져있으면 줌켜줌
                        else if (!ZoomCam)
                        {
                            ZoomCam = true;
                            stillZoomCam = true;
                            RotateCamFingerID = t.fingerId;

                            for (int index = 0; index < Input.touchCount; index++)
                            {
                                Touch zt = Input.GetTouch(index);
                                if (zt.fingerId == RotateCamFingerID)
                                    ZoomTouchpos1 = zt.position;
                                if (zt.fingerId == ZoomCamFingerID)
                                    ZoomTouchpos2 = zt.position;
                            }
                            FirstZoomFingerDistance = GetTouchesDistaceByZPos(ZoomTouchpos1, ZoomTouchpos2, -20);
                            ZoomDefZ = GameCamera.transform.localPosition.z;

                        }
                    }

                }
                //뗐을때
                else if (t.phase == TouchPhase.Ended)
                {
                    //
                }

                //움직일때
                else
                {
                    //현재 터치의 fingerid와 맞는 mover를 찾아 Update
                    for (int index = 0; index < movers.Count; index++)
                    {
                        Mover mover = (Mover)movers[index];
                        if (mover.GetFingetId() == t.fingerId)
                            mover.Update(t.position);
                    }

                    if (!ZoomCam && (RotateCam && t.fingerId == RotateCamFingerID))
                    {
                        //캠 회전 처리.
                        if (CamRotateMode)
                        {
                            Vector3 camrot_e = CamOrbit.transform.rotation.eulerAngles;
                            camrot_e.x -= t.deltaPosition.y * CamRotSpeed;
                            camrot_e.y += t.deltaPosition.x * CamRotSpeed;

                            if (MaxCamRotateXValue < 0) MaxCamRotateXValue += 360;
                            if (MinCamRotateXValue < 0) MinCamRotateXValue += 360;
                            if (camrot_e.x < 0) camrot_e.x += 360;

                            //위에서
                            if (camrot_e.x <= 180)
                            {
                                if (camrot_e.x > MaxCamRotateXValue)
                                    camrot_e.x = MaxCamRotateXValue;
                            }
                            //아래에서
                            else if (camrot_e.x > 180)
                            {
                                if (camrot_e.x < MinCamRotateXValue)
                                    camrot_e.x = MinCamRotateXValue;
                            }

                            Quaternion camrot = new Quaternion();
                            camrot.eulerAngles = camrot_e;
                            CamOrbit.transform.rotation = camrot;
                        }
                        //캠 움직임 처리
                        else
                        {
                            Vector2 MoveValue = t.deltaPosition;
                            MoveValue.x = -MoveValue.x;
                            MoveValue.y = -MoveValue.y;
                            MoveValue.x *= CamMoveSpeed;
                            MoveValue.y *= CamMoveSpeed;

                            GameCamera.transform.Translate(MoveValue);

                            Vector3 GCPos = GameCamera.transform.localPosition;
                            if (GCPos.x > CamMaxMovePos.x)
                                GCPos.x = CamMaxMovePos.x;
                            else if (GCPos.x < CamMinMovePos.x)
                                GCPos.x = CamMinMovePos.x;
                            if (GCPos.y > CamMaxMovePos.y)
                                GCPos.y = CamMaxMovePos.y;
                            else if (GCPos.y < CamMinMovePos.y)
                                GCPos.y = CamMinMovePos.y;
                            GameCamera.transform.localPosition = GCPos;
                        }
                    }
                }

                //FingerID가 살아있는 mover의 Still On만을 true로 바꾸어준다.
                for (int index = 0; index < movers.Count; index++)
                {
                    Mover mover = (Mover)movers[index];
                    if (mover.GetFingetId() == t.fingerId)
                        stillon[index] = true;
                }

                if (ZoomCam)
                {
                    if (t.fingerId == RotateCamFingerID)
                        ZoomTouchpos1 = t.position;
                    if (t.fingerId == ZoomCamFingerID)
                        ZoomTouchpos2 = t.position;
                }
            }
        }
        #endregion

        //스틸로테이트캠(살아있지 않다면) 꺼줌
        if (!stillRotateCam)
        {
            if (ZoomCam)
                RotateCamFingerID = ZoomCamFingerID;
            else
                RotateCam = false;
            ZoomCam = false;
        }
        if (!stillZoomCam)
            ZoomCam = false;

        //줌 처리
        if (ZoomCam)
        {
            float Z = FirstZoomFingerDistance - GetTouchesDistaceByZPos(ZoomTouchpos1, ZoomTouchpos2, -20);
            Z *= CamZoomSpeed;
            Vector3 cpos = GameCamera.transform.localPosition;
            cpos.z = ZoomDefZ - Z;

            if (cpos.z > CamMaxZ) cpos.z = CamMaxZ;
            else if (cpos.z < CamMinZ) cpos.z = CamMinZ;


            GameCamera.transform.localPosition = cpos;
        }



        //FingerID가 살아있지 않은 Mover가 있다면 삭제.
        for (int index = 0; index < stillon.Length; index++)
        {
            Mover mover = (Mover)movers[index];
            if (stillon[index] == false)
            {
                mover.Destroy();
                movers.RemoveAt(index);
            }

        }

    }

}