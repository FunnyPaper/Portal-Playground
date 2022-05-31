using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using GlobalUtility.Logic;
using GlobalUtility.InheritBehaviour;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(BoxCollider))]
[ExecuteAlways]
public class ExecursionFunnelField : ContinousMechanism
{
    Mesh mesh;
    BoxCollider bc;

    public bool IsActiveIns { get; set; } = false;

    [SerializeField] float force = 1;
    [SerializeField] int _fieldFaces = 16;
    float _lastDistance = 0;

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

    private void OnTriggerEnter(Collider other)
    {
        if (other.attachedRigidbody && other.GetComponent<Entity>())
        {
            other.attachedRigidbody.useGravity = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.attachedRigidbody && other.GetComponent<Entity>())
        {
            other.attachedRigidbody.useGravity = true;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if(other.attachedRigidbody && other.GetComponent<Entity>())
        {
            other.attachedRigidbody.velocity = transform.up;
            other.attachedRigidbody.useGravity = false;
            other.attachedRigidbody.AddForce(transform.up * force);
        }
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
                IsActiveIns = true;
                CreateMesh();
            }
            else
            {
                IsActiveIns = false;
                ClearMesh();
            }
            _check = currentCheck;
        }
    }

    public void CreateMesh()
    {
        RaycastHit hit;
        float distance = 100;
        Debug.DrawRay(transform.position, Vector3.up, Color.red);
        if (Physics.Raycast(transform.position, transform.up, out hit, distance))
        {
            if(hit.transform.CompareTag("Block"))
                distance = hit.distance / transform.lossyScale.y;
        }

        if (_lastDistance == distance && _activeRegardlessSignal && mesh.vertexCount != 0)
            return;

        ClearMesh();
        _lastDistance = distance;

        List<Vector3> vertices = new List<Vector3>();
        for(int i = 0; i < _fieldFaces; i++)
        {
            float angle = 2 * Mathf.PI / _fieldFaces * i;
            vertices.Add(new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)));
        }

        for (int i = 0; i < _fieldFaces; i++)
        {
            float angle = 2 * Mathf.PI / _fieldFaces * i;
            vertices.Add(new Vector3(Mathf.Cos(angle), distance, Mathf.Sin(angle)));
        }

        List<int> triangles = new List<int>();

        for(int j = 0; j < (vertices.Count / 2) - 1; j++)
        {
            triangles.Add(j);
            triangles.Add(j + 1);
            triangles.Add(j + _fieldFaces);

            triangles.Add(j + 1);
            triangles.Add(j + 1 + _fieldFaces);
            triangles.Add(j + _fieldFaces);
        }

        triangles.Add((vertices.Count - 1) / 2);
        triangles.Add(0);
        triangles.Add(vertices.Count - 1);

        triangles.Add(0);
        triangles.Add((vertices.Count) / 2);
        triangles.Add(vertices.Count - 1);

        // inne
        vertices.Add(new Vector3());
        vertices.Add(new Vector3(0, distance, 0));

        // dół
        for (int i = ((vertices.Count - 2) / 2) - 1; i > 0; i--)
        {
            triangles.Add(i);
            triangles.Add(i - 1);
            triangles.Add(vertices.Count - 1 - 1);
        }

        triangles.Add(0);
        triangles.Add(((vertices.Count - 2) / 2) - 1);
        triangles.Add(vertices.Count - 1 - 1);

        // góra
        for (int i = (vertices.Count - 2) / 2; i < (vertices.Count - 1 - 2); i++)
        {
            triangles.Add(i);
            triangles.Add(i + 1);
            triangles.Add(vertices.Count - 1);
        }

        triangles.Add(vertices.Count - 1 - 2);
        triangles.Add((vertices.Count - 2) / 2);
        triangles.Add(vertices.Count - 1);

        Vector2[] uvs = vertices.Select(v => new Vector2(v.x, v.y)).ToArray();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uvs;

        mesh.RecalculateNormals();

        bc.size = new Vector3(2, distance, 2);
        bc.center = new Vector3(0, distance / 2, 0);
    }

    public void ClearMesh()
    {
        bc.size = new Vector3();
        bc.center = new Vector3();
        mesh.Clear();
    }
}
