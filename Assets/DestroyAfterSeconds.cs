using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DestroyAfterSeconds : MonoBehaviour
{
    public float Fixedseconds = 3;

    public bool RandomRange = false;
    public float MinSeconds = 1, MaxSeconds = 3;

    public bool Shrink = false;

    private float randomNumber; 

    private void Start()
    {
        float randomNumber = Random.Range(MinSeconds, MaxSeconds);

        if (RandomRange) Destroy(gameObject,randomNumber );

        else Destroy(gameObject, Fixedseconds);

    }

    private void Update()
    {
        //causes the item to shrink out of existence if true DOES NOT WORK ON RECT TRANFORM  (this means UI stuff)
        if (Shrink)
        {
            if (RandomRange) gameObject.transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero, randomNumber * Time.deltaTime);

            else gameObject.transform.localScale = Vector3.MoveTowards(transform.localScale, Vector3.zero,  Fixedseconds *Time.deltaTime);
        }
       
    }
}
