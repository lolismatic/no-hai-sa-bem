using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BarManager : MonoBehaviour
{
    private static BarManager _instance;
    public static BarManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BarManager>();
            }
            return _instance;
        }
    }

    [SerializeField]
    private Collider _collider;
    public Collider collider
    {
        get
        {
            if (_collider == null)
            {
                _collider = GetComponent<Collider>();
            }
            return _collider;
        }
    }

    public Transform barPosition;

    public Transform restartPosition;
    public float distOnRestart = 1f;

    private void Awake()
    {
        if (barPosition == null)
        {
            barPosition = transform;
        }
    }

    public void RespawnAt(PlayerGroupStatus groupStatus, Transform restartPosition)
    {
        // find leftmost
        var leftMost = groupStatus;
        var init = leftMost;
        var montecarlo = 100;
        while (leftMost.leftGrabbed != null)
        {
            leftMost = leftMost.leftGrabbed;

            if (montecarlo-- < 0)
                return;
        }

        var count = 0;
        var counter = leftMost;
        montecarlo = 100;
        while (counter != null)
        {
            count++;
            counter = counter.rightGrabbed;
            if (montecarlo-- < 0)
                return;
        }

        var initPos = restartPosition.position + Vector3.left * count / 2 * distOnRestart;
        leftMost.transform.position = initPos;
        leftMost.transform.forward = restartPosition.forward;
        var next = leftMost.rightGrabbed;
        var nextPos = initPos;
        montecarlo = 100;
        while (next != null)
        {
            nextPos += Vector3.right * distOnRestart;
            next.transform.position = nextPos;
            next.transform.forward = restartPosition.forward;
            next = next.rightGrabbed;
            if (montecarlo-- < 0)
                return;
        }


    }
}
