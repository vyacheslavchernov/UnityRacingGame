using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Axis
{
    [SerializeField]
    [SerializeReference]
    private WheelCollider leftCollider;

    [SerializeField]
    [SerializeReference]
    private WheelCollider rightCollider;

    [SerializeField]
    [SerializeReference]
    private GameObject leftVisual;

    [SerializeField]
    [SerializeReference]
    private GameObject rightVisual;

    [SerializeField]
    private bool drivable = false;

    [SerializeField]
    private bool steering = false;

    public void applyTorgue(float torgue) {
        this.leftCollider.motorTorque = torgue;
        this.rightCollider.motorTorque = torgue;
    }

    public void applyBrake(float torgue) {
        this.leftCollider.brakeTorque = torgue;
        this.rightCollider.brakeTorque = torgue;
    }

    public void applySteering(float angle) {
        this.leftCollider.steerAngle = angle;
        this.rightCollider.steerAngle = angle;
    }

    public void updateVisual() {
        applyColliderTransformToVisual(leftCollider, leftVisual);
        applyColliderTransformToVisual(rightCollider, rightVisual);
    }

    private void applyColliderTransformToVisual(WheelCollider collider, GameObject visual) {
        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
     
        visual.transform.position = position;
        visual.transform.rotation = rotation;
    }

    public Wheel getWheel(AxisSide axisSide) {
        if (axisSide.Equals(AxisSide.LEFT)) {
            return new Wheel().setCollider(this.leftCollider).setVisual(this.leftVisual);
        }
        return new Wheel().setCollider(this.rightCollider).setVisual(this.rightVisual);
    }

    public bool isDrivable() {
        return this.drivable;
    }

    public bool isSteering() {
        return this.steering;
    }

    public enum AxisSide {
        LEFT,
        RIGHT
    }

    public class Wheel {
        private WheelCollider collider;
        private GameObject visual;

        public WheelCollider getCollider() {
            return this.collider;
        }

        public GameObject getVisual() {
            return this.visual;
        }

        public Wheel setCollider(WheelCollider collider) {
            this.collider = collider;
            return this;
        }

        public Wheel setVisual(GameObject visual) {
            this.visual = visual;
            return this;
        }
    }
}
