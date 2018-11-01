using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour {

    Camera cam;
    public LayerMask groundLayer;

    public float speed;

    Vector3 target;

	// Use this for initialization
	void Start () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {

        //On mouse left click
		if(Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray camRay = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(camRay,out hit,50, groundLayer))
            {
                target = hit.point;
            }
        }

        Vector3 toTarget = target - transform.position;
        toTarget.y = 0;
        if (toTarget.magnitude > 0.05f)
        {
            Vector3 movement = toTarget.normalized;
            movement *= speed * Time.deltaTime;
            transform.Translate(movement,Space.World);
        }
	}

    void OnDrawGizmos()
    {
        Gizmos.DrawSphere(target, 0.3f);
    }
}
