using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class FollowerVehicleAgent : Agent
{
    public Transform leadVehicle;  // Reference to the lead vehicle
    public float maxDistanceToLeadVehicle = 10f;  // Maximum desired distance from lead vehicle
    public float rewardForAlignment = 0.1f;  // Reward for aligning with lead vehicle
    public float penaltyForCollision = -1f;  // Penalty for colliding with another vehicle

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
        // Distance and relative velocity to lead vehicle
        sensor.AddObservation(leadVehicle.position - transform.position);
        sensor.AddObservation(leadVehicle.GetComponent<Rigidbody>().velocity - rb.velocity);

        // You can add more observations about other follower vehicles here, if needed.
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        int action = actionBuffers.DiscreteActions[0];
        switch (action)
        {
            case 0: // Move forward
                rb.AddForce(transform.forward * 1f);
                break;
            case 1: // Turn left
                transform.Rotate(0, -5f, 0);
                break;
            case 2: // Turn right
                transform.Rotate(0, 5f, 0);
                break;
        }

        // Check alignment with lead vehicle and reward
        float alignment = Vector3.Dot(transform.forward, leadVehicle.forward);
        AddReward(rewardForAlignment * alignment);

        // Check distance to lead vehicle and penalize if too far
        float distanceToLead = Vector3.Distance(transform.position, leadVehicle.position);
        if (distanceToLead > maxDistanceToLeadVehicle)
            AddReward(-rewardForAlignment);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.W))
            discreteActionsOut[0] = 0;
        else if (Input.GetKey(KeyCode.A))
            discreteActionsOut[0] = 1;
        else if (Input.GetKey(KeyCode.D))
            discreteActionsOut[0] = 2;
        else
            discreteActionsOut[0] = -1;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if collided with another follower vehicle and penalize
        if (collision.gameObject.CompareTag("FollowerVehicle"))
        {
            AddReward(penaltyForCollision);
            EndEpisode();
        }
    }
}
