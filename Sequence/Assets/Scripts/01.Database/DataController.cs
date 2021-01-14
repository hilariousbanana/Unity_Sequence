using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataController : MonoSingleton<DataController>
{
    public Data data;

    // Start is called before the first frame update
    void Start()
    {
        data = new Data();
        data.Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
