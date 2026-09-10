using UnityEngine;
using UnityEngine.UI;

public class ObjectiveSystem : MonoBehaviour
{
    public static ObjectiveSystem Instance{get;private set;}
    public int kills, streak, bestStreak, pickups;
    Text label;
    void Awake(){if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
    public void Build(Canvas canvas){GameObject o=new GameObject("Objective Text");o.transform.SetParent(canvas.transform,false);label=o.AddComponent<Text>();label.font=Resources.GetBuiltinResource<Font>("Arial.ttf");label.fontSize=15;label.color=Color.white;RectTransform r=label.rectTransform;r.anchorMin=new Vector2(.5f,.88f);r.anchorMax=new Vector2(.5f,.88f);r.sizeDelta=new Vector2(420,45);UpdateText();}
    public void Kill(){kills++;streak++;if(streak>bestStreak)bestStreak=streak;UpdateText();}
    public void Miss(){streak=0;UpdateText();}
    public void Pickup(){pickups++;UpdateText();}
    void UpdateText(){if(label)label.text="OBJECTIVE  KILLS "+kills+"   STREAK "+streak+"   BEST "+bestStreak;}
}