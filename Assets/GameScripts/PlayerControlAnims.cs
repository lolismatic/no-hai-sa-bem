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

    [Space]
    public float minInput = 0.1f;
    public float animSmoothing = 0.2f;
    float lastMoveMagnitude = 0f;

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

        if (xzDirection.sqrMagnitude < minInput * minInput)
        {
            xzDirection = Vector3.zero;
        }

        // rotate
        if (xzDirection != Vector3.zero)
        {
            Update_RotateTowards(xzDirection);
        }

        // move (set even if zero... because anims)
        Update_MoveTowards(xzDirection);

    }

    private void Update_MoveTowards(Vector3 xzDirection)
    {
        lastMoveMagnitude = Mathf.Lerp(lastMoveMagnitude, xzDirection.magnitude, animSmoothing);
        anim.SetFloat("Walk", lastMoveMagnitude);
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
}
