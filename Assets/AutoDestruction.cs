using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestruction : MonoBehaviour
{
    public float life_time = 5.0f;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ScheduleAutoDestruction());
    }

    private IEnumerator ScheduleAutoDestruction()
    {
        while(life_time > 0)
        {
            life_time--;
            yield return new WaitForSeconds(1);
        }
        Destroy(this.gameObject);
    }
}
