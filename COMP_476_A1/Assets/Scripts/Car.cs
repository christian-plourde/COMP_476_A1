using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private float max_velocity = 0.6f;
    private float velocity = 0.0f;
    public static float reset_offset = 0.8f;
    private AlignedMovement movement;
    private Color tag_color = Color.red;
    private Color car_color = Color.blue;
    private Color tag_target_color = Color.green;

    private float max_angular_velocity = 40.0f;
    private float angular_velocity = 0.0f;
    private float max_angular_acceleration = 50.0f;

    private bool frozen = false;
    private bool tag_target = false;

    private int passed_wall_count = 0;
    private static int passed_wall_limit = 3;

    private static GameManager game_manager;

    public static int PassedWallLimit
    {
        get { return passed_wall_limit; }
    }

    public int PassedWalls
    {
        get { return passed_wall_count; }
    }

    public void IncrementPassedWalls()
    {
        passed_wall_count++;

        if(passed_wall_count > passed_wall_limit)
        {
            passed_wall_count = 0;
        }
    }

    public bool Frozen
    {
        get { return frozen; }
        set { frozen = value; }
    }

    public Vector3 Position
    {
        get { return transform.position; }
    }

    public bool IsTag
    {
        get { return (this.CompareTag("Tag")); }
        set {
            if (value)
            {
                this.tag = "Tag";
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[0].color = tag_color;
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[1].color = tag_color;
            }
                
            else
            {
                this.tag = "Car";
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[0].color = car_color;
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[1].color = car_color;
            }
        }
    }

    public bool IsTagTarget
    {
        get { return tag_target; }
        set { tag_target = value; 
        
            if(tag_target)
            {
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[0].color = tag_target_color;
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[1].color = tag_target_color;
            }
                
            if(!tag_target)
            {
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[0].color = car_color;
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[1].color = car_color;
            }

            if (IsTag)
            {
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[0].color = tag_color;
                this.gameObject.transform.Find("body").gameObject.GetComponent<MeshRenderer>().materials[1].color = tag_color;
            }
        }
    }

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
        get {
            if (IsTag)
                return 2.5f * max_velocity;
            else if (game_manager.CarFrozen && !IsTagTarget)
            {
                return 4.0f * max_velocity;
            }
            return max_velocity; }
        set { max_velocity = value; }
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
        game_manager = FindObjectOfType<GameManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(!frozen)
        {
            movement.Move();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("Tag"))
        {
            if(IsTagTarget)
            {
                //when the tag collides with a car set ourselves to frozen and reset the target for the tag
                frozen = true;
                game_manager.TargetCaught = true;
            }
        }
            

        if (col.gameObject.CompareTag("Car"))
        {
            frozen = false;
            if(!col.GetComponent<Car>().IsTagTarget)
                col.GetComponent<Car>().Movement = new Wander(col.GetComponent<Car>());
        }

        if (game_manager.CarFrozen)
        {
            Car frozen_car = game_manager.Cars[0];

            foreach (Car c in game_manager.Cars)
            {
                if (c.Frozen)
                {
                    frozen_car = c;
                }
            }

            foreach (Car c in game_manager.Cars)
            {
                if (!c.IsTag && !c.IsTagTarget)
                {
                    c.Movement = new KinematicArrive(c);
                    c.Movement.Target = frozen_car.Position;
                }
            }
        }
    }
}
