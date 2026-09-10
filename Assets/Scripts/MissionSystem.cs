using UnityEngine;
using UnityEngine.UI;

public class MissionSystem : MonoBehaviour
{
    public static MissionSystem Instance {get;private set;}
    Text missionText; int kills; int target=15; bool completed;
    void Awake(){if(Instance&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
    public void Build(Canvas canvas){
        if(!canvas)return;
        GameObject go=new GameObject("Mission");go.transform.SetParent(canvas.transform,false);
        RectTransform r=go.AddComponent<RectTransform>();r.anchorMin=new Vector2(.5f,1);r.anchorMax=new Vector2(.5f,1);r.anchoredPosition=new Vector2(0,-70);r.sizeDelta=new Vector2(650,55);
        missionText=go.AddComponent<Text>();missionText.font=Resources.GetBuiltinResource<Font>("Arial.ttf");missionText.fontSize=28;missionText.alignment=TextAnchor.MiddleCenter;missionText.color=Color.white;Refresh();
    }
    public void Kill(){if(completed)return;kills++;if(kills>=target)completed=true;Refresh();}
    void Refresh(){if(missionText)missionText.text=completed?"المهمة مكتملة ✓":"المهمة: اقضِ على " + target + " أعداء   ("+kills+"/"+target+")";}
}
