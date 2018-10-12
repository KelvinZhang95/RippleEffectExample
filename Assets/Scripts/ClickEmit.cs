using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickEmit : MonoBehaviour {
    public RippleEffect ripple;
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("press mouse .");
            ripple.Emit(new Vector2(0.0f,0.0f));
        }
	}
}
