using UnityEngine;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine.UI;

public class ObjReaderInSence : MonoBehaviour
{
    Mesh _myMesh;
    Material _myMaterial;


    public Material dMaterial;

    public GameObject center;


    Vector3[] _vertexArray;
    ArrayList _vertexArrayList = new ArrayList();
    Vector3[] _normalArray;
    ArrayList _normalArrayList = new ArrayList();
    Vector2[] _uvArray;
    ArrayList _uvArrayList = new ArrayList();

    int[] _triangleArray;

    ArrayList _facesVertNormUV = new ArrayList();

    void Start()
    {
        _myMaterial = new Material(Shader.Find("Diffuse"));
    }

    internal class PlacesByIndex
    {
        public PlacesByIndex(int index)
        {
            _index = index;
        }
        public int _index;
        public ArrayList _places = new ArrayList();
    }
    void initArrayLists()
    {
        _uvArrayList = new ArrayList();
        _normalArrayList = new ArrayList();
        _vertexArrayList = new ArrayList();
        _facesVertNormUV = new ArrayList();
    }

    public IEnumerator SomeFunction(string path, string _textureLink)
    {
        GameObject obj_gameobject = new GameObject();
        obj_gameobject.name = path;
        initArrayLists();
        if (_myMesh != null)
            _myMesh.Clear();
        _myMesh = new Mesh();
        _myMesh.name = path;
        WWW www3d = new WWW(path);
        yield return www3d;
        string s = www3d.text;
        Debug.Log(s);
        s = s.Replace("  ", " ");
        s = s.Replace("  ", " ");
        LoadFile(s);
        _myMesh.vertices = _vertexArray;
        _myMesh.triangles = _triangleArray;
        if (_uvArrayList.Count > 0)
            _myMesh.uv = _uvArray;
        if (_normalArrayList.Count > 0)
            _myMesh.normals = _normalArray;
        else
            _myMesh.RecalculateNormals();
        _myMesh.RecalculateBounds();
        if ((MeshFilter)obj_gameobject.GetComponent("MeshFilter") == null)
            obj_gameobject.AddComponent<MeshFilter>();
        MeshFilter temp;
        temp = (MeshFilter)obj_gameobject.GetComponent("MeshFilter");
        temp.mesh = _myMesh;
        if ((MeshRenderer)obj_gameobject.GetComponent("MeshRenderer") == null)
        {
            obj_gameobject.AddComponent<MeshRenderer>();
            obj_gameobject.GetComponent<MeshRenderer>().materials[0] = dMaterial;
        }
        if (_uvArrayList.Count > 0 && _textureLink != "")
        {
            WWW wwwtx = new WWW(_textureLink);
            yield return wwwtx;
            _myMaterial.mainTexture = wwwtx.texture;
            //dMaterial.mainTexture = wwwtx.texture;
        }
        MeshRenderer temp2;
        temp2 = (MeshRenderer)obj_gameobject.GetComponent("MeshRenderer");
        if (_uvArrayList.Count > 0 && _textureLink != "")
        {
            //temp2.material = _myMaterial;
            temp2.material = dMaterial;
            _myMaterial.shader = Shader.Find("Diffuse");
        }
       
        //添加父对象以改变旋转中心
        GameObject a = new GameObject();
        a = GameObject.Find("Cube");
        obj_gameobject.transform.parent = a.transform;
        //obj_gameobject.AddComponent<objCtrl>();
        yield return new WaitForFixedUpdate();
    }

