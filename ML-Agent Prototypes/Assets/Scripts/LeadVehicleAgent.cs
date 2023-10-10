using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using NWH.VehiclePhysics2;


public class LeadVehicleAgent : Agent
{
    public CheckpointManager _checkpointManager;
    public float speed = 10f;  // Speed of the vehicle
    public float turnSpeed = 4f;  // Speed of turning or steering
    public float MaxTimeToReachNextCheckpoint = 45f;
    public VehicleController _vehicleController;

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
        _checkpointManager.ResetCheckpoints();
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public override void CollectObservations(VectorSensor sensor)
    {   
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

        //Time efficiency reward
        float timeTaken = MaxTimeToReachNextCheckpoint - _checkpointManager.TimeLeft;
        float timeReward = Mathf.Clamp01(1 - timeTaken / MaxTimeToReachNextCheckpoint);
        AddReward(timeReward);

        // Reward logic
        float distanceToCheckpoint = Vector3.Distance(transform.position, _checkpointManager.nextCheckPointToReach.transform.position);
        AddReward(-distanceToCheckpoint / 1000f);  // Normalize by some factor

        float currentSpeed = rb.velocity.magnitude;
        if (Mathf.Abs(currentSpeed - speed) < 2f)  // Tolerance of 2 units
        {
            AddReward(0.05f);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
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
