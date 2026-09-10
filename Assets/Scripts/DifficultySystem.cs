using UnityEngine;
public class DifficultySystem:MonoBehaviour{
 public static DifficultySystem Instance{get;private set;}
 public int level=1; public float enemyHealth=1f,enemySpeed=1f,enemyDamage=1f;
 void Awake(){if(Instance!=null&&Instance!=this){Destroy(gameObject);return;}Instance=this;}
 public void SetWave(int wave){level=Mathf.Max(1,wave);enemyHealth=1f+level*.12f;enemySpeed=1f+level*.035f;enemyDamage=1f+level*.08f;}
}