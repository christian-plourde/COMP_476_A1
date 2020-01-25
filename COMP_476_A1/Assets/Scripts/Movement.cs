﻿using UnityEngine;
using System.Collections;

public abstract class Movement
{
    private Car car;
    private Vector3 target;
    private GameObject target_obj;

    public Vector3 Target
    {
        get { return target; }
        set { target = value;
            //also update the position of the target
            target_obj.transform.position = target;
        }
    }

    public Car Car
    {
        get { return car; }
    }

    public Vector3 Position
    {
        get { return car.gameObject.transform.position; }
        set { car.gameObject.transform.position = value; }
    }

    public float MaxVelocity
    {
        get { return car.MaxVelocity; }
    }

    public Movement(Car car, GameObject target_obj)
    {
        this.car = car;
        this.target_obj = target_obj;
        this.target_obj = GameObject.Instantiate(target_obj, target, Quaternion.identity);
    }

    public abstract void Move();
}

public abstract class AlignedMovement : Movement
{
    protected const float time_to_target = 1.0f;
    protected const float radius_of_satisfaction = 0.02f;
    protected const float angular_radius_of_satisfaction = 0.05f;
    protected const float angular_slow_down_radius = 10.0f;
    protected const float angular_time_to_target = 1.0f;

    public AlignedMovement(Car car, GameObject target_obj) : base(car, target_obj) { }

    public float MaxAngularVelocity
    {
        get { return Car.MaxAngularVelocity; }
    }

    public float AngularVelocity
    {
        get { return Car.AngularVelocity; }
        set { Car.AngularVelocity = value; }
    }

    public float MaxAngularAcceleration
    {
        get { return Car.MaxAngularAcceleration; }
    }

    public float Orientation
    {
        get {
            if (Car.gameObject.transform.rotation.eulerAngles.y > 180.0f)
                return Car.gameObject.transform.rotation.eulerAngles.y - 360.0f;

            return Car.gameObject.transform.rotation.eulerAngles.y; }

        set {
            Quaternion quat = new Quaternion();
            Vector3 euler_angs = new Vector3(0.0f, value, 0.0f);
            quat.eulerAngles = euler_angs;
            Car.gameObject.transform.rotation = quat; }
    }

    public abstract void Align();
}

public class KinematicSeek : AlignedMovement
{
    public KinematicSeek(Car car, GameObject target_obj) : base(car, target_obj) { }

    public override void Move()
    {
        //Debug.Log("Car: " + Position.ToString() + " Target: " + Target.ToString());
        //------------------------------ VELOCITY DIRECTION --------------------------------//
        
        //the direction of the velocity is computed by taking the difference between the position of the target and the character
        Vector3 velocity_dir = Target - Position;
        //Now that we have the direction we need to normalize it
        velocity_dir = velocity_dir.normalized;

        //-------------------------------- SEEK VELOCITY ---------------------------------//

        //Now that the velocity direction has been computed we need to calculate the seek velocity
        //this is done by multiplying the max velocity of the car by the velocity direction (normalized)
        Vector3 seek_velocity = MaxVelocity * velocity_dir;

        //------------------------------- POSITION UPDATE -------------------------------//

        //the final step is to use the seek velocity to update the position of the car
        Position = Position + seek_velocity * Time.deltaTime;
    }

    public override void Align()
    {
        //----------------------------- LOOK DIRECTION ---------------------------//

        //the first thing we need to do here is determine the target orientation
        //this is done by taking the difference in the orientation of the line connection the car and target and the current orientation
        //of the car
        //first lets find the orientation of that line
        //lets get the direction from the position of the car to the target
        Vector3 dir = Target - Position;

        //next lets create the target rotation, which is the orientation to the target position
        Quaternion target_rotation = Quaternion.LookRotation(dir, Vector3.up);
        float target_rotation_euler = target_rotation.eulerAngles.y;

        //since we want the target rotation to be signed, if it is larger than 180 degrees, we will make it negative from 0
        if (target_rotation_euler > 180)
            target_rotation_euler = target_rotation_euler - 360.0f;

        //we only allow the character to rotate around the y_axis, therefore we only need the angle around y axis that separates
        //the car and the target

        float rotation_diff = target_rotation_euler - Orientation;

        //------------------------------------- RADIUS OF SATISFACTION CHECK ---------------------------------//
        
        //we need to check if we are within the radius of satifaction, it which case, we should stop rotating the character
        //set its orientation to that of the target, and return
        if(Mathf.Abs(rotation_diff) < angular_radius_of_satisfaction)
        {
            Orientation = target_rotation_euler;
            return;
        }

        //if this was not the case we need to keep rotating

        //-------------------------------------- GOAL ANGULAR VELOCITY -------------------------------------//

        //first step is to compute the goal angular velocity, which is the speed to the target based on time to target and angle from target
        float goal_angular_velocity = MaxAngularVelocity * rotation_diff / angular_slow_down_radius;

        //with the goal angular acceleration the character should have based on time to target
        float ang_acc = MaxAngularAcceleration;

        //we only change the angular acceleration if if its less than the max acceleration, otherwise acceleration is capped at its max
        float char_acc = (goal_angular_velocity - AngularVelocity) / angular_time_to_target;

        if (char_acc < ang_acc)
            ang_acc = char_acc;

        //------------------------------------ CAR VELOCITY ----------------------------------------//

        //Now that we have the car's accelertion, we can recompute its angular velocity

        AngularVelocity = AngularVelocity + ang_acc * Time.deltaTime;

        //------------------------------------- CAR ORIENTATION ---------------------------------//

        //finally we compute the new orientation based on the new velocity

        Orientation = Orientation + AngularVelocity * Time.deltaTime;
    }
}

public class KinematicArrive : KinematicSeek
{
    
    public KinematicArrive(Car car, GameObject target_obj) : base(car, target_obj) { }

    public override void Move()
    {
        //------------------------------ VELOCITY DIRECTION --------------------------------//

        //the direction of the velocity is computed by taking the difference between the position of the target and the character
        Vector3 velocity_dir = Target - Position;
        //Now that we have the direction we need to normalize it
        velocity_dir = velocity_dir.normalized;

        //----------------------------- VELOCITY MAGNITUDE -------------------------------//

        //the next step is to determine the magnitude of the velocity to use. This will either be the maximum velocity, or 
        //the velocity based on the time to target. Whichever is smallest will be the one we choose.
        float char_to_target_speed = (Target - Position).magnitude / time_to_target;
        float velocity = 0.0f;

        if (MaxVelocity < char_to_target_speed)
            velocity = MaxVelocity;

        else
            velocity = char_to_target_speed;

        //now that we have the velocity we will need to check if we should stop

        //-------------------------------- RADIUS OF SATISFACTION CHECK ----------------------//

        //to check if we should stop, we check how far we are to the target. If the distance is less than the radius of satisfaction,
        //we set the position of the car to the position of the target and return

        if ((Target - Position).magnitude < radius_of_satisfaction)
        {
            Position = Target;
            return;
        }

        //if we are outside the radius of satisfaction, we need to move with the prescribed velocity
        Vector3 v_move = velocity * velocity_dir;
        Position = Position + v_move * Time.deltaTime;
    }
}
