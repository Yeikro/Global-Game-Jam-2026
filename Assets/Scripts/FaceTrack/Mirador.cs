using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mirador : MonoBehaviour
{
    public FaceDataReceiver faceDataReceiver;
    public float zOffset;
    public Vector3 offset;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3 (faceDataReceiver.puntos[28].x, faceDataReceiver.puntos[28].y, zOffset) + offset;
    }
}
