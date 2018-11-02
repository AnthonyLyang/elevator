using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectManager : MonoBehaviour
{
    //设定物体生成高度差的参数
    public float Height;
    float heighttemp;

    //设定mesh大小的参数
    public float ZLength;
    public float XWidth;
    public float YHeight;

    //Laser刷新时间的参数
    public float LaserSurviveTime;
    //ele刷新时间的参数
    public float EleRefreshTime;
    //下落物生存时间的参数
    public float FallSurvivalTime;


    ObjectStorage storage;
    GameObject[] Elevators;
    GameObject[] Lasers;
    GameObject[] Lasers2;

    //可能player的character也要在这里进行管理以方便访问
    //同样也要存到ObjectStorage里
    private void Awake()
    {
        storage = ObjectStorage.Instance;
        CreateParentMeshCube();
        var ElevatorGroup = transform.GetChild(0);
        var LaserGroup = transform.GetChild(1);
        var Laser2Group = transform.GetChild(2);
        //加另一个朝向的射线
        var count = ElevatorGroup.childCount;
        Elevators = new GameObject[count];
        Lasers = new GameObject[count];
        Lasers2 = new GameObject[count];
        for (int i = 0; i < count; i++)
        {
            Elevators[i] = ElevatorGroup.GetChild(i).gameObject;
            Lasers[i] = LaserGroup.GetChild(i).gameObject;
            Lasers2[i] = Laser2Group.GetChild(i).gameObject;
        }
        for (int i = 0; i < count; i++)
        {
            storage.Lasers.AddLast(Lasers[i]);
            storage.Lasers2.AddLast(Lasers2[i]);
            storage.RenewQueue(Elevators[i]);
        }
        storage.PresentLaser = storage.Lasers.First;
        storage.PresentLaser2 = storage.Lasers2.First;
        //到时候还要写一个背景对象池更新
        //以上初始化对象池信息

        //初始化初始Mesh
        storage.XWidth = XWidth;
        storage.YHeight = YHeight;
        storage.ZLength = ZLength;
        storage.FallSurvive = FallSurvivalTime;
        CreateParentMeshCube();

        //初始化FallingParts序列
        var FallingGroup = transform.GetChild(3);
        for (int j = 0; j < FallingGroup.childCount; j++)
        {
            var fallpart = FallingGroup.GetChild(j).gameObject;
            storage.Fallings.AddLast(fallpart);
            storage.Infos.AddLast(new FallInfo(fallpart));
        }
        storage.PresentFall = storage.Fallings.First;
        storage.PresentInfo = storage.Infos.First;

        //加入Player
        storage.Player = transform.GetChild(4).gameObject;
        storage.PlayerStat = storage.Player.GetComponent<TransformControllTest>();
    }
    // Use this for initialization
    void Start ()
    {
        StartCoroutine(InstallLasers(storage.PresentLaser));
        StartCoroutine(ResetLasers(storage.Lasers));
        StartCoroutine(InstallElevator());
        StartCoroutine(InstallLasers(storage.PresentLaser2));
        StartCoroutine(ResetLasers(storage.Lasers2));

        //Invoke("StartAnother", 3f);
    }	
	// Update is called once per frame
	void Update ()
    {
		
	}
    

    //双向激光之后可能刷新激光的机制要重写

    IEnumerator InstallLasers(LinkedListNode<GameObject> node)
    {
        while (node != null)
        {
            if (node.Next != null)
            {
                SetLaserHeight(node);
                node = node.Next;
            }
            yield return 0;
        }
    }
    IEnumerator ResetLasers(LinkedList<GameObject> list)
    {
        while (true)
        {
            yield return new WaitForSeconds(LaserSurviveTime);
            storage.RenewList(list);
        }
    }//刷新激光的协程
    IEnumerator InstallElevator()
    {
        while (true)
        {
            if (storage.EnableElevators.Count > 0)
            {
                var PreEle = storage.EnableElevators.Dequeue();
                PreEle.SetActive(true);
            }
            yield return new WaitForSeconds(EleRefreshTime);
        }
    }
    void CreateParentMeshCube()
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
        storage.DefaultMesh.vertices = NewVertices;
        storage.DefaultMesh.triangles = TriangleNumber;
        var uvtemp = storage.RenewUV(NewVertices);
        storage.DefaultMesh.uv = uvtemp;

        //uvtemp.CopyTo(ParentMesh.uv,0);
        storage.DefaultMesh.RecalculateBounds();
        storage.DefaultMesh.RecalculateNormals();
        storage.DefaultMesh.RecalculateTangents();
    }
    void SetLaserHeight(LinkedListNode<GameObject> lasernode)
    {
        //这个函数以后要改成根据玩家的位置刷新激光
        //按照固定的高度差刷新一定会导致电梯与激光的高度最终永久错开
        //就再也碰不到了
        heighttemp += Height;
        var height = new Vector3(0, heighttemp, Random.Range(-ZLength / 2f, ZLength / 2f));
        lasernode.Value.transform.position = height;
        lasernode.Value.SetActive(true);
    }


    void StartAnother()
    {
        StartCoroutine(InstallLasers(storage.PresentLaser2));
        StartCoroutine(ResetLasers(storage.Lasers2));
    }
}

