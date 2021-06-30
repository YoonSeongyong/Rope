using UnityEngine;
using System.Collections;
using System;
using System.IO;

public class MapImporter : MonoBehaviour
{
    public LevelMakerManager LevelMakermgr;

    public Camera UICamera;
    public TextMesh text;

    public Transform GUIPos;

    public GameObject CollsPanel;
    public Collider2D ScrollBox;

    private bool Open;
    private bool init;

    private Vector2 scrollPosition;
    private Vector2 scrollMax;

    private DirectoryInfo dI;

    // Use this for initialization
    void Start()
    {
        //LevelMaker/Save 경로 에 접근
        dI = null;
        dI = new DirectoryInfo(Application.persistentDataPath + "/LevelMaker/Save");

        text.text = "";

        #region Exists
        //만약 해당 경로가 존재하지 않으면
        if (!dI.Exists)
        {
            //생성 및 접근
            dI = new DirectoryInfo(Application.persistentDataPath);
            dI = dI.CreateSubdirectory("LevelMaker/Save");

            //해당 경로에 기본 Map파일 복사
            TextAsset[] bindata = Resources.LoadAll<TextAsset>("LevelMaker/Save");
            foreach (TextAsset mapfile in bindata)
            {
                FileInfo file = new FileInfo(dI.FullName + "/" + mapfile.name + ".gkmap");

                FileStream fs = file.Create();
                BinaryWriter bw = new BinaryWriter(fs, System.Text.Encoding.UTF8);

                bw.Write(mapfile.bytes);

                bw.Close();
                fs.Close();
            }
        }
        #endregion

        Open = false;

        scrollPosition = new Vector2();
        scrollMax = new Vector2();

        CollsPanel.SetActive(Open);

        init = true;
    }

    public void OpenLoader()
    {
        Open = true;
        CollsPanel.SetActive(Open);
    }
    public void CloseLoader()
    {
        Open = false;
        CollsPanel.SetActive(Open);
    }

    public void SaveFile(string filename, string script)
    {
        string scr = dI.FullName + "/" + filename;

        FileInfo file = new FileInfo(scr + ".gkmap");
        while (file.Exists)
        {
            scr += "_NEW";
            file = new FileInfo(scr + ".gkmap");
        }

        FileStream fs = file.Create();

        StreamWriter sw = new StreamWriter(fs);

        sw.Write(script);

        sw.Close();
        fs.Close();
    }

    public Rect WorldToGuiRect(float x, float y, float width, float height, float Anchorx = 0.5f, float Anchory = 0.5f)
    {
        return WorldToGuiRect(new Rect(x, y, width, height), new Vector2(Anchorx, Anchory));
    }
    public Rect WorldToGuiRect(Rect rect, Vector2 AnchorPoint)
    {
        rect.x += GUIPos.localPosition.x - (AnchorPoint.x * rect.width);
        rect.y += UICamera.transform.position.y + (rect.height - (AnchorPoint.y * rect.height)) + GUIPos.localPosition.y;

        Vector2 guiPosition = UICamera.WorldToScreenPoint(rect.position);
        Vector2 guiSize = UICamera.WorldToScreenPoint(rect.position + rect.size);
        guiSize = guiSize - guiPosition;

        guiPosition.y = Screen.height - guiPosition.y;

        return new Rect(guiPosition.x, guiPosition.y, guiSize.x, guiSize.y);
    }

    void LateUpdate()
    {
        if (!Open)
            return;

        if (scrollPosition.y < 0) scrollPosition.y = 0;
        if (scrollPosition.y != scrollMax.y) scrollPosition.y = scrollMax.y;

    }

    void OnGUI()
    {
        if (!Open || !init)
            return;

        int h = Screen.height;
        GUI.skin.box.fontSize = (int)(0.04f * h);
        GUI.skin.button.fontSize = (int)(0.04f * h);

        GUI.skin.verticalScrollbar.fixedWidth = (int)(0.07f * h);
        GUI.skin.verticalScrollbarThumb.fixedWidth = (int)(0.07f * h);

        GUI.Box(WorldToGuiRect(0, 0, 16, 8.5f), "LOAD MAP");

        if (GUI.Button(WorldToGuiRect(0, 3.0f, 15, 0.8f), "종료"))
            CloseLoader();

        GUILayout.BeginArea(WorldToGuiRect(0, -0.5f, 15, 5.5f));
            scrollMax = GUILayout.BeginScrollView(scrollPosition);

            foreach (FileInfo item in dI.GetFiles())
            {
                if (item.Extension == ".gkmap")
                {
                    if (GUILayout.Button(new GUIContent(item.Name)))
                    {
                        StreamReader stream = item.OpenText();
                        string str = stream.ReadToEnd();
                        stream.Close();

                        LevelMakermgr.Callback_Loader_Load(str);
                    }
                }
            }
            GUILayout.EndScrollView();
        GUILayout.EndArea();
    }

}