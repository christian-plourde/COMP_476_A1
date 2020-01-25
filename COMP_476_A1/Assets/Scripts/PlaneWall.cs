using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaneWall : MonoBehaviour
{
    public GameObject pair_wall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter(Collider col)
    {
        if (!col.gameObject.CompareTag("Tag") && !col.gameObject.CompareTag("Car"))
            return;

        //on collision entry, check the position of the pair wall compared to that of its pair wall
        col.transform.position += (pair_wall.transform.position - transform.position);

        Vector3 diff = pair_wall.transform.position - transform.position;
        if (diff.x < 0)
            col.transform.position = new Vector3(col.transform.position.x + Car.reset_offset, col.transform.position.y, col.transform.position.z);
    
        else if(diff.x > 0)
            col.transform.position = new Vector3(col.transform.position.x - Car.reset_offset, col.transform.position.y, col.transform.position.z);
    
        if(diff.z < 0)
            col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, col.transform.position.z + Car.reset_offset);
    
        else if (diff.z > 0)
            col.transform.position = new Vector3(col.transform.position.x, col.transform.position.y, col.transform.position.z - Car.reset_offset);

    }
}
