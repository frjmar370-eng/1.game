using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class CityTestMap : MonoBehaviour
{
    public static CityTestMap Instance;
    readonly List<GameObject> spawned = new List<GameObject>();
    Material road, sidewalk, asphalt, concrete, glass, brick, metal, tree, grass, window, vehicle;

    public void Build()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        CreateMaterials();
        BuildLighting();
        BuildGround();
        BuildRoadNetwork();
        BuildBuildings();
        BuildProps();
        BuildPlayerStart();
        BuildSigns();
    }

    void CreateMaterials()
    {
        road=Mat("Road",new Color(.055f,.06f,.065f));
        sidewalk=Mat("Sidewalk",new Color(.27f,.28f,.29f));
        asphalt=Mat("Asphalt",new Color(.11f,.12f,.13f));
        concrete=Mat("Concrete",new Color(.48f,.49f,.5f));
        glass=Mat("Glass",new Color(.05f,.16f,.22f),.25f);
        brick=Mat("Brick",new Color(.32f,.22f,.18f));
        metal=Mat("Metal",new Color(.2f,.23f,.26f),.55f);
        tree=Mat("Tree",new Color(.08f,.26f,.12f));
        grass=Mat("Grass",new Color(.12f,.25f,.1f));
        window=Mat("Windows",new Color(.08f,.42f,.55f),.15f);
        vehicle=Mat("Vehicle",new Color(.12f,.15f,.18f));
    }

    Material Mat(string n, Color c, float metallic=0f)
    {
        var m=new Material(Shader.Find("Standard")); m.name=n; m.color=c; m.SetFloat("_Metallic",metallic); m.SetFloat("_Glossiness",.55f); return m;
    }

    void BuildLighting()
    {
        RenderSettings.ambientMode=UnityEngine.Rendering.AmbientMode.Flat;
        RenderSettings.ambientLight=new Color(.32f,.36f,.42f);
        RenderSettings.fog=true; RenderSettings.fogColor=new Color(.45f,.5f,.55f); RenderSettings.fogStartDistance=45; RenderSettings.fogEndDistance=180;
        var sun=new GameObject("City Sun"); spawned.Add(sun); var l=sun.AddComponent<Light>(); l.type=LightType.Directional; l.intensity=1.15f; l.transform.rotation=Quaternion.Euler(48,-28,0);
    }

    void BuildGround()
    {
        var g=GameObject.CreatePrimitive(PrimitiveType.Plane); g.name="City Terrain"; g.transform.position=new Vector3(0,-.08f,0); g.transform.localScale=new Vector3(14,1,14); g.GetComponent<Renderer>().sharedMaterial=grass; spawned.Add(g);
        var center=GameObject.CreatePrimitive(PrimitiveType.Plane); center.name="City Asphalt District"; center.transform.position=new Vector3(0,-.045f,0); center.transform.localScale=new Vector3(11,1,11); center.GetComponent<Renderer>().sharedMaterial=asphalt; spawned.Add(center);
    }

    void BuildRoadNetwork()
    {
        for(int i=-2;i<=2;i++)
        {
            float p=i*22f;
            Road(new Vector3(p,.01f,0),new Vector3(7f,.04f,130f));
            Road(new Vector3(0,.02f,p),new Vector3(130f,.04f,7f));
            for(int s=-2;s<=2;s++)
            {
                float q=s*22f;
                SidewalkStrip(new Vector3(p-4,.04f,q),new Vector3(1.4f,.08f,15f));
                SidewalkStrip(new Vector3(p+4,.04f,q),new Vector3(1.4f,.08f,15f));
                SidewalkStrip(new Vector3(q,.04f,p-4),new Vector3(15f,.08f,1.4f));
                SidewalkStrip(new Vector3(q,.04f,p+4),new Vector3(15f,.08f,1.4f));
            }
        }
        for(int i=-2;i<=2;i++) for(int j=-2;j<=2;j++)
        {
            float x=i*22f,z=j*22f;
            for(int k=-2;k<=2;k++)
            {
                float d=k*2.1f; LineMark(new Vector3(x+d,.055f,z),new Vector3(.12f,.015f,1.0f));
                LineMark(new Vector3(x,.055f,z+d),new Vector3(1.0f,.015f,.12f));
            }
        }
    }

    void Road(Vector3 p,Vector3 s){var g=Primitive(PrimitiveType.Cube,"Road",p,s,road);spawned.Add(g);}
    void SidewalkStrip(Vector3 p,Vector3 s){var g=Primitive(PrimitiveType.Cube,"Sidewalk",p,s,sidewalk);spawned.Add(g);}
    void LineMark(Vector3 p,Vector3 s){var g=Primitive(PrimitiveType.Cube,"Lane Mark",p,s,concrete);spawned.Add(g);}

    void BuildBuildings()
    {
        int index=0;
        for(int gx=-2;gx<=2;gx++) for(int gz=-2;gz<=2;gz++)
        {
            if(gx==0 && gz==0) continue;
            float bx=gx*22f, bz=gz*22f;
            for(int b=0;b<3;b++)
            {
                float x=bx + (b-1)*5.1f + ((gz%2)*.5f); float z=bz + ((b%2==0)?-3.8f:3.8f);
                float w=7.0f + ((index*3)%3), d=6.0f + ((index*5)%3), h=7f + ((index*11)%18);
                Building("Building_"+index++,new Vector3(x,h*.5f,z),new Vector3(w,h,d),index%3);
            }
        }
        Building("Central Tower",new Vector3(0,10,0),new Vector3(14,20,14),2);
        Building("Central Tower Annex",new Vector3(9,4,0),new Vector3(3,8,10),1);
    }

    void Building(string name,Vector3 p,Vector3 size,int style)
    {
        GameObject root=new GameObject(name); root.transform.position=p; spawned.Add(root);
        MeshFilter mf=root.AddComponent<MeshFilter>(); MeshRenderer mr=root.AddComponent<MeshRenderer>(); mf.sharedMesh=BoxMesh(size); mr.sharedMaterial=style==0?concrete:(style==1?brick:metal);
        BoxCollider bc=root.AddComponent<BoxCollider>(); bc.size=size;
        int floors=Mathf.Max(2,Mathf.FloorToInt(size.y/3f));
        for(int f=0;f<floors;f++)
        {
            float y=-size.y*.5f+1.25f+f*3f;
            for(int side=-1;side<=1;side+=2)
            {
                for(int i=-2;i<=2;i++)
                {
                    float x=i*(size.x*.18f); var w=Primitive(PrimitiveType.Quad,"Window",root.transform.position+new Vector3(x,y,side*(size.z*.505f)),new Vector3(size.x*.12f,1.25f,.03f),window); w.transform.rotation=Quaternion.Euler(side<0?90:90,0,0); spawned.Add(w);
                }
            }
        }
        var roof=Primitive(PrimitiveType.Cylinder,"RoofUnit",p+Vector3.up*(size.y*.5f+.7f),new Vector3(1.2f,.7f,1.2f),metal); spawned.Add(roof);
    }

    Mesh BoxMesh(Vector3 s)
    {
        Vector3 h=s*.5f; Vector3[] v={new(-h.x,-h.y,-h.z),new(h.x,-h.y,-h.z),new(h.x,-h.y,h.z),new(-h.x,-h.y,h.z),new(-h.x,h.y,-h.z),new(h.x,h.y,-h.z),new(h.x,h.y,h.z),new(-h.x,h.y,h.z)};
        int[] t={0,2,1,0,3,2,4,5,6,4,6,7,0,1,5,0,5,4,1,2,6,1,6,5,2,3,7,2,7,6,3,0,4,3,4,7};
        var m=new Mesh();m.vertices=v;m.triangles=t;m.RecalculateNormals();return m;
    }

    void BuildProps()
    {
        for(int x=-2;x<=2;x++) for(int z=-2;z<=2;z++)
        {
            for(int k=-1;k<=1;k++)
            {
                float px=x*22f+k*6.2f,pz=z*22f+((k+1)%2)*4.5f;
                StreetLight(new Vector3(px,0,pz));
                if((x+z+k)%2==0) Tree(new Vector3(px+1.5f,0,pz+1.3f));
            }
            if(x!=0 || z!=0) Car(new Vector3(x*22f+2.2f,.45f,z*22f-1.2f),x%2==0);
        }
        for(int i=-2;i<=2;i++)
        {
            Barrier(new Vector3(i*22f,1.1f,9)); Barrier(new Vector3(i*22f,1.1f,-9));
        }
    }

    void StreetLight(Vector3 p)
    {
        var pole=Primitive(PrimitiveType.Cylinder,"Street Light",p+Vector3.up*2.2f,new Vector3(.09f,2.2f,.09f),metal);spawned.Add(pole);
        var arm=Primitive(PrimitiveType.Cylinder,"Light Arm",p+Vector3.up*4.25f+Vector3.right*.55f,new Vector3(.07f,.55f,.07f),metal);arm.transform.rotation=Quaternion.Euler(0,0,90);spawned.Add(arm);
        var lamp=Primitive(PrimitiveType.Sphere,"Lamp",p+Vector3.up*4.25f+Vector3.right*1.0f,new Vector3(.28f,.16f,.28f),window);spawned.Add(lamp);
        var l=lamp.AddComponent<Light>();l.type=LightType.Point;l.range=7;l.intensity=2.2f;l.color=new Color(.65f,.85f,1f);
    }

    void Tree(Vector3 p)
    {
        var trunk=Primitive(PrimitiveType.Cylinder,"Tree Trunk",p+Vector3.up*1.2f,new Vector3(.35f,1.2f,.35f),brick);spawned.Add(trunk);
        var crown=Primitive(PrimitiveType.Sphere,"Tree Crown",p+Vector3.up*3.0f,new Vector3(1.8f,2.2f,1.8f),tree);spawned.Add(crown);
    }

    void Car(Vector3 p,bool flip)
    {
        GameObject root=new GameObject("Parked Vehicle");root.transform.position=p;root.transform.rotation=Quaternion.Euler(0,flip?90:0,0);spawned.Add(root);
        Primitive(PrimitiveType.Cube,"Car Body",p+Vector3.up*.25f,new Vector3(2.8f,.5f,1.35f),vehicle,root.transform);
        Primitive(PrimitiveType.Cube,"Car Cabin",p+Vector3.up*.72f,new Vector3(1.45f,.55f,1.05f),glass,root.transform);
        for(int i=-1;i<=1;i+=2) for(int j=-1;j<=1;j+=2) { var w=Primitive(PrimitiveType.Cylinder,"Wheel",p+new Vector3(i*1.0f,-.05f,j*.7f),new Vector3(.34f,.16f,.34f),metal,root.transform); w.transform.rotation=Quaternion.Euler(90,0,0); }
    }

    void Barrier(Vector3 p){var a=Primitive(PrimitiveType.Cylinder,"Barrier",p+Vector3.up*.65f,new Vector3(.18f,.65f,.18f),metal);spawned.Add(a);var b=Primitive(PrimitiveType.Cylinder,"Barrier",p+Vector3.up*.65f+Vector3.right*1.2f,new Vector3(.18f,.65f,.18f),metal);spawned.Add(b);var c=Primitive(PrimitiveType.Cube,"Barrier Rail",p+Vector3.up*1.1f+Vector3.right*.6f,new Vector3(1.35f,.1f,.1f),window);spawned.Add(c);}

    void BuildSigns()
    {
        for(int i=-2;i<=2;i++) Sign(new Vector3(i*22f,2.3f,7.3f),"DISTRICT "+(i+3));
        Sign(new Vector3(0,3f,14f),"CITY CENTER");
    }
    void Sign(Vector3 p,string text)
    {
        var board=Primitive(PrimitiveType.Cube,"City Sign",p,new Vector3(2.8f,.08f,1.1f),metal);spawned.Add(board);
        var canvas=new GameObject("Sign Text");canvas.transform.position=p+Vector3.back*.06f;canvas.transform.rotation=Quaternion.Euler(90,0,0);var tm=canvas.AddComponent<TextMesh>();tm.text=text;tm.fontSize=34;tm.characterSize=.045f;tm.anchor=TextAnchor.MiddleCenter;tm.color=Color.white;spawned.Add(canvas);
    }

    void BuildPlayerStart()
    {
        GameObject player=GameObject.Find("Player");
        if(player!=null) { player.transform.position=new Vector3(0,0.05f,-14f); return; }
        player=new GameObject("Player");player.transform.position=new Vector3(0,0.05f,-14f);var cc=player.AddComponent<CharacterController>();cc.height=1.8f;cc.radius=.35f;cc.center=Vector3.up*.9f;
        var body=Primitive(PrimitiveType.Capsule,"Player Body",player.transform.position+Vector3.up*.9f,new Vector3(.72f,.9f,.72f),metal);body.transform.SetParent(player.transform,true);
        var head=Primitive(PrimitiveType.Sphere,"Player Head",player.transform.position+Vector3.up*1.85f,new Vector3(.62f,.62f,.62f),window);head.transform.SetParent(player.transform,true);
        var game=player.AddComponent<ShotgunGame>();
        var cam=new GameObject("Third Person Camera").AddComponent<Camera>();cam.tag="MainCamera";cam.fieldOfView=68;cam.nearClipPlane=.05f;cam.farClipPlane=170;game.playerCamera=cam;game.thirdPerson=true;
        player.AddComponent<ShotgunWeaponView>().playerCamera=cam;
    }

    GameObject Primitive(PrimitiveType type,string name,Vector3 pos,Vector3 scale,Material mat,Transform parent=null)
    {
        var g=GameObject.CreatePrimitive(type);g.name=name;g.transform.position=pos;g.transform.localScale=scale;g.GetComponent<Renderer>().sharedMaterial=mat;if(parent!=null)g.transform.SetParent(parent,true);return g;
    }
}
