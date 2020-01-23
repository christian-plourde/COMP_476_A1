using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Car : MonoBehaviour
{
    private const float max_velocity = 0.03f;
    public static float reset_offset = 0.8f;
    public Movement movement;

    public float MaxVelocity
    {
        get { return max_velocity; }
    }

    // Start is called before the first frame update
    void Start()
    {
        movement = new KinematicSeek(this);
    }

    // Update is called once per frame
    void Update()
    {
        gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z + MaxVelocity);
        movement.Move();
    }
}
