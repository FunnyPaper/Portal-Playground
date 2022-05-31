using System.Linq;
using UnityEngine;
using GlobalUtility.InheritBehaviour;
using GlobalUtility.Logic;

public class RotateMechanism : Mechanism
{
    [SerializeField] Vector3 _angle = Vector3.zero;

    protected override void Start()
    {
        base.Start();
    }

    public override void Check()
    {
        if (InputSignals.Count < 1)
            return;

        bool currentCheck = true;

        currentCheck = LogicFunctions.EvaluateFunction(
            InputSignals.Select(signal => signal.IsActive),
            LogicType);

        //Jeśli jednak przejdzie test to jednorazowo zmień stan
        if (currentCheck != _check)
        {
            if (currentCheck)
            {
                transform.Rotate(_angle);
            }
            _check = currentCheck;
        }
    }
}
