using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomChunk : MonoBehaviour, ISpawnChunk
{

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public int Seed {
        get {return _seed;}
        set {_seed = value;}
    }
    int _seed;

    public void init() {
        //TODO: make random seed
        int thisSeed = 42;
        initWithSeed(thisSeed);
    }


    public void initWithSeed(int seed) {
        Seed = seed;
    }
}
