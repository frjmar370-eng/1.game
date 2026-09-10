using UnityEngine;

public class AndroidGamePolish : MonoBehaviour
{
    void Awake(){
        Application.targetFrameRate=60;
        QualitySettings.vSyncCount=0;
        QualitySettings.antiAliasing=2;
        QualitySettings.shadowDistance=45f;
        QualitySettings.pixelLightCount=3;
        Screen.sleepTimeout=SleepTimeout.NeverSleep;
    }

    void Start(){
        Camera cam=Camera.main;
        if(cam){
            cam.fieldOfView=68f;
            cam.nearClipPlane=.03f;
            cam.farClipPlane=90f;
        }
    }
}
