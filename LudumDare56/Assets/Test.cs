using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
	{
		Stats a = new Stats();
		a.name = new List<string>() { "Bal", "loon" };
		Stats b = new Stats();
		b.name = new List<string>() { "Cat", "er", "pil", "lar" };

        for(int i = 0; i < 10; i++)
        {
            Stats c = Stats.Merge(a, b);
            print(a.Name + " + " + b.Name + " => " + c.Name);
        }
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
