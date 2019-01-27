using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControlAnims : MonoBehaviour
{
    [SerializeField]
    private Player _player;
    public Player player
    {
        get
        {
            if (_player == null)
            {
                _player = GetComponent<Player>();
            }
            return _player;
        }
    }

    [SerializeField]
    private Animator _anim;
    public Animator anim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponentInChildren<Animator>();
            }
            return _anim;
        }
    }

    [Header("Input")]
    public float minInput = 0.1f;
    public float animSmoothing = 0.1f;
    Vector3 lastAnimDir;

    [Header("Velocities")]
    public float animSpeedBoostTowardsPlayers = 0.5f;
    public Vector2 distRangeToPlayerCenterToSpeedBoost = new Vector2(1, 2);
    public Vector2 angleRangeToPlayerCenterToSpeedBoost = new Vector2(30f, 90f);

    public Vector3 curDir;
    public float curSpeed;

    [SerializeField]
    private GroupToBarThing _groupToBar;
    public GroupToBarThing groupToBar
    {
        get
        {
            if (_groupToBar == null)
            {
                _groupToBar = GetComponent<GroupToBarThing>();
            }
            return _groupToBar;
        }
    }
    public float towardsBarMulti = 1f;
    public float towardsBarMaxVelocity = 1f;

    #region stolen from skijump AI ;)
    [Header("Rotation")]
    [Tooltip("Body turning smoothness override for target movement")]
    [SerializeField]
    private AnimationCurve _distToTargetToBodyTurningSmoothness = new AnimationCurve() { keys = new Keyframe[] { new Keyframe(0, 1f, 0, 0), new Keyframe(3, 0.036f, 0, 0) } };

    [SerializeField]
    private float _bodyTurningSmoothnessDefault = 0.05f;
    private float forceMaxRotationSmoothing;

    public void RotationUpdate(Vector3 movementDir)
    {
        // set rotation to where we are moving
        var e = movementDir;
        e = Vector3.ProjectOnPlane(e, Vector3.up);
        if (movementDir == Vector3.zero)
        {
            return;
        }
        var targetDistance = e.magnitude;
        var hasMoveTarget = false;
        var bodyTurningSmoothness = hasMoveTarget ? _distToTargetToBodyTurningSmoothness.Evaluate(targetDistance) : _bodyTurningSmoothnessDefault;
        bodyTurningSmoothness = Mathf.Max(bodyTurningSmoothness, forceMaxRotationSmoothing);
        transform.forward = Vector3.Lerp(transform.forward, e, bodyTurningSmoothness);
    }
    #endregion

    private void OnValidate()
    {
        if (anim != null)
        {
        }
    }

    private void Update()
    {
        var playerIn = InputManager.instance.GetPlayerInput(player.playerId);

        var xzDirection = new Vector3(playerIn.x, 0, playerIn.y);

        if (this.groupToBar.state == GroupToBarThing.GroupStates.TowardsBar)
        {
            var towardsBar = (BarManager.instance.barPosition.position - transform.position).normalized * towardsBarMulti;
            xzDirection = (xzDirection + towardsBar) / 2;
            xzDirection = Vector3.ClampMagnitude(xzDirection, towardsBarMaxVelocity);
        }

        if (xzDirection.sqrMagnitude < minInput * minInput)
        {
            xzDirection = Vector3.zero;
        }

        lastAnimDir = Vector3.MoveTowards(lastAnimDir, xzDirection, animSmoothing);

        // rotate
        if (lastAnimDir != Vector3.zero)
        {
            Update_RotateTowards(lastAnimDir);
        }

        // move (set even if zero... because anims)
        Update_MoveTowards(lastAnimDir, playerIn != Vector2.zero);


    }


    private void Update_MoveTowards(Vector3 dir, bool inputActive)
    {
        var speed = dir.magnitude;
        if (inputActive)
        {
            // if speed is towards player center of gravity, give it some boost.
            var playerCenterOfGravity = PlayerCenterOfGravityManager.instance.centerOfGravity;
            var toCenter = playerCenterOfGravity - transform.position;
            var angleToCenter = Vector3.Angle(dir, toCenter);
            // 0..1 parameter where 1 is MINIMUM angle, 0 is maximum angle, so we can multiply with the speed boost effect DIRECTLY without more operations
            var angleParam = Mathf.InverseLerp(angleRangeToPlayerCenterToSpeedBoost.y, angleRangeToPlayerCenterToSpeedBoost.x, angleToCenter);

            // 0..1 param where 1 is max distance, 0 is min distance, so we can multiply with speed boost => speed boost is max when we are FAR.
            var distParam = Mathf.InverseLerp(distRangeToPlayerCenterToSpeedBoost.x, distRangeToPlayerCenterToSpeedBoost.y, toCenter.magnitude);

            speed += angleParam * animSpeedBoostTowardsPlayers * distParam;
        }

        curSpeed = speed;
        curDir = dir.normalized * speed;

        anim.SetFloat("Walk", speed);

    }

    private void Update_RotateTowards(Vector3 dir)
    {
        // shitty but reliable
        //transform.LookAt(transform.position + dir);

        RotationUpdate(dir);

    }

    [DebugButton]
    private void SetKinematicRBs(bool kinematic)
    {
        foreach (var rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = kinematic;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.Lerp(Color.green, Color.yellow, Mathf.Clamp01(curSpeed));
        if (curSpeed >= 1)
        {
            Gizmos.color = Color.Lerp(Color.yellow, Color.red, (curSpeed - 1) / animSpeedBoostTowardsPlayers);
        }
        Gizmos.DrawLine(transform.position + Vector3.up * 0.2f, transform.position + Vector3.up * 0.2f + curDir);

    }
}
