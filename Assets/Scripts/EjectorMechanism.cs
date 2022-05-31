using System.Linq;
using UnityEngine;
using GlobalUtility.Logic;
using GlobalUtility.InheritBehaviour;

public class EjectorMechanism : Mechanism
{
    GameObject SpawnPoint;
    
    [Header("Spawn Data")]
    [SerializeField] GameObject SpawnObject = null;
    [SerializeField] int MaxSpawns = 1;

    protected override void Start()
    {
        base.Start();
        SpawnPoint = transform?.GetChild(2).gameObject;
        Check();
    }

    public void Spawn()
    {
        if (MaxSpawns-- > 0)
        {
            Instantiate(SpawnObject, SpawnPoint.transform.position, Quaternion.Euler(0, 0, 0));
            _audioSource.Play();
        }
        else Debug.Log("Ejector został wykorzystany");
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
                Spawn();
            }
            _check = currentCheck;
        }
    }
}
