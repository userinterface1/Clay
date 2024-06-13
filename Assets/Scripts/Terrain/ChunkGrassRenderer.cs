using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class ChunkGrassRenderer
{
    private Material _grassMaterial;

    public ChunkGrassRenderer(Material grassMaterial)
    {
        _grassMaterial = grassMaterial;
    }

    public void GrassRender()
    {
        _grassMaterial.SetVector("viewPosition", Camera.main.transform.position);
    }
}
