using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    [SerializeField]
    private Axis[] wheels;

    [SerializeField]
    private GameObject centerOfMass = null;

    [SerializeField]
    private float steeringAngle = 20;

    [SerializeField]
    private float brake = 300;

    [SerializeField]
    private float torgue = 120;

    private Rigidbody rb;

[SerializeField]
    private float motorRpm = 0;

    [SerializeField]
    private float idleRpm = 1300;

    [SerializeField]
    private float peakRpm = 6000;
    
    [SerializeField]
    private float maxRpm = 8000;

    [SerializeField]
    private float peakTorgue = 150;

    [SerializeField]
    private float transmitionCoff = 1;

    [SerializeField]
    private List<float> gearsCoffs;

    [SerializeField]
    private int gear = -1;

    [SerializeField]
    private bool shift = false;

[SerializeField]
    private float clutch = 0;



    // Start is called before the first frame update
    void Start()
    {
        rb = this.GetComponent<Rigidbody>();       
    }

    // Update is called once per frame
    void Update()
    {
        float throttle = Input.GetAxis("Vertical");
        float rpmDelta = 0.01f - (clutch*0.009f);
        float wheelsRpm = Mathf.Abs(getDrivableWheelsAvgRpm());
        if (gear == -1) {
            transmitionCoff = 0;
        } else {
            transmitionCoff = gearsCoffs[gear];
        }
        //rpm = wheel rotation rate * gear ratio * differential ratio * 60 / 2p
        Debug.Log(wheelsRpm * transmitionCoff * 50 / 2 * Mathf.PI);//wheelsRpm*transmitionCoff);
        Debug.Log(rb.velocity.magnitude);

        if (throttle > 0) {
            if (gear == -1) {
                    gear = 0;
            }

            if (motorRpm <= peakRpm && !shift) {
                clutch = 1;
                motorRpm = Mathf.Lerp(motorRpm, maxRpm, rpmDelta);
                // motorRpm = Mathf.Clamp(Mathf.Lerp(motorRpm, idleRpm+wheelsRpm, rpmDelta), idleRpm, maxRpm);
            } else {
                if (gear != gearsCoffs.Count - 1) {
                    shift = true;
                }
                
                if (shift) {
                    clutch = 0;
                    motorRpm = Mathf.Lerp(motorRpm, peakRpm*0.8f, rpmDelta);
                    if (Mathf.Abs(peakRpm*0.8f - motorRpm) < 100) {
                        shift = false;
                        gear++;
                    }
                }
            }
        } else {
            clutch = 0;
        }

        // if (gear == -1) {
        //     transmitionCoff = 0;
        // } else {
        //     transmitionCoff = gearsCoffs[gear];
        // }

        foreach (Axis axis in wheels)
        {
            if (axis.isSteering()) {
                axis.applySteering(Input.GetAxis("Horizontal")*steeringAngle);
            }
            
            if (axis.isDrivable()) {

                // кооэфициент, который постепенно наращивает момент двигателя до пикового и потом снижает его
                var rpmCoff = motorRpm/peakRpm;
                if (rpmCoff > 1) {
                    rpmCoff = 2 - rpmCoff;
                }
                // axis.applyTorgue(clutch*transmitionCoff*(torgue*rpmCoff*motorRpm/9550));
                axis.applyTorgue(clutch*(transmitionCoff*torgue*rpmCoff));
            }

            if (Input.GetKeyDown(KeyCode.Space)) {
                axis.applyBrake(brake);
            }
            if (Input.GetKeyUp(KeyCode.Space)) {
                axis.applyBrake(0);
            }

            axis.updateVisual();
        }



        if (throttle == 0) {
            if (gear == -1) {
                motorRpm = Mathf.Lerp(motorRpm, idleRpm, rpmDelta);
            } else {
                motorRpm = Mathf.Lerp(motorRpm, idleRpm, 0.0001f);
            }
        }

        if (centerOfMass != null) {
            rb.centerOfMass = centerOfMass.transform.localPosition;
        }
    }


    private float getDrivableWheelsAvgRpm() {
        float rpm = 0f;
        int count = 0;

        foreach (Axis axis in wheels)
        {
            if (axis.isDrivable()) {
                    rpm += axis.getWheel(Axis.AxisSide.LEFT).getCollider().rpm;
                    rpm += axis.getWheel(Axis.AxisSide.RIGHT).getCollider().rpm;
                    count += 2;
            }
        }

        return rpm/count;
    }
    
}

