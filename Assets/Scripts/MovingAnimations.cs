using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingAnimations : MonoBehaviour {

    Vector3 lastPosition;

    Animator anim;

    public float smooth;
    Vector3 refMovement;
    Vector3 movement;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        movement = Vector3.SmoothDamp(movement, transform.position - lastPosition,ref refMovement,smooth);
        float speed = movement.magnitude / Time.deltaTime;
        anim.SetFloat("Speed", speed);

        transform.LookAt(transform.position + movement);
        lastPosition = transform.position;
    }
}
