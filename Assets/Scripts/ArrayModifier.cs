using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class ArrayModifier : MonoBehaviour
{
    public GameObject ObjectToClone;

    List<List<List<GameObject>>> _initializes = new List<List<List<GameObject>>>();

    [Range(1, 100)]
    public List<int> counts = new List<int>{ 1, 1, 1 };

    [Range(1, 100)]
    [HideInInspector]
    public int count = 1;

    public List<Vector3> offsets = new List<Vector3>
    {
        Vector3.zero,
        Vector3.zero,
        Vector3.zero
    };

    public void Extend(int value)
    {
        if (counts.Count >= 3)
            return;

        counts.Add(value);
        offsets.Add(Vector3.zero);
    }

    public void Shrink(int index)
    {
        counts.RemoveAt(index);
        offsets.RemoveAt(index);
    }

    public void ResizeInitializes()
    {
        int diffX = counts[0] - _initializes.Count;
        for (int i = 0; i < diffX; i++)
            _initializes.Add(new List<List<GameObject>>());

        for (int i = 0; i < _initializes.Count; i++)
        {
            int diffY = counts[1] - _initializes[i].Count;
            for (int j = 0; j < diffY; j++)
                _initializes[i].Add(new List<GameObject>());

            for (int j = 0; j < _initializes[i].Count; j++)
            {
                int diffZ = counts[2] - _initializes[i][j].Count;

                for (int k = 0; k < diffZ; k++)
                    _initializes[i][j].Add(Instantiate(ObjectToClone, transform.position + offsets[0] * i + offsets[1] * j + offsets[2] * k, Quaternion.identity)); // aplikuj offset

                for (int k = 0; k < _initializes[i][j].Count; k++)
                    _initializes[i][j][k].transform.parent = this.transform;

                if (diffZ < 0)
                {
                    List<GameObject> range = _initializes[i][j].GetRange(_initializes[i][j].Count + diffZ, -diffZ);
                    _initializes[i][j].RemoveRange(_initializes[i][j].Count + diffZ, -diffZ);
                    foreach (GameObject go in range)
                        DestroyImmediate(go);
                }
            }

            if (diffY < 0)
                _initializes[i].RemoveRange(_initializes[i].Count + diffY, -diffY);
        }

        if (diffX < 0)
            _initializes.RemoveRange(_initializes.Count + diffX, -diffX);
    }

    public void UpdateInitializesPosition()
    {
        if (ObjectToClone)
            for (int i = 0; i < _initializes.Count; i++)
                for (int j = 0; j < _initializes[i].Count; j++)
                    for (int k = 0; k < _initializes[i][j].Count; k++)
                        _initializes[i][j][k].transform.position = transform.position + offsets[0] * i + offsets[1] * j + offsets[2] * k;
    }

    public void Clear()
    {
        for (int i = 0; i < _initializes.Count;)
        {
            for (int j = 0; j < _initializes[i].Count;)
            {
                for (int k = 0; k < _initializes[i][j].Count;)
                {
                    DestroyImmediate(_initializes[i][j][k]);
                    _initializes[i][j].Remove(_initializes[i][j][k]);
                }
                _initializes[i][j].Clear();
                _initializes[i].Remove(_initializes[i][j]);
            }
            _initializes[i].Clear();
            _initializes.Remove(_initializes[i]);
        }
        _initializes.Clear();
    }
}
