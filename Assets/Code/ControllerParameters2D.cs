using UnityEngine;
using System.Collections;
using System;



[Serializable] //InspectorWindow bearbeiten
public class ControllerParameters2D
{
    //Ziel: contains only parameters, zb slope limit, gravity limit usw - we can easy override this parameters this class
    public enum jumpBehavior
    {
        canJumpOnGround,
        canJumpAnywhere,
        cantJump
    }
    public Vector2 maxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float slopeLimit = 30;

    public float gravity = -25f;

    public jumpBehavior jumpRestriction;

    public float jumpFrequency = .25f;

    public float jumpMagnitued = 12;  //Add jumpMACHT! :P



}
