using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using NWH.VehiclePhysics2;


public class LeadVehicleAgent : Agent
{
    public VehicleController _vehicleController;
    public float speed = 10f;  // Speed of the vehicle
    public float turnSpeed = 2f;  // Speed of turning or steering
    public GameObject[] Checkpoints;
    private Rigidbody rb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    public override void Initialize()
    {
        _vehicleController = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        for (int i=0; i< Checkpoints.Length; i++){
            Checkpoints[i].gameObject.SetActive(true);
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {   
        // Current speed
        //sensor.AddObservation(rb.velocity);
        
        // Raycasts or sensors to detect road boundaries, obstacles, etc.
        // This is a basic example, in a real scenario, you'd use multiple raycasts/sensors
        //sensor.AddObservation(Physics.Raycast(transform.position, transform.forward, 10f));  // Detect obstacles in front

        AddReward(-0.001f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float move = actionBuffers.ContinuousActions[1];
        float turn = actionBuffers.ContinuousActions[0];

        // Movement
        rb.MovePosition(transform.position + transform.forward * move * speed * Time.fixedDeltaTime);
        transform.Rotate(0, turn * turnSpeed, 0);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);  // Move forward or backward
        continuousActionsOut[0] = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);  // Turn left or right
    }

     private void OnTriggerEnter(Collider other)
    {
        // If the agent collides with a checkpoint, add a reward and deactivate the checkpoint
        if (other.gameObject.tag == "Checkpoint")
        {
            AddReward(10.0f);
            other.gameObject.SetActive(false);
        }

        // If the agent collides with the goal, add a reward and end the episode
        if (other.gameObject.tag == "Goal")
        {
            other.gameObject.SetActive(false);
            AddReward(10.0f);
            EndEpisode();
        }
    }

     private void OnCollisionEnter(Collision collision)
    {
        // If the agent collides with the wall or road blocker, end the episode
        if (collision.gameObject.tag == "Wall")
        {
            SetReward(-10.0f);
            EndEpisode();
        }

        if (collision.gameObject.tag == "RoadBlocker")
        {
            SetReward(-10.0f);
            EndEpisode();
        }
    }
}
