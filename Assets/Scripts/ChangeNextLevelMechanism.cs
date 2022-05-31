using UnityEngine;
using System.Linq;
using GlobalUtility.InheritBehaviour;
using GlobalUtility.Logic;

[RequireComponent(typeof(Exit))]
public class ChangeNextLevelMechanism : Mechanism
{
    [SerializeField] string NewDestination = string.Empty;

    Exit ExitDoor;

    string OldDestination;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ExitDoor = GetComponent<Exit>();
        OldDestination = ExitDoor.nextLevelName;
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
                ExitDoor.nextLevelName = NewDestination;
            }
            else
            {
                ExitDoor.nextLevelName = OldDestination;
            }
            _check = currentCheck;
        }
    }
}
