using RootMotion;
using RootMotion.FinalIK;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class PlayerGroupStatus : MonoBehaviour
{
    public static Dictionary<Player, PlayerGroupStatus> playerGroupStatus = new Dictionary<Player, PlayerGroupStatus>();

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
    private PlayerContactPoints _playerContactPoints;
    public PlayerContactPoints playerContactPoints
    {
        get
        {
            if (_playerContactPoints == null)
            {
                _playerContactPoints = GetComponent<PlayerContactPoints>();
            }
            return _playerContactPoints;
        }
    }

    [SerializeField]
    private FullBodyBipedIK _ik;
    public FullBodyBipedIK ik
    {
        get
        {
            if (_ik == null)
            {
                _ik = GetComponent<FullBodyBipedIK>();
            }
            return _ik;
        }
    }

    public static PlayerGroupStatus Get(Player fromPlayer)
    {
        PlayerGroupStatus gg = null;
        playerGroupStatus.TryGetValue(fromPlayer, out gg);
        return gg;
    }

    public float minDistToGrab = 1f;
    public float maxDistToUngrab = 2f;

    public PlayerGroupStatus leftGrabbed;
    public PlayerGroupStatus rightGrabbed;

    private List<PlayerGroupStatus> nearestGrabbableLeft = new List<PlayerGroupStatus>();
    private List<PlayerGroupStatus> nearestGrabbableRight = new List<PlayerGroupStatus>();

    public event System.Action OnGroup;
    public event System.Action OnUngroup;

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

    // can ungroup as long as we are not going towards bar.
    public bool canUngroup
    {
        get
        {
            return groupToBar.state != GroupToBarThing.GroupStates.TowardsBar;
        }
    }
    
    [Space]
    public float groupRotateSmooth = 0.1f;

    private void Awake()
    {
        playerGroupStatus[player] = this;
    }

    private void OnValidate()
    {
        if (player != null)
        {
        }
        if (playerContactPoints != null)
        {
        }
        if (ik != null)
        {
        }
    }

    private void Update()
    {
        UpdateIkWeights();

        RotateUpdate_ToGroup();

        // clear cache of nearby players
        nearestGrabbableLeft.Clear();
        nearestGrabbableRight.Clear();

        // check all players in world...
        for (int i = 0; i < Player.allPlayers.Count; i++)
        {
            var other = Player.allPlayers[i];
            if (other != player)
            {
                var otherGS = other.GetGroupStatus();
                var otherCP = other.GetContactPoints();
                var feetDist = (otherCP.player.transform.position - player.transform.position).magnitude;

                // grab states
                // if left is free, must find the nearest available RIGHT to grab.

                // we are already grabbing the other player.
                if (leftGrabbed == otherGS || rightGrabbed == otherGS)
                {
                    // check if we should ungrab them...?
                    if (feetDist > maxDistToUngrab)
                    {
                        if (canUngroup)
                        {
                            Ungrab(otherGS);

                        }
                    }
                }
                else if (leftGrabbed == null || rightGrabbed == null) // if either hand is free AND we are not already grabbing the other player...
                {
                    // check if we can grab their left/right. i.e. if our left AND their right is free...
                    if (feetDist < minDistToGrab)
                    {
                        // if our left is free and right is not grabbing same guy
                        if (leftGrabbed == null && rightGrabbed != otherGS)
                        {
                            var isOtherGSConnectedToMe = IsOtherGSConnectedToMe(otherGS, otherGS, true);
                            if (!isOtherGSConnectedToMe)
                            {
                                var otherFarRight = FindAvailable(false, otherGS);
                                if (otherFarRight != null)
                                {
                                    nearestGrabbableRight.Add(otherFarRight);
                                }
                            }
                        }

                        // if our right is free
                        if (rightGrabbed == null && leftGrabbed != otherGS)
                        {
                            var isOtherGSConnectedToMe = IsOtherGSConnectedToMe(otherGS, otherGS, false);
                            if (!isOtherGSConnectedToMe)
                            {
                                var otherFarLeft = FindAvailable(true, otherGS);
                                if (otherFarLeft != null)
                                {
                                    nearestGrabbableLeft.Add(otherFarLeft);
                                }
                            }
                        }
                    }

                }

            }
        }

        // grab for left, the nearest available right.
        if (leftGrabbed == null && nearestGrabbableRight.Count > 0)
        {
            var nearestRight = nearestGrabbableRight.Aggregate((p1, p2) => GetNearestDistToPlayer(p1, true) < GetNearestDistToPlayer(p2, true) ? p1 : p2);
            Grab(true, nearestRight);
        }

        // grab for right.
        if (rightGrabbed == null && nearestGrabbableLeft.Count > 0)
        {
            var nearestLeft = nearestGrabbableLeft.Aggregate((p1, p2) => GetNearestDistToPlayer(p1, false) < GetNearestDistToPlayer(p2, false) ? p1 : p2);
            Grab(false, nearestLeft);
        }
    }

    // is the other connected to me on their left?
    private bool IsOtherGSConnectedToMe(PlayerGroupStatus otherFirst, PlayerGroupStatus otherGS, bool otherLeft)
    {
        if (otherGS == this)
        {
            return true;
        }

        if ((otherLeft ? otherGS.leftGrabbed : otherGS.rightGrabbed) == this)
        {
            return true;
        }

        var pointer = otherLeft ? otherGS.leftGrabbed : otherGS.rightGrabbed;
        if (pointer == otherFirst)
            return false;

        if (pointer != this && pointer != null)
        {
            return IsOtherGSConnectedToMe(otherFirst, pointer, otherLeft);
        }
        return false;
    }

    private void RotateUpdate_ToGroup()
    {
        if (leftGrabbed != null || rightGrabbed != null)
        {
            Vector3 groupForward = Vector3.zero;
            // calculate group forward. not transform forward, but cross product of dudes in group' forward.
            if (false)
            {
                var count = 1;
                groupForward = transform.forward;
                // cycle left.
                var pointer = leftGrabbed;
                while (pointer != null)
                {
                    groupForward += pointer.transform.forward;
                    count++;
                    pointer = pointer.leftGrabbed;
                }
                // cycle right.
                pointer = rightGrabbed;
                while (pointer != null)
                {
                    groupForward += pointer.transform.forward;
                    count++;
                    pointer = pointer.rightGrabbed;
                }
            }

            // cross product of NEIGHBORS!!!
            var crossLeft = leftGrabbed != null ? (Vector3.Cross(transform.position - leftGrabbed.transform.position, Vector3.up)) : transform.forward;
            var crossRight = rightGrabbed != null ? (Vector3.Cross(rightGrabbed.transform.position - transform.position, Vector3.up)) : transform.forward;
            groupForward = (crossLeft + crossRight) / 2;

            // set rotation towards group forward... cannot rotate unless whole group rotates hahhahahhhhhhhhhh :(((
            var e = groupForward;
            e = Vector3.ProjectOnPlane(e, Vector3.up);
            if (groupForward == Vector3.zero)
            {
                return;
            }
            var bodyTurningSmoothness = groupRotateSmooth;
            transform.forward = Vector3.Lerp(transform.forward, e, bodyTurningSmoothness);

        }
    }

    float leftWeight;
    float rightWeight;
    public float smoothWeights = 0.02f;

    private void UpdateIkWeights()
    {
        leftWeight = Mathf.MoveTowards(leftWeight, leftGrabbed ? 1f : 0f, smoothWeights);
        ik.solver.leftHandEffector.positionWeight = leftWeight;
        rightWeight = Mathf.MoveTowards(rightWeight, rightGrabbed ? 1f : 0f, smoothWeights);
        ik.solver.rightHandEffector.positionWeight = rightWeight;
    }

    // finds the last available left/right grabbable player = the one that has the *left* (left/right) free hand.
    private PlayerGroupStatus FindAvailable(bool otherLeft, PlayerGroupStatus otherGS)
    {
        // if otherGS has a free hand on the otherLeft, return true.
        if ((otherLeft && otherGS.leftGrabbed == null)
            ||(!otherLeft && otherGS.rightGrabbed == null))
        {
            return otherGS;
        }

        // else, return the otherGS's otherLeft side and check again, until we find a null, or the initial one.
        else
        {
            // start checking in the order of otherLeft
            var init = otherGS;
            var pointer = otherLeft ? otherGS.leftGrabbed : otherGS.rightGrabbed;
            while (pointer != init)
            {
                // if the next element after pointer is null, return the pointer.
                var nextAfterPointer = (otherLeft ? pointer.leftGrabbed : pointer.rightGrabbed);
                if (nextAfterPointer == null)
                {
                    return pointer;
                }
                else
                {
                    pointer = nextAfterPointer;
                }
            }
            return null;
        }
    }

    // gets dist from the other player shoulder to this player's left/right hand.
    private float GetNearestDistToPlayer(PlayerGroupStatus otherPlayer, bool myLeft)
    {
        // find nearest hand to the middle bit of the player
        var otherMidShoulder = otherPlayer.playerContactPoints.middleShoulderBackGrab;
        var dist = 0f;
        if (myLeft)
        {
            var distLeft = otherMidShoulder.position - this.playerContactPoints.leftHand.position;
            dist = distLeft.magnitude;
        }
        else
        {
            var distRight = otherMidShoulder.position - this.playerContactPoints.rightHand.position;
            dist = distRight.magnitude;
        }
        return dist;
    }

    // grabs referenced player with left/right hand
    private void Grab(bool myLeft, PlayerGroupStatus nearest, bool reciprocate = true)
    {
        if (myLeft)
        {
            if (rightGrabbed != nearest)
            {
                leftGrabbed = nearest;
                // ik 
                ik.solver.leftHandEffector.target = leftGrabbed.playerContactPoints.middleShoulderBackGrab;

                if (reciprocate)
                {
                    // other player grab this
                    nearest.Grab(!myLeft, this, false);
                }

                if (OnGroup != null)
                {
                    OnGroup();
                }

            }
        }
        else
        {
            if (leftGrabbed != nearest)
            {
                rightGrabbed = nearest;
                ik.solver.rightHandEffector.target = rightGrabbed.playerContactPoints.middleShoulderBackGrab;

                if (reciprocate)
                {
                    // other player grab this
                    nearest.Grab(!myLeft, this, false);
                }

                if (OnGroup != null)
                {
                    OnGroup();
                }

            }
        }

    }

    private void Ungrab(PlayerGroupStatus other, bool reciprocate = true)
    {
        if (leftGrabbed == other)
        {
            // set IK shit
            if (reciprocate)
            {
                leftGrabbed.Ungrab(this, false);
            }
            ik.solver.leftHandEffector.target = null;
            leftGrabbed = null;

            if (OnUngroup != null)
            {
                OnUngroup();
            }

        }
        else if (rightGrabbed == other)
        {
            // set IK shit
            if (reciprocate)
            {
                rightGrabbed.Ungrab(this, false);
            }
            ik.solver.rightHandEffector.target = null;
            rightGrabbed = null;

            if (OnUngroup != null)
            {
                OnUngroup();
            }

        }
    }


}



public static class PlayerGroupStatusExtensions
{
    public static PlayerGroupStatus GetGroupStatus(this Player player)
    {
        return PlayerGroupStatus.Get(player);
    }
}