public class ObjectStorage
{
    //这个类负责储存和更新游戏中所有重复出现的动态对象
    private static ObjectStorage mInstance;
    public static ObjectStorage Instance
    {
        get
        {
            if (mInstance == null)
            {
                mInstance = new ObjectStorage();
            }
            return mInstance;
        }
    }//单例
    private ObjectStorage()
    {
        DefaultMesh = new Mesh();
        Lasers = new LinkedList<GameObject>();
        Lasers2 = new LinkedList<GameObject>();
        EnableElevators = new Queue<GameObject>();
        Fallings = new LinkedList<GameObject>();
        Infos = new LinkedList<FallInfo>();

    }//构造函数
    public Mesh DefaultMesh;//初始的正方体Mesh在这里保存，由外界脚本导入
    public float ZLength;
    public float XWidth;
    public float YHeight;//Mesh的长宽高参数，因为是立方体mesh

    public float FallSurvive;
    //角色控制器信息
    public GameObject Player;
    public TransformControllTest PlayerStat;
    public LinkedList<GameObject> Lasers;
    public LinkedListNode<GameObject> PresentLaser;
    public LinkedList<GameObject> Lasers2;
    public LinkedListNode<GameObject> PresentLaser2;
    public LinkedListNode<GameObject> RenewList(LinkedList<GameObject>list)
    {
        var temp = list.First;
        temp.Value.SetActive(false);
        //在这里调用一个重置对象的函数
        //不用了，直接inactive就行了
        list.RemoveFirst();
        list.AddLast(temp);
        return list.First;
    }
    //激光和背景的对象循环用链表肯定没有问题，但是电梯板的对象循环不一定能保证正确
    //理论 尝试：队列
    //激活的新物体执行一个协程，该销毁时自己回到队列中调用RenewQueue
    //需要激活新物体时再从队列中取 没有不取
    public Queue<GameObject> EnableElevators;
    public void RenewQueue(GameObject input)
    {
        EnableElevators.Enqueue(input);
    }
    //同样用一个链表储存fallingpieces
    //需要多一些的fallingpiecies对象
    //并且需要另一个链表储存fallingpieces的信息
    public LinkedList<GameObject> Fallings;
    public LinkedList<FallInfo> Infos;
    public LinkedListNode<GameObject> PresentFall;
    public LinkedListNode<FallInfo> PresentInfo;
    //每次有新的块被切下时这两个链表更新
    public void RenewFall()
    {
        var tempA = Fallings.First;
        var tempB = Infos.First;
        Fallings.RemoveFirst();
        Infos.RemoveFirst();
        Fallings.AddLast(tempA);
        Infos.AddLast(tempB);
        PresentFall = Fallings.First;
        PresentInfo = Infos.First;
    }

    //更新mesh的函数
    public Vector2[] RenewUV(Vector3[] verticeinput)
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
                uvtemp[i] = new Vector2(temp[i].y / YHeight, temp[i].z / ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].y / YHeight, temp[i + 1].z / ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].y / YHeight, temp[i + 2].z / ZLength);

            }
            else if (temp[i].y == temp[i + 1].y && temp[i].y == temp[i + 2].y)
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].z / ZLength);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].z / ZLength);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].z / ZLength);
            }
            else if (temp[i].z == temp[i + 1].z && temp[i].z == temp[i + 2].z)
            {
                uvtemp[i] = new Vector2(temp[i].x / XWidth, temp[i].y / YHeight);
                uvtemp[i + 1] = new Vector2(temp[i + 1].x / XWidth, temp[i + 1].y / YHeight);
                uvtemp[i + 2] = new Vector2(temp[i + 2].x / XWidth, temp[i + 2].y / YHeight);
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
    public void RenewMesh(Mesh meshtorenew)
    {
        meshtorenew.uv = RenewUV(meshtorenew.vertices);
        meshtorenew.RecalculateBounds();
        meshtorenew.RecalculateNormals();
        meshtorenew.RecalculateTangents();
    }
    public void RenewMesh(Mesh input, Mesh meshtorenew)
    {
        var temp = new Vector3[input.vertices.Length];
        var tritemp = new int[input.triangles.Length];
        input.vertices.CopyTo(temp, 0);
        input.triangles.CopyTo(tritemp, 0);
        meshtorenew.vertices = temp;
        meshtorenew.triangles = tritemp;
        meshtorenew.uv = RenewUV(meshtorenew.vertices);
        meshtorenew.RecalculateBounds();
        meshtorenew.RecalculateNormals();
        meshtorenew.RecalculateTangents();
    }



}

public class FallInfo
{
    public MeshFilter FallFilter;
    public MeshCollider FallCollider;
    public Rigidbody Fallbody;

    public FallInfo(GameObject o)
    {
        FallFilter = o.GetComponent<MeshFilter>();
        FallCollider = o.GetComponent<MeshCollider>();
    }
}






