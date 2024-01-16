using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(TilemapRenderer))]
public class TilemapHider : MonoBehaviour
{
	public Renderer rend;
	// Start is called before the first frame update
	void Start()
    {
        rend.enabled = false;
	}

    // Update is called once per frame
    void Update()
    {
        
    }
}
