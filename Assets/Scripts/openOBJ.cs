using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;

public class openOBJ : MonoBehaviour
{


    public Text _text;
    // Use this for initialization  
    void Start()
    {
        string path = "file:///storage/emulated/0/ss.obj";   //.obj文件在安卓sd卡路径 >>>>>待修改为手动选择文件
        DirectoryInfo s = new DirectoryInfo(path);
        if(!s.Exists)
        {
            _text.text = ".obj file not exists! ";
        }
        
        string _textureLink = " ";//纹理路径
        
        ObjReaderInSence obj = new ObjReaderInSence();
        StartCoroutine(obj.SomeFunction(path, _textureLink));  
        
        _text.text = "loading " + path + " ..."; 

    }

    // Update is called once per frame  
    void Update()
    {

    }
}