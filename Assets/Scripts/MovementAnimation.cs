using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAnimation : MonoBehaviour {

    Vector3 lastPosition;

    Animator anim;

    public float smooth;
    Vector3 refMovement;
    Vector3 movement;
    float refAngle;
    float currentAngle;

	// Use this for initialization
	void Awake () {
        anim = GetComponent<Animator>();

        //Make the character look toward the camera by default
        currentAngle = -90;
	}
	
	// Update is called once per frame
	void Update () {
        movement = Vector3.SmoothDamp(movement, transform.position - lastPosition,ref refMovement,0.1f);

        float targetAngle;
        if ((transform.position - lastPosition).magnitude > Mathf.Epsilon)
            targetAngle = Vector3.SignedAngle(Vector3.right, movement,Vector3.down);
        else
            targetAngle = currentAngle;

        currentAngle = Mathf.SmoothDampAngle(currentAngle, targetAngle, ref refAngle, smooth);


        float speed = movement.magnitude / Time.deltaTime;
        anim.SetFloat("Speed", speed);

        transform.LookAt(transform.position + new Vector3(Mathf.Cos(currentAngle * Mathf.Deg2Rad),0, Mathf.Sin(currentAngle * Mathf.Deg2Rad)));
        lastPosition = transform.position;
    }
}
