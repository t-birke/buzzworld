using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class curvedText2 : Text {
    [SerializeField] private float diameter = 10;
 
protected override void OnPopulateMesh(VertexHelper vh)
{
    base.OnPopulateMesh(vh);
    Debug.Log("onPopulateMesh Override called");
    for (int i = 0; i < vh.currentVertCount; i++)
    {
        UIVertex vert = UIVertex.simpleVert;
        vh.PopulateUIVertex(ref vert, i);
        Vector3 position = vert.position;
 
        //manipulate position
        float ratio = (float)position.x / preferredWidth;
        float mappedRatio = ratio * 2 * Mathf.PI;
        float cos = Mathf.Cos(mappedRatio);
        float sin = Mathf.Sin(mappedRatio);
 
        position.x = -cos * diameter;
        position.z = sin * diameter;
 
        vert.position = position;
        vh.SetUIVertex(vert, i);
    }
}
}