using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private float cullingDistance;

    [SerializeField] private float scale;
    [SerializeField] private float octaves;
    [SerializeField] private float persistance;
    [SerializeField] private float lacunarity;

    [SerializeField] private int chunkWidthCount;
    [SerializeField] private int chunkHeightCount;
    [SerializeField] private GameObject chunkPrefab;
    [SerializeField] private List<Chunk> chunks = new List<Chunk>();

    private static Dictionary<int3, Chunk> grid2Chunk = new Dictionary<int3, Chunk>(); 

    public static float[,,] worldSurfaceLevel;

    void Start()
    {
        float[,] perlinMap = GenerateMap(chunkWidthCount * Chunk.CHUNK_WIDTH + 1, chunkWidthCount * Chunk.CHUNK_WIDTH + 1, scale, octaves, persistance, lacunarity);
        float[,] falloffMap = GenerateFalloffMap(chunkWidthCount * Chunk.CHUNK_WIDTH + 1);

        worldSurfaceLevel = new float[chunkWidthCount * Chunk.CHUNK_WIDTH + 1, chunkHeightCount * Chunk.CHUNK_VERTICES_HEIGHT + 1, chunkWidthCount * Chunk.CHUNK_WIDTH + 1];

        for (int x = 0, width = worldSurfaceLevel.GetLength(0); x < width; ++x)
        {
            for (int z = 0; z < width; ++z)
            {
                for (int y = 0, height = worldSurfaceLevel.GetLength(1); y < height; ++y)
                {
                    float thisHeight = (perlinMap[x, z] * (1 - falloffMap[x, z]) * height) - 1f;
                    worldSurfaceLevel[x, y, z] = (float)y - thisHeight;
                }
            }
        }

        for (int x = 0; x < chunkWidthCount; ++x)
        {
            for(int z = 0; z < chunkWidthCount; ++z)
            {
                for (int y = 0; y < chunkHeightCount; ++y)
                {
                    GameObject chunkObject = Instantiate(chunkPrefab, new float3(x * Chunk.CHUNK_WIDTH, y * Chunk.CHUNK_HEIGHT, z * Chunk.CHUNK_WIDTH), Quaternion.identity, transform);
                    Chunk chunk = chunkObject.GetComponent<Chunk>();
                    grid2Chunk.Add(new int3(x, y, z), chunk);
                    chunks.Add(chunk);
                }
            }
        }
    }

    private void Update()
    {
        float2 playerPositionXZ = new float2(Camera.main.transform.position.x, Camera.main.transform.position.z);

        for (int i = 0, length = chunks.Count; i < length; ++i)
        {
            Chunk chunk = chunks[i];
            float2 chunkCenterXZ = new float2(chunk.transform.position.x + Chunk.CHUNK_WIDTH / 2, chunk.transform.position.z + Chunk.CHUNK_WIDTH / 2);

            if (math.distancesq(chunkCenterXZ, playerPositionXZ) < cullingDistance * cullingDistance)
            {
                if (chunk.gameObject.activeSelf) continue;
                chunk.gameObject.SetActive(true);
            }
            else
            {
                if (!chunk.gameObject.activeSelf) continue;
                chunk.gameObject.SetActive(false);
            }
        }
    }

    public static void DeformationChunk(float3 point, float deformation)
    {
        float3 playerPosition = Camera.main.transform.position;
        int3 playerGridPosition = new int3((int)playerPosition.x, (int)playerPosition.y, (int)playerPosition.z);
        playerGridPosition.x = playerGridPosition.x / Chunk.CHUNK_WIDTH;
        playerGridPosition.y = playerGridPosition.y / Chunk.CHUNK_HEIGHT;
        playerGridPosition.z = playerGridPosition.z / Chunk.CHUNK_WIDTH;

        for (int x = -1; x < 2; ++x)
        {
            for (int y = -1; y < 2; ++y)
            {
                for (int z = -1; z < 2; ++z)
                {
                    if (grid2Chunk.TryGetValue(playerGridPosition + new int3(x, y, z), out Chunk chunk))
                    {
                        chunk.DeformationChunk(point, deformation);
                    }
                }
            }
        }
    }

    public float[,] GenerateFalloffMap(int size)
    {
        float[,] map = new float[size, size];

        for (int i = 0; i < size; ++i)
        {
            for (int j = 0; j < size; ++j)
            {
                float x = i / (float)size * 2 - 1;
                float z = j / (float)size * 2 - 1;
                float value = Mathf.Max(Mathf.Abs(x), Mathf.Abs(z));
                map[i, j] = value;//Mathf.Pow(value, 3) / (Mathf.Pow(value, 3) + Mathf.Pow(2.2f + 2.2f * value, 3));
            }
        }

        return map;
    }

    public float[,] GenerateMap(int width, int height, float scale, float octaves, float persistance, float lacunarity)
    {
        float[,] noiseMap = new float[width, height];
        scale = Mathf.Max(0.0001f, scale);
        float maxNoiseHeight = float.MinValue; //�ִ� ���� ��� ���� ����
        float minNoiseHeight = float.MaxValue; //�ּ� ���� ��� ���� ����
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                float amplitude = 1; //����. �������� ���� ���õ� ��.
                float frequency = 1; //���ļ�. �������� ���ݰ� ���õ� ��. ���ļ��� Ŀ������ ����� ��������
                float noiseHeight = 0;

                for (int i = 0; i < octaves; i++) //��Ÿ�갡 �����Ҽ��� ���� ���ļ��� ���� ������ ����� ��ø��.
                {
                    float xCoord = x / scale * frequency;
                    float yCoord = y / scale * frequency;
                    float perlinValue = Mathf.PerlinNoise(xCoord, yCoord) * 2 - 1; //0~1 ������ ���� ��ȯ�ϴ� �Լ�. 2�� ���ϰ� 1�� ���� -1~1 ������ ������ ��ȯ
                    noiseHeight += perlinValue * amplitude;
                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                else if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
                noiseMap[x, y] = noiseHeight;
            }
        }
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]); //lerp�� ���Լ��� �ּڰ��� �ִ��� ���հ��� 3��° ���ڷ� ������ 0~1������ ���� ��ȯ
            }
        }
        return noiseMap;
    }
}
