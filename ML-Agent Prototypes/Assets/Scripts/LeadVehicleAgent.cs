using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class LeadVehicleAgent : Agent
{
    public Transform target;  // Reference to a target or endpoint, if there's a destination
    public float speed = 10f;  // Speed of the vehicle
    public float turnSpeed = 5f;  // Speed of turning or steering
    public float distancePenalty = -0.05f;  // Penalty for distance from center of the lane

    private Rigidbody rb;
    private Vector3 startingPosition;
    private Quaternion startingRotation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startingPosition = transform.position;
        startingRotation = transform.rotation;
    }

    public override void OnEpisodeBegin()
    {
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
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        float move = actionBuffers.ContinuousActions[0];
        float turn = actionBuffers.ContinuousActions[1];

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
        continuousActionsOut[0] = Input.GetKey(KeyCode.W) ? 1 : 0;  // Move forward
        continuousActionsOut[1] = Input.GetKey(KeyCode.A) ? -1 : (Input.GetKey(KeyCode.D) ? 1 : 0);  // Turn left or right
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