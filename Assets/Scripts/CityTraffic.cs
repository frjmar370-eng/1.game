using UnityEngine;

public class CityTraffic : MonoBehaviour
{
    public float speed = 6f;
    public float despawnEdge = 142f;
    Vector3 axis;

    void Start(){ axis = transform.forward; }

    void Update()
    {
        transform.position += axis * speed * Time.deltaTime;
        Vector3 p = transform.position;
        if (Mathf.Abs(p.x) > despawnEdge || Mathf.Abs(p.z) > despawnEdge)
        {
            if (Mathf.Abs(axis.x) > .5f) p.x = -Mathf.Sign(axis.x) * 138f;
            else p.z = -Mathf.Sign(axis.z) * 138f;
            transform.position = p;
        }
    }
}

public static class CityTrafficBootstrap
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void SpawnTraffic()
    {
        GameObject prefab = Resources.Load<GameObject>("Car");
        if (!prefab) return;
        float[] lanes = { -120f, -40f, 40f, 120f };
        for (int i=0;i<lanes.Length;i++)
        {
            Spawn(prefab,new Vector3(lanes[i],.25f,-105f),Quaternion.identity,5.5f+i);
            Spawn(prefab,new Vector3(-105f,.25f,lanes[i]),Quaternion.Euler(0,90,0),5f+i);
        }
    }
    static void Spawn(GameObject prefab,Vector3 pos,Quaternion rot,float speed)
    {
        GameObject car=Object.Instantiate(prefab,pos,rot);
        car.name="TrafficCar";
        car.AddComponent<CityTraffic>().speed=speed;
    }
}
