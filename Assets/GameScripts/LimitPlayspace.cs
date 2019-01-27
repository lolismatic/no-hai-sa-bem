using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class LimitPlayspace : MonoBehaviour
{

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

    public float moveToCenterDistOnOutOfPlayspace = 2f;

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
        if (!LevelCollider.instance.collider.bounds.Contains(ik.references.pelvis.transform.position))
        {
            ragdollTool.Ragdoll(true, addForceWhenOutside, moveToCenterDistOnOutOfPlayspace);

            reviveTimer = Time.time + reviveDelay;
        }

        if (ragdoll.isRagdoll)
        {
            if (Time.time > reviveTimer)
            {
                ragdollTool.Ragdoll(false, 0, 0f);
                var containsPelvis = LevelCollider.instance.collider.bounds.Contains(ik.references.pelvis.transform.position);
                var containsRoot = LevelCollider.instance.collider.bounds.Contains(transform.position);
                if (!containsPelvis || !containsRoot)
                {
                    BarManager.instance.RespawnAt(ragdollTool.groups, BarManager.instance.restartPosition);
                    //transform.position = LevelCollider.instance.transform.position + (transform.position - LevelCollider.instance.transform.position).normalized * 10;
                }
            }
        }
    }

}