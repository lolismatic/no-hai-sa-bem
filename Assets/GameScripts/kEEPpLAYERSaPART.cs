using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class kEEPpLAYERSaPART : MonoBehaviour
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
    private PlayerGroupStatus _group;
    public PlayerGroupStatus group
    {
        get
        {
            if (_group == null)
            {
                _group = GetComponent<PlayerGroupStatus>();
            }
            return _group;
        }
    }

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


    public float swarmingForce = 0.1f;
    public float minDist = 0.3f;

    public float keepTogetherForce = 0.1f;
    public float distForKeepTogether = 0.6f;

    public bool keepTogether
    {
        get
        {
            return groupToBar.state == GroupToBarThing.GroupStates.TowardsBar;
        }
    }

    private void Update()
    {
        Vector3 away = Vector3.zero;
        var average = 0;

        if (group.leftGrabbed != null)
        {
            var towardsLEft = group.leftGrabbed.transform.position - transform.position;
            towardsLEft.y = 0;
            if (keepTogether)
            {
                if (towardsLEft.sqrMagnitude > distForKeepTogether * distForKeepTogether)
                {
                    away += towardsLEft.normalized;
                    average += 1;
                }
            }
            else
            {
                if (towardsLEft.sqrMagnitude < minDist * minDist)
                {
                    away += towardsLEft;
                    average += 1;
                }
            }
        }
        if (group.rightGrabbed != null)
        {
            var toRight = group.rightGrabbed.transform.position - transform.position;
            toRight.y = 0;
            if (keepTogether)
            {
                if (toRight.sqrMagnitude > distForKeepTogether * distForKeepTogether)
                {
                    away += toRight.normalized;
                    average += 1;
                }
            }
            else
            {
                if (toRight.sqrMagnitude < minDist * minDist)
                {
                    away += toRight;
                    average += 1;
                }
            }
        }

        transform.position += (keepTogether ? -1 * keepTogetherForce : 1f * swarmingForce)
            * away / Mathf.Max(1, average) * Time.deltaTime;

    }
}