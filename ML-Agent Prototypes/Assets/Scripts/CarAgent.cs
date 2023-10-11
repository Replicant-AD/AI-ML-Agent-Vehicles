using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarAgent : Agent
{
    private CarController m_Car;
    private Rigidbody rb;
    public GameObject[] Checkpoints;

    public override void Initialize()
    {
        m_Car = GetComponent<CarController>();
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 0.06754f, -42.1f);
        this.transform.localRotation = new Quaternion(0, 0, 0, 0);
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        
        for (int i=0; i< Checkpoints.Length; i++){
            Checkpoints[i].gameObject.SetActive(true);
        }
        
    }

    
    public override void CollectObservations(VectorSensor sensor)
    {
        AddReward(-0.001f);
    }
    

    public override void OnActionReceived(float[] vectorAction)
    {
        Vector3 agentMovement = Vector3.zero;
        agentMovement.x = vectorAction[0];
        agentMovement.z = vectorAction[1];
        m_Car.Move(agentMovement.x, agentMovement.z, 0f, 0f);
    }

    public override void Heuristic(float[] actionsOut)
    {
        actionsOut[0] = Input.GetAxis("Horizontal");
        actionsOut[1] = Input.GetAxis("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Checkpoint")
        {
            AddReward(10.0f);
            other.gameObject.SetActive(false);
        }

        if (other.gameObject.tag == "Goal")
        {
            other.gameObject.SetActive(false);
            AddReward(10.0f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
