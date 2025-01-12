using System.Collections;
using UnityEngine;

public class CoroutineRunner : MonoBehaviour
{
    private static CoroutineRunner _instance;

    public static CoroutineRunner Instance
    {
        get
        {
            if (_instance != null) return _instance;
            _instance = new GameObject("CoroutineRunner").AddComponent<CoroutineRunner>();

            return _instance;
        }
    }

    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void StopConcreteCoroutine(Coroutine coroutine)
    {
        StopCoroutine(coroutine);
    }
}