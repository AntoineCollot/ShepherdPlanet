using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyboardControl : MonoBehaviour {

    [Tooltip("Movement speed")]
    public float speed = 2;
	
	// Update is called once per frame
	void Update () {
        //Get the inputs from the keyboard
        Vector3 inputs = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        //Move the transform by the input amount
        transform.Translate(inputs * speed * Time.deltaTime,Space.World);

        //Make sure the transform stays inside the pen
        transform.position = Pen.Instance.ClampPositionToPen(transform.position);
    }
}
