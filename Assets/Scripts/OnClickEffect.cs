using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickEffect : MonoBehaviour {

    [SerializeField]
    GameObject clickEffectPrefab;

    public LayerMask groundLayer;

    Camera cam;

	// Use this for initialization
	void Awake () {
        cam = Camera.main;
	}
	
	// Update is called once per frame
	void Update () {
        //On mouse left click
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray camRay = cam.ScreenPointToRay(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0));
            if (Physics.Raycast(camRay, out hit, 50, groundLayer))
            {
                GameObject effect = Instantiate(clickEffectPrefab,hit.point,Quaternion.identity,transform);
                Destroy(effect, 1.5f);
            }
        }
    }
}
