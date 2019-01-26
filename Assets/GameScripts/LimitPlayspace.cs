using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LimitPlayspace : MonoBehaviour
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

    public float addForceWhenOutside = 100f;
    public float updateRareDelay = 0.5f;
    private float updateRareTimer;


    public float reviveDelay = 3f;
    private float reviveTimer;

    private void Update()
    {
        if (Time.time > updateRareTimer)
        {
            updateRareTimer = Time.time + updateRareDelay;
            UpdateRare();
        }
    }

    private void UpdateRare()
    {
        if (!LevelCollider.instance.collider.bounds.Contains(player.transform.position))
        {
            ragdoll.EnableRagdoll();
            var toCollider = LevelCollider.instance.collider.transform.position - player.transform.position;

            MoveTowardsInsideSoWeDontExplodeAgain(toCollider);


            ik.references.head.GetComponentInChildren<Rigidbody>().AddForce(toCollider.normalized * addForceWhenOutside, ForceMode.Impulse);
            if (rb != null)
            {
                rb.isKinematic = false;
            }

            reviveTimer = Time.time + reviveDelay;
        }

        if (ragdoll.isRagdoll)
        {
            Move2Pelvis(Vector3.zero);
            if (Time.time > reviveTimer)
            {
                ragdoll.DisableRagdoll();
                MoveToZeroOnY();
            }
        }
    }

    private void MoveToZeroOnY()
    {
        var pos = ik.references.root.position;
        pos.y = 0;
        ik.references.root.position = pos;
    }

    private void MoveTowardsInsideSoWeDontExplodeAgain(Vector3 toCollider)
    {
        var moveeee = -toCollider;
        moveeee.Normalize();
        moveeee.y = 0;
        Move2Pelvis(moveeee);
        MoveToZeroOnY();
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