    public void LoadFile(string s)
    {
        string[] lines = s.Split("\n"[0]);

        foreach (string item in lines)
        {
            ReadLine(item);
        }
        ArrayList tempArrayList = new ArrayList();
        for (int i = 0; i < _facesVertNormUV.Count; ++i)
        {
            if (_facesVertNormUV[i] != null)
            {
                PlacesByIndex indextemp = new PlacesByIndex(i);
                indextemp._places.Add(i);
                for (int j = 0; j < _facesVertNormUV.Count; ++j)
                {
                    if (_facesVertNormUV[j] != null)
                    {
                        if (i != j)
                        {
                            Vector3 iTemp = (Vector3)_facesVertNormUV[i];
                            Vector3 jTemp = (Vector3)_facesVertNormUV[j];
                            if (iTemp.x == jTemp.x && iTemp.y == jTemp.y)
                            {
                                indextemp._places.Add(j);
                                _facesVertNormUV[j] = null;
                            }
                        }
                    }
                }
                tempArrayList.Add(indextemp);
            }
        }
        _vertexArray = new Vector3[tempArrayList.Count];
        _uvArray = new Vector2[tempArrayList.Count];
        _normalArray = new Vector3[tempArrayList.Count];
        _triangleArray = new int[_facesVertNormUV.Count];
        int teller = 0;
        foreach (PlacesByIndex item in tempArrayList)
        {
            foreach (int item2 in item._places)
            {
                _triangleArray[item2] = teller;
            }
            Vector3 vTemp = (Vector3)_facesVertNormUV[item._index];
            _vertexArray[teller] = (Vector3)_vertexArrayList[(int)vTemp.x - 1];
            if (_uvArrayList.Count > 0)
            {
                Vector3 tVec = (Vector3)_uvArrayList[(int)vTemp.y - 1];
                _uvArray[teller] = new Vector2(tVec.x, tVec.y);
            }
            if (_normalArrayList.Count > 0)
            {
                _normalArray[teller] = (Vector3)_normalArrayList[(int)vTemp.z - 1];
            }
            teller++;
        }
    }

    public void ReadLine(string s)
    {
        char[] charsToTrim = { ' ', '\n', '\t', '\r' };
        s = s.TrimEnd(charsToTrim);
        string[] words = s.Split(" "[0]);
        foreach (string item in words)
            item.Trim();
        if (words[0] == "v")
            _vertexArrayList.Add(new Vector3(System.Convert.ToSingle(words[1], CultureInfo.InvariantCulture), System.Convert.ToSingle(words[2], CultureInfo.InvariantCulture), System.Convert.ToSingle(words[3], CultureInfo.InvariantCulture)));

        if (words[0] == "vn")
            _normalArrayList.Add(new Vector3(System.Convert.ToSingle(words[1], CultureInfo.InvariantCulture), System.Convert.ToSingle(words[2], CultureInfo.InvariantCulture), System.Convert.ToSingle(words[3], CultureInfo.InvariantCulture)));
        if (words[0] == "vt")
            _uvArrayList.Add(new Vector3(System.Convert.ToSingle(words[1], CultureInfo.InvariantCulture), System.Convert.ToSingle(words[2], CultureInfo.InvariantCulture)));
        if (words[0] == "f")
        {
            ArrayList temp = new ArrayList();
            ArrayList triangleList = new ArrayList();
            for (int j = 1; j < words.Length; ++j)
            {
                Vector3 indexVector = new Vector3(0, 0);
                string[] indices = words[j].Split("/"[0]);
                indexVector.x = System.Convert.ToInt32(indices[0], CultureInfo.InvariantCulture);
                if (indices.Length > 1)
                {
                    if (indices[1] != "")
                        indexVector.y = System.Convert.ToInt32(indices[1], CultureInfo.InvariantCulture);
                }
                if (indices.Length > 2)
                {
                    if (indices[2] != "")
                        indexVector.z = System.Convert.ToInt32(indices[2], CultureInfo.InvariantCulture);
                }
                temp.Add(indexVector);
            }
            for (int i = 1; i < temp.Count - 1; ++i)
            {
                triangleList.Add(temp[0]);
                triangleList.Add(temp[i]);
                triangleList.Add(temp[i + 1]);
            }

            foreach (Vector3 item in triangleList)
            {
                _facesVertNormUV.Add(item);
            }
        }
    }
}
