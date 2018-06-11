using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CodingBoxMethodController : MonoBehaviour
{
    private Coroutine _updateCoroutine;

    private List<IEnumerator> _methodList = new List<IEnumerator>();

    public void StartMethodLoop()
    {
        _updateCoroutine = StartCoroutine(MethodLoopCoroutine());
    }

    public void AddMethod(IEnumerator method)
    {
        _methodList.Add(method);
    }

    private IEnumerator MethodLoopCoroutine()
    {
        foreach (var method in _methodList)
        {
            yield return StartCoroutine(method);
        }
    }
}
