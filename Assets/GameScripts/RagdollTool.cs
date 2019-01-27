using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RagdollTool : MonoBehaviour
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
    private RagdollUtility _ragdoll;
    public RagdollUtility ragdoll
    {
        get
        {
            if (_ragdoll == null)
            {
                _ragdoll = GetComponentInChildren<RagdollUtility>();
            }
            return _ragdoll;
        }
    }

    [SerializeField]
    private Rigidbody _rb;
    public Rigidbody rb
    {
        get
        {
            if (_rb == null)
            {
                _rb = GetComponent<Rigidbody>();
            }
            return _rb;
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

    [SerializeField]
    private PlayerGroupStatus _groups;

    public bool shouldRevive = true;

    public PlayerGroupStatus groups
    {
        get
        {
            if (_groups == null)
            {
                _groups = GetComponent<PlayerGroupStatus>();
            }
            return _groups;
        }
    }
    
    public bool isRagdoll { get { return ragdoll.isRagdoll; } }

    public  event System.Action<float> OnRagdoledWithDelay;

    public void Ragdoll(bool active, float forceTowardsMiddle, float moveToCenterDist, float delayToRevive)
    {
        if (active)
        {
            ragdoll.EnableRagdoll();
            var toCollider = LevelCollider.instance.collider.transform.position - player.transform.position;

            MoveTowardsInsideSoWeDontExplodeAgain(toCollider, moveToCenterDist);


            if (shouldRevive)
            {
                if (!LevelCollider.instance.collider.bounds.Contains(ik.references.pelvis.transform.position))
                {
                    ik.references.head.GetComponentInChildren<Rigidbody>().AddForce(toCollider.normalized * forceTowardsMiddle, ForceMode.Impulse);
                }

                if (rb != null)
                {
                    rb.isKinematic = false;
                }
            }

            if (OnRagdoledWithDelay != null)
            {
                OnRagdoledWithDelay(delayToRevive);
            }

        }
        else
        {
            Move2Pelvis(Vector3.zero);
            ragdoll.DisableRagdoll();
            MoveToZeroOnY();
        }
    }

    private void MoveTowardsInsideSoWeDontExplodeAgain(Vector3 toCollider, float moveToCenterDist = 0)
    {
        var moveeee = -toCollider;
        moveeee.y = 0;
        moveeee.Normalize();
        moveeee *= moveToCenterDist;
        Move2Pelvis(moveeee);
        MoveToZeroOnY();
    }

    private void MoveToZeroOnY()
    {
        var pos = ik.references.root.position;
        pos.y = 0;
        ik.references.root.position = pos;
    }

    private void Move2Pelvis(Vector3 offset)
    {
        var pelvis = ik.references.pelvis;
        var root = ik.references.root;

        // Move the root of the character to where the pelvis is without moving the ragdoll
        Vector3 toPelvis = pelvis.position - root.position + offset;
        root.position += toPelvis;
        pelvis.transform.position -= toPelvis;
    }
}