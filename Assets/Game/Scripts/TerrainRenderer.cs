using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TerrainRenderer : MonoBehaviour
{
    [SerializeField]
    private Material meshMaterial;

    [SerializeField]
    private float isolevel;
    [SerializeField]
    private float radius;

    [SerializeField]
    private Vector3Int size = new Vector3Int(32, 32, 32);

    [SerializeField]
    private bool enableEditing = true;

    private IsosurfaceProcessor isosurface;
    private float[] voxels;

    [SerializeField]
    private Text test;

    private bool isDirty = true;

    private Vector3 hitPosition;
    private MeshInstance meshInstance;

    [SerializeField]
    private float modificationPower = 10;

    private void Start()
    {
        isosurface = new IsosurfaceProcessor();

        meshInstance = new MeshInstance("Mesh");
        meshInstance.Transform.position = new Vector3(-size.x / 2f, -size.y / 2f, -size.y / 2f);
        meshInstance.Transform.tag = "MC_Object";
        meshInstance.MeshRenderer.sharedMaterial = meshMaterial;

        // Generate our voxel data
        voxels = new float[size.x * size.y * size.z];

        for (int z = 0; z < size.z; z++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    float sx = x - size.x / 2;
                    float sy = y - size.y / 2;
                    float sz = z - size.z / 2;

                    int n = x + y * size.x + z * size.x * size.y;
                    float density = sx * sx + sy * sy + sz * sz - radius * radius;

                    voxels[n] = density;
                }
            }
        }
    }

    private void Generate()
    {
        isosurface.Isolevel = isolevel;
        IsosurfaceMesh isosurfaceMesh = isosurface.Generate(voxels, size.x, size.y, size.z);
        meshInstance.FromIsosurface(isosurfaceMesh);
    }

    private void Update()
    {
        Generate();
        if (meshInstance == null) return;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        test.text = "not hit";

        Vector3 point;
        if (meshInstance.Raycast(ray, out point))
        {
            test.text = "hit";
            if (enableEditing && Input.GetMouseButton(0))
            {
                AddModification(hitPosition, !Input.GetKey(KeyCode.LeftControl));
            }

            hitPosition = point;
        }
    }

    private void AddModification(Vector3 position, bool destroy = true)
    {
        for (int z = 0; z < size.z; z++)
        {
            for (int y = 0; y < size.y; y++)
            {
                for (int x = 0; x < size.x; x++)
                {
                    float sx = x - size.x / 2;
                    float sy = y - size.y / 2;
                    float sz = z - size.z / 2;

                    int n = x + y * size.x + z * size.x * size.y;
                    float modificationDensity = modificationPower / ((sx - position.x) * (sx - position.x) +
                                                                     (sy - position.y) * (sy - position.y) +
                                                                     (sz - position.z) * (sz - position.z));

                    modificationDensity *= destroy ? 1 : -1;
                    voxels[n] += modificationDensity;
                }
            }
        }

        isDirty = true;
    }

    private void LateUpdate()
    {
        if (!isDirty) return;

        isDirty = false;
        Generate();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = new Color(1, 0, 0);
        Gizmos.DrawSphere(hitPosition, 0.1f);

        Gizmos.DrawLine(Camera.main.ScreenToWorldPoint(Input.mousePosition), hitPosition);
    }
}
