using UnityEngine;
public class ComboSystem:MonoBehaviour{
 public static ComboSystem Instance{get;private set;}
 public int combo;float lastKill;public float window=4f;
 void Awake(){if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
 public int RegisterKill(){if(Time.time-lastKill>window)combo=0;combo++;lastKill=Time.time;return combo;}
 public void Reset(){combo=0;lastKill=0;}
}