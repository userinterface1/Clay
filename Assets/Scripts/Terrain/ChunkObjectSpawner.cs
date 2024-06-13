using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static UnityEngine.Mesh;

public class ChunkObjectSpawner : MonoBehaviour 
{
    [SerializeField] private GameObject treePrefab;
    [SerializeField] private Mesh chunkMesh;

    public void Spawn()
    {
        chunkMesh = GetComponent<MeshFilter>().mesh;

        MeshDataArray meshDataArray = Mesh.AcquireReadOnlyMeshData(chunkMesh);
        MeshData meshData = meshDataArray[0];
        NativeArray<Vertex> vertices = meshData.GetVertexData<Vertex>();
        NativeList<int> checkVertexIndices = new NativeList<int>(Allocator.TempJob);
        Job job = new Job() { vertices = vertices, checkVertexIndices = checkVertexIndices };
        job.Schedule().Complete();

        for (int i = 0, length = checkVertexIndices.Length; i < length; ++i)
        {
            int rand = UnityEngine.Random.Range(0, 100);
            if (rand == 0)
            {
                rand = UnityEngine.Random.Range(0, 360);

                Vertex vertex = vertices[checkVertexIndices[i]];
                float3 spawnPoint = new float3(transform.position);
                spawnPoint += vertex.position + new float3(0, -0.5f, 0);
                Instantiate(treePrefab, spawnPoint, Quaternion.Euler(0, rand, 0), transform);
            }
        }

        checkVertexIndices.Dispose();
        meshDataArray.Dispose();
    }

    private struct Job : IJob
    {
        [ReadOnly]
        public NativeArray<Vertex> vertices;
        [NativeDisableContainerSafetyRestriction]
        public NativeList<int> checkVertexIndices;

        public void Execute()
        {
            for (int i = 0, length = vertices.Length; i < length; ++i)
            {
                if (vertices[i].position.y > 5.0f)
                {
                    checkVertexIndices.Add(i);
                }
            }
        }
    }
}
