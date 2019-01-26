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

    public float swarmingForce = 0.1f;
    public float minDist = 0.3f;

    private void Update()
    {
        Vector3 away = Vector3.zero;
        var average = 0;

        if (group.leftGrabbed != null)
        {
            var towardsLEft = group.leftGrabbed.transform.position - transform.position;
            towardsLEft.y = 0;
            if (towardsLEft.sqrMagnitude < minDist * minDist)
            {
                away += towardsLEft;
                average += 1;
            }
        }
        if (group.rightGrabbed != null)
        {
            var toRight = group.rightGrabbed.transform.position - transform.position;
            toRight.y = 0;
            if (toRight.sqrMagnitude < minDist * minDist)
            {
                away += toRight;
                average += 1;
            }
        }

        transform.position += away / Mathf.Max(1, average) * swarmingForce * Time.deltaTime;

    }
}