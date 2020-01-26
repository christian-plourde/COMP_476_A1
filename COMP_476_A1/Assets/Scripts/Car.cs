using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private float max_velocity = 0.6f;
    private float velocity = 0.0f;
    public static float reset_offset = 0.8f;
    private AlignedMovement movement;
    public GameObject target_obj;

    private float max_angular_velocity = 20.0f;
    private float angular_velocity = 0.0f;
    private float max_angular_acceleration = 50.0f;

    private bool frozen = false;

    public AlignedMovement Movement
    {
        get { return movement; }
        set { movement = value; }
    }

    public float Velocity
    {
        get { return velocity; }
        set { velocity = value; }
    }

    public float MaxVelocity
    {
        get { return max_velocity; }
    }

    public float MaxAngularVelocity
    {
        get { return max_angular_velocity; }
    }

    public float AngularVelocity
    {
        get { return angular_velocity; }
        set { angular_velocity = value; }
    }

    public float MaxAngularAcceleration
    {
        get { return max_angular_acceleration; }
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = new Wander(this, target_obj);
        movement.Target = new Vector3(6, 0, 2);
    }

    // Update is called once per frame
    void Update()
    {
        if(!frozen)
        {
            movement.Move();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Tag"))
            frozen = true;

        if (col.gameObject.CompareTag("Car"))
            frozen = false;
    }
}
