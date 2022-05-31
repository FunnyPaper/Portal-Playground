using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using GlobalUtility.InheritBehaviour;
using GlobalUtility.Logic;

[RequireComponent(typeof(LineRenderer))]
public class BeamEmitter : ContinousMechanism
{
    LineRenderer lr;
    
    [SerializeField] Transform StartingPoint = null;
    [SerializeField] GameObject CollidingPoint = null;
    [SerializeField] float distance = 2000;

    public bool IsActiveIns { get; set; } = false;

    List<Vector3> _hitPoints = new List<Vector3>();

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        lr = GetComponent<LineRenderer>();
        CollidingPoint.transform.position = StartingPoint.position;
        CollidingPoint.layer = LayerMask.NameToLayer("Ignore Raycast");
    }

    // Update is called once per frame
    private new void Update()
    {
        if (IsActiveIns || _activeRegardlessSignal)
        {
            GetBouncesPoints();
            if (_hitPoints.Count >= 2)
            {
                lr.positionCount = _hitPoints.Count;
                lr.SetPositions(_hitPoints.ToArray());
            }
            else
            {
                lr.positionCount = 2;
                lr.SetPosition(0, StartingPoint.position);
                lr.SetPosition(1, StartingPoint.up * distance);
            }
        }
    }

    void GetBouncesPoints()
    {
        _hitPoints.Clear();
        _hitPoints.Add(StartingPoint.position);

        RaycastHit hit;
        Vector3 startPoint = StartingPoint.position;
        Vector3 direction = StartingPoint.up;

        while(Physics.Raycast(startPoint, direction, out hit))
        {
            Debug.DrawRay(startPoint, direction, Color.yellow);
            if (!hit.transform.CompareTag("Redirection_Cube"))
                break;

            if (_hitPoints.Contains(hit.point) || _hitPoints.Contains(hit.transform.position))
                break;

            _hitPoints.Add(hit.point);
            _hitPoints.Add(hit.transform.position);
            startPoint = hit.transform.position;
            direction = hit.transform.forward;
        }

        if(Physics.Raycast(startPoint, direction, out hit))
        {
            if (hit.transform.GetComponent<LaserSocket>())
            {
                _hitPoints.Add(hit.transform.position);
            }
            else
                _hitPoints.Add(hit.point);
        }
        else
            _hitPoints.Add(direction * distance);

        Debug.DrawRay(startPoint, direction, Color.yellow);
        CollidingPoint.transform.position = _hitPoints[_hitPoints.Count - 1];
    }

    public override void Check()
    {
        if (InputSignals.Count < 1)
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
            }
            else
            {
                IsActiveIns = false;
                _hitPoints.Clear();
                lr.positionCount = 0;
                CollidingPoint.transform.position = StartingPoint.position;
            }
            _check = currentCheck;
        }
    }
}
