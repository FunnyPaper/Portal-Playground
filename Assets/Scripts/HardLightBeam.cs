using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalUtility.Logic;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
[ExecuteAlways]
public class HardLightBeam : ContinousMechanism
{
    Mesh mesh;
    BoxCollider bc;
    public bool IsActiveIns { get; set; } = false;

    private float _lastDistance = 0;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().sharedMesh = mesh;
        bc = GetComponent<BoxCollider>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        mesh.MarkDynamic();
        ClearMesh();
        Check();
    }

    private new void Update()
    {
        if (transform.hasChanged && IsActiveIns)
        {
            transform.hasChanged = false;
            CreateMesh();
        }

        if (_activeRegardlessSignal)
            CreateMesh();
    }

    public override void Check()
    {
        if (InputSignals.Count < 1 || _activeRegardlessSignal)
            return;

        bool currentCheck = true;

        currentCheck = LogicFunctions.EvaluateFunction(
            InputSignals.Select(signal => signal.IsActive),
            LogicType);

        if (currentCheck != _check)
        {
            if (currentCheck)
            {
                Debug.Log("TRUE! - Ex");
                IsActiveIns = true;
                CreateMesh();
            }
            else
            {
                Debug.Log("FALSE!");
                IsActiveIns = false;
                ClearMesh();
            }
            _check = currentCheck;
        }
    }

    void CreateMesh()
    {
        RaycastHit hit;
        float distance = 100;
        Debug.DrawRay(transform.position, Vector3.forward, Color.red);
        if(Physics.Raycast(transform.position, transform.forward, out hit, distance))
        {
            distance = hit.distance;
        }

        if (_lastDistance == distance && _activeRegardlessSignal && mesh.vertexCount != 0)
            return;

        ClearMesh();
        _lastDistance = distance;

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(-1, 0.01f, 0),
            new Vector3(-1, 0.01f, distance),
            new Vector3(1, 0.01f, 0),
            new Vector3(1, 0.01f, distance),
            new Vector3(-1, -0.01f, 0),
            new Vector3(-1, -0.01f, distance),
            new Vector3(1, -0.01f, 0),
            new Vector3(1, -0.01f, distance)
        };

        int[] triangles = new int[]
        {
            0, 1, 2, 1, 3, 2,
            1, 5, 3, 5, 7, 3,
            5, 4, 7, 4, 6, 7,
            4, 0, 6, 0, 2, 6,
            5, 1, 4, 1, 0, 4,
            6, 2, 7, 2, 3, 7
        };

        Vector2[] uvs = vertices.Select(v => new Vector2(v.x, v.z)).ToArray();

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        bc.size = new Vector3(2, 0, distance);
        bc.center = new Vector3(0, 0, distance / 2);
    }

    void ClearMesh()
    {
        bc.size = new Vector3();
        bc.center = new Vector3();
        mesh.Clear();
    }
}
