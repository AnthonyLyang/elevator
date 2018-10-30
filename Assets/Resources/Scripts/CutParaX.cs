using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutParaX : MonoBehaviour {
    Mesh MeshA;
    Mesh MeshB;
    Mesh MeshParent;
    MeshCollider meshcollider;
    public float ZLength;
    public float XWidth;
    public float YHeight;
	// Use this for initialization
	void Start ()
    {
        meshcollider = GetComponent<MeshCollider>();
        MeshParent = GetComponent<MeshFilter>().mesh;
        MeshA = transform.GetChild(0).GetComponent<MeshFilter>().mesh;//前半块，调整前半块的后面
        MeshB = transform.GetChild(1).GetComponent<MeshFilter>().mesh;//后半块，调整前面
        CreateMesh();
        meshcollider.sharedMesh = MeshParent;
    }
	
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Triggered");
        var Slide = other.transform.position/*(other.transform.position - transform.position)*/;
        //获得新顶点在Z轴上的坐标分量
        //接下来分别对前半块的后面和后半块前面的顶点调整分量
        OnCutX(Slide);
        //接下来要判断后半块和前半块哪个更大，更大的留下替换parent的mesh，小的赋给坠落的object
        //通过比较顶面的宽度判断谁更大
        //GetComponent<MeshFilter>().mesh = CompareMesh(MeshA, MeshB);
        GetComponent<MeshFilter>().sharedMesh = CompareMesh(MeshA, MeshB);
        MeshParent = CompareMesh(MeshA, MeshB);
        meshcollider.sharedMesh = MeshParent;

        
        //生成坠落的块
        #region 以后此段改成利用对象池的方式，不新建对象了
        GameObject fallingobject = new GameObject();
        fallingobject.AddComponent<MeshFilter>();
        fallingobject.AddComponent<MeshRenderer>();
        fallingobject.AddComponent<MeshCollider>();
        fallingobject.AddComponent<Rigidbody>();
        #endregion
        if (MeshParent == MeshA)
        {
            fallingobject.GetComponent<MeshFilter>().sharedMesh = MeshB;
        }
        else fallingobject.GetComponent<MeshFilter>().sharedMesh = MeshA;
        fallingobject.GetComponent<MeshCollider>().sharedMesh = fallingobject.GetComponent<MeshFilter>().sharedMesh;
        fallingobject.GetComponent<MeshCollider>().convex = true;
        fallingobject.transform.SetParent(transform);
        //之后要更新meshA和meshB以等待下次切分
        MeshA = GetComponent<MeshFilter>().sharedMesh;
        MeshB = GetComponent<MeshFilter>().sharedMesh;
        fallingobject.transform.SetParent(null);
    }
    void OnCutX(Vector3 slide)
    {
        var Temp = MeshA.vertices;
        for (int i = 0; i < MeshA.vertices.Length; i++)
        {
            if (Temp[i].z > slide.z)
            {
                Temp[i].z = slide.z;
            }
        }
        MeshA.vertices = Temp;
        MeshA.uv = RenewUV(MeshA.vertices);
        Temp = MeshB.vertices;
        for (int i = 0; i < MeshB.vertices.Length; i++)
        {
            if (Temp[i].z <= slide.z)
            {
                Temp[i].z = slide.z;
            }
        }
        MeshB.vertices = Temp;
        MeshB.uv = RenewUV(MeshB.vertices);
        MeshA.RecalculateBounds();
        MeshA.RecalculateNormals();
        MeshA.RecalculateTangents();
        MeshB.RecalculateBounds();
        MeshB.RecalculateNormals();
        MeshB.RecalculateTangents();
    }
    Mesh CompareMesh(Mesh A,Mesh B)
    {
        if ((A.vertices[24] - A.vertices[25]).magnitude >= (B.vertices[24] - B.vertices[25]).magnitude)
        {
            return A;
        }
        else
        {
            return B;
        }
    }
    void CreateMesh()
    {
        var PosSilde = new Vector3(XWidth, YHeight, ZLength);
        var NewVertices = new Vector3[]
        {
            //front
            new Vector3(0, 0, 0), new Vector3(0, YHeight, 0), new Vector3(XWidth, YHeight, 0),new Vector3(XWidth, YHeight, 0), new Vector3(XWidth, 0, 0), new Vector3(0, 0, 0),
            //back
            new Vector3(0, 0, ZLength), new Vector3(XWidth, YHeight, ZLength), new Vector3(0, YHeight, ZLength),new Vector3(XWidth, YHeight, ZLength), new Vector3(0, 0, ZLength), new Vector3(XWidth, 0, ZLength),
            //right
            new Vector3(XWidth, 0, 0), new Vector3(XWidth, YHeight, 0), new Vector3(XWidth, 0, ZLength),new Vector3(XWidth, YHeight, ZLength), new Vector3(XWidth, 0, ZLength), new Vector3(XWidth, YHeight, 0),
            //left
            new Vector3(0, 0, 0), new Vector3(0, 0, ZLength), new Vector3(0, YHeight, 0),new Vector3(0, YHeight, ZLength), new Vector3(0, YHeight, 0), new Vector3(0, 0, ZLength),
            //top
            new Vector3(0, YHeight, 0), new Vector3(0, YHeight, ZLength), new Vector3(XWidth, YHeight, 0),new Vector3(XWidth, YHeight, 0), new Vector3(0, YHeight, ZLength), new Vector3(XWidth, YHeight, ZLength),
            //bottom
            new Vector3(0, 0, 0), new Vector3(XWidth, 0, 0), new Vector3(0, 0, ZLength),new Vector3(XWidth, 0, 0), new Vector3(XWidth, 0, ZLength), new Vector3(0, 0, ZLength)
        };
        var TriangleNumber = new int[NewVertices.Length];
        for (int i = 0; i < NewVertices.Length; i++)
        {
            NewVertices[i] -= PosSilde / 2f;
            TriangleNumber[i] = i;
        }
      
        MeshParent.vertices = NewVertices;
        MeshParent.triangles = TriangleNumber;
        MeshParent.RecalculateBounds();
        MeshParent.RecalculateNormals();
        MeshParent.RecalculateTangents();
        MeshParent.uv = RenewUV(NewVertices);



        MeshA.vertices = NewVertices;
        MeshA.triangles = TriangleNumber;
        MeshA.RecalculateBounds();
        MeshA.RecalculateNormals();
        MeshA.RecalculateTangents();
        MeshA.uv = RenewUV(NewVertices);


        MeshB.vertices = NewVertices;
        MeshB.triangles = TriangleNumber;
        MeshB.uv = RenewUV(NewVertices);
        MeshB.RecalculateBounds();
        MeshB.RecalculateNormals();
        MeshB.RecalculateTangents();
    }
    Vector2[] RenewUV(Vector3[]verticeinput)
    {
        var PosSilde = new Vector3(XWidth, YHeight, ZLength);

        var temp = new Vector3[verticeinput.Length];
        verticeinput.CopyTo(temp, 0);
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] += PosSilde / 2f;
        }
        var uvtemp = new Vector2[temp.Length];
        for (int i = 0; i < uvtemp.Length;)
        {
            if (temp[i].x == temp[i + 1].x && temp[i].x == temp[i + 2].x)
            {
                uvtemp[i] = new Vector2(temp[i].y/YHeight, temp[i].z/ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].y/YHeight, temp[i + 1].z/ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].y/YHeight, temp[i + 2].z/ZLength);

            }
            else if (temp[i].y == temp[i + 1].y && temp[i].y == temp[i + 2].y)
            {
                uvtemp[i] = new Vector2(temp[i].x/XWidth, temp[i].z/ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x/XWidth, temp[i + 1].z/ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x/XWidth, temp[i + 2].z/ZLength);
            }
            else if (temp[i].z == temp[i + 1].z && temp[i].z == temp[i + 2].z)
            {
                uvtemp[i] = new Vector2(temp[i].x/XWidth, temp[i].y/YHeight);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x/XWidth, temp[i + 1].y/YHeight);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x/XWidth, temp[i + 2].y/YHeight);
            }
            else
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].y / YHeight);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].y / YHeight);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].y / YHeight);
            }
            i += 3;
        }
        return uvtemp;
    }
}
