using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    public RawImage nuevoraw;

    // Start is called before the first frame update
    void Start()
    {
        UDPImageReceiver.instance2.rawImage = nuevoraw;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
