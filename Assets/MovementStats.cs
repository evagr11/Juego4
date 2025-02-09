using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]

//Uno para cada tipo de salto, un una lista

public class MovementStats : ScriptableObject
{
    [Header("Ground")]
    public float groundAcceleration;
    public float maxGroundHorizontalSpeed;
    public float groundHorizontalFriction;
    [Header("Jump")]
    public float onAirJumps;
    public float jumpStrength;
    public float maxJumpTime;
    [Header("Air")]
    public float airAcceleration;
    public float maxAirHorizontalSpeed;

    public float yVelocityLowGravityThreshold;
    public float maxFallSpeed;
    public float airFriccion;
    public float risingtGravity;
    public float peakGravity;
    public float fallingGravity;

}
