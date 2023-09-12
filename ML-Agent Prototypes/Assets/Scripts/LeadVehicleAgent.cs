using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using NWH.VehiclePhysics2;


public class LeadVehicleAgent : Agent
{
    public CheckpointManager _checkpointManager;
    public Transform target;  // Reference to a target or endpoint, if there's a destination
    public float speed = 5f;  // Speed of the vehicle
    public float turnSpeed = 2f;  // Speed of turning or steering
    public float distancePenalty = -0.05f;  // Penalty for distance from center of the lane
    public VehicleController _vehicleController;

    private Rigidbody rb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    public override void Initialize()
    {
        _vehicleController = GetComponent<VehicleController>();
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
        _checkpointManager.ResetCheckpoints();
        transform.position = startingPosition;
        transform.rotation = startingRotation;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Relative direction to target or endpoint
        sensor.AddObservation(target.position - transform.position);
        
        // Current speed
        sensor.AddObservation(rb.velocity);
        
        // Raycasts or sensors to detect road boundaries, obstacles, etc.
        // This is a basic example, in a real scenario, you'd use multiple raycasts/sensors
        sensor.AddObservation(Physics.Raycast(transform.position, transform.forward, 10f));  // Detect obstacles in front

        //Checkpoint manager
        Vector3 diff = _checkpointManager.nextCheckPointToReach.transform.position - transform.position;
        sensor.AddObservation(diff / 20f);
        AddReward(-0.001f);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float move = actionBuffers.ContinuousActions[1];
        float turn = actionBuffers.ContinuousActions[0];

        // Movement
        rb.MovePosition(transform.position + transform.forward * move * speed * Time.fixedDeltaTime);
        transform.Rotate(0, turn * turnSpeed, 0);

        // Check if the vehicle is off the road or too close to the edges and penalize
        // This is a basic example, you'd need a more complex system to detect road boundaries in a real scenario
        if (IsOffRoad())
        {
            AddReward(distancePenalty);
            EndEpisode();
        }

        // Reward for moving closer to the target (optional)
        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        AddReward(-distanceToTarget * 0.001f);
    }

    private bool IsOffRoad()
    {
        // Here, you might check the vehicle's position against road boundaries or use raycasting to check the terrain below the vehicle.
        // Returning 'false' by default for the example's sake.
        return false;  
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[1] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetKey(KeyCode.W) ? 1 : (Input.GetKey(KeyCode.S) ? -1 : 0);  // Move forward or backward
        continuousActionsOut[0] = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);  // Turn left or right
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Penalize for collisions
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            AddReward(-1);
            EndEpisode();
        }
    }
}
