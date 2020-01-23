using UnityEngine;
using System.Collections;

public abstract class Movement
{
    private Car car;
    private Vector3 target;

    public Vector3 Target
    {
        get { return target; }
        set { target = value; }
    }

    public Vector3 Position
    {
        get { return car.gameObject.transform.position; }
    }

    public Movement(Car car)
    {
        this.car = car;
    }

    public abstract void Move();

}

public class KinematicSeek : Movement
{
    public KinematicSeek(Car car) : base(car) { }

    public override void Move()
    {
        Debug.Log("Car: " + Position.ToString() + " Target: " + Target.ToString());
    }
}
