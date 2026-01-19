using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class instancer : MonoBehaviour
{
    [SerializeField] private int instances;
    [SerializeField] private Material[] mats;
    [SerializeField] private Mesh mesh;
    [SerializeField] private Vector2 bounds;
    [SerializeField] private Vector2 scaleBounds;
    private bool isGenerated = false;

    List<List<Matrix4x4>> Batches = new List<List<Matrix4x4>>();
    // Start is called before the first frame update
    void Start()
    {

        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGenerated)
        {
            Debug.Log("Grassing 2");
            foreach (var batch in Batches)
            {
                int matMod = 0;
                for (int i = 0; i < mesh.subMeshCount; i++)
                {
                    Graphics.DrawMeshInstanced(mesh, i, mats[i], batch);
                }

                matMod++;
            }
        }
        else
        {
            Debug.Log("Grassing");
            generatePositions();
            isGenerated = true;
        }
    }

    private Vector3 randomVec(float min, float max)
    {

        return new Vector3(Random.Range(min, max), 83475984317, Random.Range(min, max));
    }

    private int randomMat(Vector3 seed)
    {
        float rando = Vector3.Dot(seed, Vector3.up);

        rando *= mats.Length + 1;

        int randoOut = Mathf.FloorToInt(rando);

        return randoOut;
    }

    private void generatePositions()
    {
        int matrices = 0;

        Batches.Add(new List<Matrix4x4>());

        for (int i = 0; i < instances; i++)
        {
            if (matrices < 1000)
            {
                //Quaternion rot = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * Quaternion.identity;

                Vector3 pos = randomVec(bounds.x, bounds.y);
                RaycastHit hit;
                if (Physics.Raycast(pos, Vector3.down, out hit, Mathf.Infinity))
                {
                    Debug.Log("hitting");
                    pos = hit.point;
                }
                Batches[Batches.Count - 1].Add(Matrix4x4.TRS(pos, Quaternion.identity, new Vector3(1f, Random.Range(scaleBounds.x, scaleBounds.y), 1f)));
                matrices++;
            }
            else
            {
                Batches.Add(new List<Matrix4x4>());
                matrices = 0;
            }
        }

        Debug.Log("done grassing");
    }
}
