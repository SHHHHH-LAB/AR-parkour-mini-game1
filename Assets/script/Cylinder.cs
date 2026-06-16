using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cylinder : MonoBehaviour
{

    public GameObject center;
    public GameObject rotationObject;
    public float rotationSpeed;
    public float forwardSpeed;

    private float minZ = -1;
    private float maxZ = 1f;
    private int direction = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotationObject.transform.RotateAround(center.transform.position, Vector3.up, Time.deltaTime * rotationSpeed);
       /* center.transform.Translate(new Vector3(0,0,direction*forwardSpeed*Time.deltaTime));
        float currrentZ=center.transform.position.z;
        if(currrentZ <=minZ)
        {
            direction = 1;
        }
        if (currrentZ >= maxZ)
        {
            direction = -1;
        }*/
    }
}
