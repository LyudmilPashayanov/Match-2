using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class InvertedMask : Image
{
    private static readonly int StencilComp = Shader.PropertyToID("_StencilComp");

    public override Material materialForRendering
    {
        get
        {
            Material material = new Material(base.materialForRendering);
            material.SetInt(StencilComp, (int)CompareFunction.NotEqual);
            return material;
        }
    }
}
