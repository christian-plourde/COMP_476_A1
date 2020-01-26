using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class is responsible for managing the state of the game at any point in time. It is reponsible for assigning the tag at the
 * start of the game, and keeping track of who is frozen as well as assigning movement types to the players as well as their
 * targets.
 */

public class GameManager : MonoBehaviour
{
    private bool target_caught = false;
    private Car[] cars;

    public Car[] Cars
    {
        get { return cars; }
    }

    public bool CarFrozen
    {
        get
        {
            foreach (Car c in cars)
            {
                if (c.Frozen)
                    return true;
            }

            return false;
        }
    }
    
    public bool TargetCaught
    {
        set { target_caught = value; }
    }

    private Car CurrentTag
    {
        get
        {
            foreach (Car c in cars)
            {
                if (c.IsTag)
                    return c;
            }

            return null;
        }
    }

    private Car CurrentTagTarget
    {
        get 
        { 
            foreach(Car c in cars)
            {
                if (c.IsTagTarget)
                    return c;
            }

            return null;
        }

        set
        {
            foreach(Car c in cars)
            {
                if (c.IsTagTarget)
                    c.IsTagTarget = false;

                if (c == value)
                    c.IsTagTarget = true;
            }
        }
    }

    private void Initialize()
    {
        //Assigns the tag randomly at the start of the game
        int r = Random.Range(0, cars.Length);

        int i = 0;

        foreach(Car c in cars)
        {
            c.Frozen = false;

            if (i == r)
            {
                //assign this person as the tag
                c.IsTag = true;

                //also if we have found the tag we need to assign a target to him
                
                if (r == cars.Length - 1)
                {
                    c.Movement = new Pursue(c, cars[0]);
                    c.IsTag = true;
                    cars[0].IsTagTarget = true;
                }

                else
                {
                    c.Movement = new Pursue(c, cars[r + 1]);
                    c.IsTag = true;
                    cars[r + 1].IsTagTarget = true;
                }
            }
                
            else
            {
                c.IsTag = false;
                
            }
                
            i++;
        }

        foreach(Car c in cars)
        {
            if(!c.IsTag)
            {
                if (c.IsTagTarget)
                {
                    c.Movement = new Evade(c, CurrentTag);
                    c.IsTagTarget = true;
                }

                else
                    c.Movement = new Wander(c);
            }
        }
    }

    private Car SelectRandomTarget()
    {
        //this will return a random target for the tag to chase
        //we need to make sure the target is not frozen

        foreach(Car c in cars)
        {
            if(c != CurrentTag && !c.Frozen && c != CurrentTagTarget)
            {
                return c;
            }
        }

        return null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //---------------------- GET REFERENCE TO EACH CAR SCRIPT ---------------------//

        //Here we are getting a reference to each car script in the game world.
        //we will then assign one of them to be the tag at random
        cars = FindObjectsOfType<Car>();

        Initialize();   
    }

    // Update is called once per frame
    void Update()
    {
        //if the number of walls the tag target has gone through is too high, then update the tag target. This prevents deadlock
        //if the car is going through the wall constantly and the seeker is always turning around.
        if(CurrentTagTarget.PassedWalls >= Car.PassedWallLimit)
        {
            target_caught = true;
        }

        if(target_caught)
        {
            CurrentTagTarget = SelectRandomTarget();

            if (CurrentTagTarget == null)
            {
                Initialize();
            }
            
            else
            {
                CurrentTag.Movement = new Pursue(CurrentTag, CurrentTagTarget);
                CurrentTagTarget.IsTagTarget = true;
                CurrentTagTarget.Movement = new Evade(CurrentTagTarget, CurrentTag);
            }

            target_caught = false;
        }
    }
}
