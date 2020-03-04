using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppData : MonoBehaviour
{
    // Start is called before the first frame update
    public List<string> CsbImagePaths = new List<string>();
    void Start()
    {
        
    }

    public void AddCsbImagePaths(List<string> paths)
    {
        CsbImagePaths.AddRange(paths);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
