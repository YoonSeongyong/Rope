using LitJson;
using UnityEngine;
using System.Collections;


public class LevelMakerManager : MonoBehaviour
{
    public RopeManager RopeMgr;
    public TextMesh RopeLenthTextMesh;
    public MapImporter importer;

    public GameObject SavePopup;
    public TextMesh TitleText;
    public TextMesh SubTitleText;

    public TextMesh TitleText_View;
    public TextMesh SubTitleText_View;

    public class SaveData
    {
        public string Title;
        public string SubTitle;

        public int RopeLenght;
        public vec3[] RopePos;
        public vec3[] RopeRot;
    }

    public struct vec3
    {
        public double x;
        public double y;
        public double z;

        public vec3(Vector3 vec)
        {
            x = (double)vec.x;
            y = (double)vec.y;
            z = (double)vec.z;
        }
        public vec3(double _x, double _y, double _z)
        {
            x = _x;
            y = _y;
            z = _z;
        }

        public Vector3 ToVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }

    }

    public void Callback_Reset()
    {
        TitleText_View.text = "TITLE";
        SubTitleText_View.text = "SUB TITLE";

        RopeMgr.DeleteAllRope();
    }
    public void Callback_Create()
    {
        int _tryint;
        if (int.TryParse(RopeLenthTextMesh.text, out _tryint))
        {
            if (_tryint >= 3 && _tryint <= 50)
                RopeMgr.CreateRope(_tryint,2);
        }
    }

    void Start()
    {
        SavePopup.SetActive(false);
    }

    public string MakeMapJsonString()
    {
        SaveData info = new SaveData();
        info.Title = TitleText.text;
        info.SubTitle = SubTitleText.text;

        int ropelen = RopeMgr.RopeLenght;
        Vector3[] RopePos = RopeMgr.GetRopeObjPos();
        Vector3[] RopeRot = RopeMgr.GetRopeObjRot();

        info.RopeLenght = ropelen;
        info.RopePos = new vec3[ropelen];
        info.RopeRot = new vec3[ropelen];

        for (int i = 0; i < info.RopeLenght; i++)
        {
            info.RopePos[i] = new vec3(RopePos[i]);
            info.RopeRot[i] = new vec3(RopeRot[i]);
        }
        return JsonMapper.ToJson(info);
    }

    public void Callback_Save()
    {
        SavePopup.SetActive(true);
    }

    public void Callback_SavePopup_Save()
    {
        TitleText_View.text = TitleText.text;
        SubTitleText_View.text = SubTitleText.text;

        importer.SaveFile(TitleText.text, MakeMapJsonString());

        TitleText.text = "TITLE";
        SubTitleText.text = "SUB TITLE";


        SavePopup.SetActive(false);
    }

    public void Callback_SavePopup_Exit()
    {
        SavePopup.SetActive(false);
    }

    public void Callback_Load()
    {
        importer.OpenLoader();
    }

    public void Callback_Loader_Load(string json)
    {
        SaveData save = JsonMapper.ToObject<SaveData>(json);

        Vector3[] RopePos = new Vector3[save.RopeLenght];
        Quaternion[] RopeRot = new Quaternion[save.RopeLenght];
        for (int i = 0; i < save.RopeLenght; i++)
        {
            RopePos[i] = save.RopePos[i].ToVector3();
            RopeRot[i] = new Quaternion();
            RopeRot[i].eulerAngles = save.RopeRot[i].ToVector3();
        }
        int RopeLenght = save.RopeLenght;

        RopeMgr.CreateRope(RopeLenght, RopePos, RopeRot);

        TitleText_View.text = save.Title;
        SubTitleText_View.text = save.SubTitle;
    }

}
