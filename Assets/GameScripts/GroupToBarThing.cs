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

    public enum GroupStates
    {
        FromBar, // the default state, everyone starts together from bar. can go home connected
        LostAndAlone, // when losing connection to friends, this state can only be solved by going towards bar and returning to the initial FromBar
        TowardsBar, // when meeting someone, start going towards bar. cannot ungroup, can only go to bar and reset to FromBar, adding score...

        Unaffected, // rules don't apply to this guy...???
    }

    public GroupStates state = GroupStates.FromBar;

    private void OnEnable()
    {
        groupStatus.OnGroup += GroupStatus_OnGroup;
        groupStatus.OnUngroup += GroupStatus_OnUngroup;
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
                groupStatus.AllowUngrouping(this);
                state = GroupStates.FromBar;
            }
        }
    }

    private void GroupStatus_OnUngroup()
    {
        if (state == GroupStates.FromBar)
        {
            // we just lost our friends, become alone.
            state = GroupStates.LostAndAlone;
        }
        else if (state == GroupStates.LostAndAlone)
        {
            // cannot get further lost...
            // no-op
        }
        else if (state == GroupStates.TowardsBar)
        {
            // ungrouped from towards bar??? :/ impossible!!!! ERROR
            // no-op
        }
    }

    private void GroupStatus_OnGroup()
    {
        if (state == GroupStates.LostAndAlone)
        {
            // when we grouped from being lost, we go towards bar. so direction vector is always towards bar, plus the input. instead of ever being able to go away from bar.
            state = GroupStates.TowardsBar;

            // and we cannot ungroup.
            groupStatus.DisallowUngrouping(this);
        }
    }
}
