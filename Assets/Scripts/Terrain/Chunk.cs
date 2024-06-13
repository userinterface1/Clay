using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int CHUNK_WIDTH = 32;
    public const int CHUNK_HEIGHT = 16;
    public const int CHUNK_VERTICES_WIDTH = CHUNK_WIDTH + 1;
    public const int CHUNK_VERTICES_HEIGHT = CHUNK_HEIGHT + 1;

    private ChunkMeshGenerator chunkMeshGenerator;
    private ChunkGrassRenderer grassRenderer;
    private ChunkObjectSpawner chunkObjectSpawner;

    void Start()
    {
        chunkMeshGenerator = new ChunkMeshGenerator(GetComponent<MeshFilter>(), GetComponent<MeshCollider>(), new int3(transform.position));
        chunkMeshGenerator.GenerateChunkMesh();

        grassRenderer = new ChunkGrassRenderer(GetComponent<Renderer>().materials[1]);

        chunkObjectSpawner = GetComponent<ChunkObjectSpawner>();

        chunkObjectSpawner.Spawn();
    }

    private void Update()
    {
        grassRenderer.GrassRender();
    }

    public void DeformationChunk(float3 point, float deformation)
    {
        chunkMeshGenerator.DeformationMesh(point, deformation);
    }
}
