using System.Linq;
using UnityEngine;
using GlobalUtility.Logic;
using GlobalUtility.InheritBehaviour;

public class FlipActiveMechanism : Mechanism
{
    [SerializeField] bool _isActiveOnStart = true;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        if (!_isActiveOnStart)
            Flip();
        Check();
    }

    void Flip()
    {
        transform.gameObject.SetActive(!transform.gameObject.activeSelf);
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
            Flip();
            _check = currentCheck;
        }
    }
}
