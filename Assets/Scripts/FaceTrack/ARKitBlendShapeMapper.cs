using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARKitBlendShapeMapper : MonoBehaviour
{
    public ARKitFaceSolver arkit;
    public SkinnedMeshRenderer mesh;

    [Header("Indices del modelo (Blender)")]
    public int jawOpen = 41;
    //public int mouthSmile_L = 21;
    //public int mouthSmile_R = 22;
    public int eyeBlink_L = 46;
    public int eyeBlink_R = 45;
    //public int browInnerUp = 44;

    void Update()
    {
        if (arkit == null || mesh == null) return;

        Apply(jawOpen, arkit.jawOpen);
        //Apply(mouthSmile_L, arkit.mouthSmile_L);
        //Apply(mouthSmile_R, arkit.mouthSmile_R);
        Apply(eyeBlink_L, arkit.eyeBlink_L);
        Apply(eyeBlink_R, arkit.eyeBlink_R);
        //Apply(browInnerUp, arkit.browInnerUp);
    }

    void Apply(int index, float value)
    {
        mesh.SetBlendShapeWeight(index, value * 100f);
    }
}
