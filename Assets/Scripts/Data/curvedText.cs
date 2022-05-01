using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
 
[ExecuteInEditMode]
public class curvedText : MonoBehaviour {
    public string RoundText = "";
    public float Radius = 10;
    public GameObject TextMeshPrefab;
    public bool join,destroy;
    List<GameObject> prefabs = new List<GameObject>();
    void Update(){
        if(join){
            Vector3 center = transform.position;
            float ang = 0;
            for (int i = 0; i < RoundText.Length; i++){
                Vector3 pos = RandomCircle(center, Radius,ang);
                Quaternion rot = Quaternion.FromToRotation(Vector3.forward, center-pos);
                     
 
                prefabs.Add(Instantiate(TextMeshPrefab, pos, rot));
                char c = RoundText[i];
                prefabs[i].transform.parent = transform;
                prefabs[i].GetComponentInChildren<TextMeshPro>().text = c.ToString();
                ang -= 360 / RoundText.Length-1;
            }
            prefabs[0].transform.rotation = Quaternion.Euler(0,prefabs[0].transform.rotation.y,prefabs[0].transform.rotation.z);
            join = false;
        }
        if(destroy){
            for(int i = 0;i<prefabs.Count;i++){
                DestroyImmediate(prefabs[i]);
            }
            prefabs = new List<GameObject>();
            destroy = false;
        }
    }
 
    Vector3 RandomCircle ( Vector3 center ,float radius,float ang){
        Vector3 pos;
        pos.x = center.x + radius * Mathf.Sin(ang * Mathf.Deg2Rad);
        pos.y = center.y;
        pos.z = center.z + radius * Mathf.Cos(ang * Mathf.Deg2Rad);
        return pos;
    }
   
}