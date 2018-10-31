using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EleSplit : MonoBehaviour
{
    public bool Fail;
    ObjectStorage storage;
    Mesh ParentMesh;//以后应该改写到GameMode里 MeshA和MeshB作为对象属性 初始化时深拷贝ParentMesh
    Mesh MeshA;
    Mesh MeshB;
    Mesh FallMesh;
    MeshFilter StayMesh;
    MeshCollider StayMeshCollider;

    GameObject FallingPart;

    MeshFilter FallFilter;
    MeshCollider FallCollider;

    Transform FallParent;

    Rigidbody rigid;

    private void Awake()
    {

    }
    // Use this for initialization
    void Start ()
    {
        
	}
    private void OnEnable()
    {
        rigid = GetComponent<Rigidbody>();
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        FallParent = transform.parent.parent.parent.GetChild(2);//这里永远是GameMode最初节点的child
        ParentMesh = new Mesh();
        MeshA = new Mesh();
        MeshB = new Mesh();
        FallMesh = new Mesh();
        StayMesh = GetComponent<MeshFilter>();
        StayMeshCollider = GetComponent<MeshCollider>();

        storage = ObjectStorage.Instance;
        ParentMesh = storage.DefaultMesh;
        storage.RenewMesh(ParentMesh, MeshA);
        storage.RenewMesh(ParentMesh, MeshB);
        StayMesh.mesh = ParentMesh;
        StayMeshCollider.sharedMesh = StayMesh.mesh;

        rigid.isKinematic = true;
        Fail = false;
    }

    // Update is called once per frame
    void Update ()
    {
		
	}
    private void OnTriggerExit(Collider other)
    {
        if (Fail)
        {
            return;
        }
        Debug.Log("Triggered");
        storage.PresentFall.Value.transform.SetParent(transform);
        storage.PresentFall.Value.transform.localPosition = Vector3.zero;
        FallingPart = storage.PresentFall.Value;
        FallFilter = storage.PresentInfo.Value.FallFilter;
        FallCollider = storage.PresentInfo.Value.FallCollider;
        storage.RenewFall();

        Debug.Log(other.transform.parent.localRotation.y);
        var Slide = (other.transform.position - transform.position);
        //这里要分两种情况，垂直X的和垂直Z的 先写了垂直Z的
        if (other.transform.parent.localRotation.eulerAngles.y == 90f)
        {
            OnCutZ(Slide);
            Debug.Log(StayMesh.mesh == null);
            storage.RenewMesh(CompareMeshParaZ(MeshA, MeshB), StayMesh.mesh);
        }
        else
        {
            OnCutX(Slide);
            Debug.Log(StayMesh.mesh == null);
            storage.RenewMesh(CompareMeshParaX(MeshA, MeshB), StayMesh.mesh);
        }
        FallFilter.mesh = FallMesh;
        FallCollider.sharedMesh = FallMesh;
        FallCollider.convex = true;
        FallingPart.SetActive(true);
        StartCoroutine(FallRecycle(FallingPart));
        //激活这个子物体，几秒后再关掉
        //关掉的时间应该小于第二次碰到激光的时间
        //或者建立第三个对象池用于存储fallingparts？
        storage.RenewMesh(StayMesh.mesh, MeshA);
        storage.RenewMesh(StayMesh.mesh, MeshB);
        StayMeshCollider.sharedMesh = StayMesh.mesh;
        StartCoroutine(EleRecycle());
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
        Temp = MeshB.vertices;
        for (int i = 0; i < MeshB.vertices.Length; i++)
        {
            if (Temp[i].z <= slide.z)
            {
                Temp[i].z = slide.z;
            }
        }
        MeshB.vertices = Temp;
        storage.RenewMesh(MeshA);
        storage.RenewMesh(MeshB);
    }
    void OnCutZ(Vector3 slide)
    {
        var Temp = MeshA.vertices;
        for (int i = 0; i < MeshA.vertices.Length; i++)
        {
            if (Temp[i].x > slide.x)
            {
                Temp[i].x = slide.x;
            }
        }
        MeshA.vertices = Temp;
        Temp = MeshB.vertices;
        for (int i = 0; i < MeshB.vertices.Length; i++)
        {
            if (Temp[i].x <= slide.x)
            {
                Temp[i].x = slide.x;
            }
        }
        MeshB.vertices = Temp;
        storage.RenewMesh(MeshA);
        storage.RenewMesh(MeshB);
    }
    Mesh CompareMeshParaX(Mesh A, Mesh B)
    {
        if ((A.vertices[24] - A.vertices[25]).magnitude >= (B.vertices[24] - B.vertices[25]).magnitude)
        {
            storage.RenewMesh(B, FallMesh);
            if((A.vertices[24] - A.vertices[25]).magnitude * (A.vertices[30] - A.vertices[31]).magnitude < 0.2 * storage.ZLength * storage.XWidth)
            {
                Fail = true;
            }
            return A;
        }
        else
        {
            storage.RenewMesh(A, FallMesh);
            if ((B.vertices[24] - B.vertices[25]).magnitude * (B.vertices[30] - B.vertices[31]).magnitude < 0.2 * storage.ZLength * storage.XWidth)
            {
                Fail = true;
            }
            return B;
        }
    }
    Mesh CompareMeshParaZ(Mesh A, Mesh B)
    {
        if ((A.vertices[30] - A.vertices[31]).magnitude >= (B.vertices[30] - B.vertices[31]).magnitude)
        {
            storage.RenewMesh(B, FallMesh);
            return A;
        }
        else
        {
            storage.RenewMesh(A, FallMesh);
            return B;
        }
    }
    //ele的销毁和回收
    IEnumerator EleRecycle()
    {
        Debug.Log(Fail);
        if (Fail)
        {
            rigid.isKinematic = false;
            yield return new WaitForSeconds(storage.FallSurvive+0.5f);
            gameObject.SetActive(false);
            storage.RenewQueue(gameObject);
        }
    }

    //fallingpart的销毁和回收
    IEnumerator FallRecycle(GameObject o)
    {
        o.transform.SetParent(FallParent);
        yield return new WaitForSeconds(storage.FallSurvive);
        o.transform.localPosition = Vector3.zero;
        o.transform.localRotation = Quaternion.identity;
        o.SetActive(false);
    }
}