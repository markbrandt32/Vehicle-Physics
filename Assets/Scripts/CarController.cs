using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
    [SerializeField]
    private List<WheelCollider> axleSockets;
    //private WheelCollider m_leftWheel, m_rightWheel;

    [SerializeField]
    private bool m_isMotor, m_isSteering, m_isBraking;

    public List<WheelCollider> AxleSockets { get { return axleSockets; } }
    public bool IsMotor { get { return m_isMotor; } }
    public bool IsSteering { get { return m_isSteering; } }
    public bool IsBraking { get { return m_isBraking; } }
}

public class CarController : MonoBehaviour {

    [SerializeField]
    private List<AxleInfo> m_axleInfos;

    [SerializeField]
    private float m_maxMotorTorque, m_maxSteeringAngle, m_brakeTorque;

    [SerializeField]
    private Transform centerMass;

    void Start () {
        GetComponent<Rigidbody>().centerOfMass = centerMass.localPosition;
        RemoveCarJitter();
    }

    void FixedUpdate () {
        float motor = m_maxMotorTorque * Input.GetAxisRaw("Vertical");
        float steer = m_maxSteeringAngle * Input.GetAxisRaw("Horizontal");
        float brake = m_brakeTorque * Input.GetAxisRaw("Jump");

        foreach (AxleInfo axleInfo in m_axleInfos) {
            foreach (WheelCollider wheel in axleInfo.AxleSockets) {
                if (axleInfo.IsMotor) {
                    wheel.motorTorque = motor;
                }

                if (axleInfo.IsSteering) {
                    wheel.steerAngle = steer;
                }

                if (axleInfo.IsBraking) {
                    wheel.brakeTorque = brake;
                }

                ApplyVisualWheel(wheel);
            }
        }
    }

    public int[] GetAxleAndWheelCount() {
        int[] output = new int[m_axleInfos.Count];
        for (int i = 0; i < m_axleInfos.Count; i++) {
            output[i] = m_axleInfos[i].AxleSockets.Count;
        }

        return output;
    }

    public void SetWheel(WheelCollider wheel, int axleIndex, int socketIndex) {
        if (axleIndex < m_axleInfos.Count && axleIndex >= 0) {
            //This index in the axlesInfos List exists
            if (socketIndex < m_axleInfos[axleIndex].AxleSockets.Count && socketIndex >= 0) {
                //This index in the axleSockets List exists
                m_axleInfos[axleIndex].AxleSockets[socketIndex] = wheel;
            } else {
                Debug.LogError("The socketIndex a wheel is set does not exist!");
            }
        } else {
            Debug.LogError("The axleIndex a wheel is set does not exist!");
        }

        RemoveCarJitter();
    }

    private void RemoveCarJitter() {
        //Removes car jittering when turning or stopping
        foreach (AxleInfo axleInfo in m_axleInfos) {
            foreach (WheelCollider wheel in axleInfo.AxleSockets) {
                wheel.ConfigureVehicleSubsteps(5, 12, 15);
            }
        }
    }

    private void ApplyVisualWheel (WheelCollider collider) {
        if (collider.transform.childCount == 0) {
            Debug.LogError("No wheel visual found for " + collider.name 
                + ". Set the visual wheel as the child of the collider.");
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;

        WheelHit hit = new WheelHit();
        if (collider.GetGroundHit(out hit)) {
            //Debug.Log(hit.forwardSlip + " " + hit.sidewaysSlip);
            ParticleSystem.EmissionModule emission = collider.GetComponent<ParticleSystem>().emission;
            if (Mathf.Abs(hit.forwardSlip) >= collider.forwardFriction.extremumSlip || 
                Mathf.Abs(hit.sidewaysSlip) >= collider.sidewaysFriction.extremumSlip) {
                emission.enabled = true;
            } else {
                emission.enabled = false;
            }
        }
    }
}
