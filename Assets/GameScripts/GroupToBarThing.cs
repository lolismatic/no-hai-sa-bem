using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class GroupToBarThing : MonoBehaviour
{
    [SerializeField]
    private PlayerGroupStatus _groupStatus;
    public PlayerGroupStatus groupStatus
    {
        get
        {
            if (_groupStatus == null)
            {
                _groupStatus = GetComponent<PlayerGroupStatus>();
            }
            return _groupStatus;
        }
    }

    [SerializeField]
    private RagdollTool _ragdollTool;
    public RagdollTool ragdollTool
    {
        get
        {
            if (_ragdollTool == null)
            {
                _ragdollTool = GetComponent<RagdollTool>();
            }
            return _ragdollTool;
        }
    }

    public bool applyForceOnAlone = false;
    public float forceOnAlone = 10f;

    public enum GroupStates
    {
        FromBar, // the default state, everyone starts together from bar. can go home connected
        LostAndAlone, // when losing connection to friends, this state can only be solved by going towards bar and returning to the initial FromBar
        TowardsBar, // when meeting someone, start going towards bar. cannot ungroup, can only go to bar and reset to FromBar, adding score...

        Unaffected, // rules don't apply to this guy...???
    }

    public GroupStates state = GroupStates.FromBar;
    public event System.Action<GroupStates> OnStateChange;

    [SerializeField]
    private ParticleSystem _fromBarParticles;
    public ParticleSystem fromBarParticles
    {
        get
        {
            if (_fromBarParticles == null)
            {
                _fromBarParticles = GetComponentInChildren<ParticleSystem>();
            }
            return _fromBarParticles;
        }
    }


    private void OnEnable()
    {
        groupStatus.OnGroup += GroupStatus_OnGroup;
        groupStatus.OnUngroup += GroupStatus_OnUngroup;
        TriggerChangedState();
    }

    private void OnDisable()
    {
        groupStatus.OnGroup -= GroupStatus_OnGroup;
        groupStatus.OnUngroup -= GroupStatus_OnUngroup;
    }

    private void Update()
    {
        if (BarManager.instance.collider.bounds.Contains(transform.position))
        {
            if (state == GroupStates.TowardsBar)
            {
                state = GroupStates.FromBar;
                TriggerChangedState();
                MakeConnectionsHaveSpecificState(GroupStates.FromBar);

                //fromBarParticles.Play();

                // RespawnAtBar
                BarManager.instance.RespawnAt(groupStatus, BarManager.instance.restartPosition);

            }
        }
    }

    private void GroupStatus_OnUngroup()
    {
        if (state == GroupStates.FromBar)
        {
            if (groupStatus.leftGrabbed == null && groupStatus.rightGrabbed == null)
            {
                // we just lost our friends, become alone.
                state = GroupStates.LostAndAlone;
                TriggerChangedState();

                if (applyForceOnAlone)
                {
                    ragdollTool.Ragdoll(true, forceOnAlone, 0);
                }
            }
        }
    }

    private void GroupStatus_OnGroup()
    {
        if (state == GroupStates.LostAndAlone || state == GroupStates.FromBar)
        {
            // when we grouped from being lost, we go towards bar. so direction vector is always towards bar, plus the input. instead of ever being able to go away from bar.
            state = GroupStates.TowardsBar;
            TriggerChangedState();

            // make the others also go towards bar, regardless of their current state.
            MakeConnectionsHaveSpecificState(GroupStates.TowardsBar);
        }

    }

    private void MakeConnectionsHaveSpecificState(GroupStates state)
    {
        var init = groupStatus;
        var pointer = groupStatus.leftGrabbed;
        while (pointer != null && pointer != init)
        {
            var grp = pointer.GetComponent<GroupToBarThing>();
            grp.state = state;
            TriggerChangedState();
            pointer = pointer.leftGrabbed;
        }
        pointer = groupStatus.rightGrabbed;
        while (pointer != null && pointer != init)
        {
            var grp = pointer.GetComponent<GroupToBarThing>();
            grp.state = state;
            TriggerChangedState();
            pointer = pointer.rightGrabbed;
        }
    }

    private void TriggerChangedState()
    {
        if (OnStateChange != null)
        {
            OnStateChange(state);
        }
    }

    private void OnDrawGizmos()
    {
        if (state == GroupStates.FromBar)
        {
            Gizmos.color = Color.green;
        }
        else if (state == GroupStates.LostAndAlone)
        {
            Gizmos.color = Color.red;
        }
        else if (state == GroupStates.TowardsBar)
        {
            Gizmos.color = Color.cyan;
        }

        Gizmos.DrawSphere(transform.position + Vector3.up * 4, 0.5f);

    }
}
