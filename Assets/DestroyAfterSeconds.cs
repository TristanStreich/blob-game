using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float Fixedseconds = 3;

    public bool RandomRange = false;
    public float MinSeconds = 1, MaxSeconds = 3;
  
    private void Start()
    {
        if (RandomRange) Destroy(gameObject, Random.Range(MinSeconds, MaxSeconds));

        else Destroy(gameObject, Fixedseconds);
        
    }
}
