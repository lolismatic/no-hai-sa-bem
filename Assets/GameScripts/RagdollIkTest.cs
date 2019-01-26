using RootMotion.FinalIK;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class RagdollIkTest : MonoBehaviour
{
    [SerializeField]
    private RagdollUtility _ragdoll;
    public RagdollUtility ragdoll
    {
        get
        {
            if (_ragdoll == null)
            {
                _ragdoll = GetComponent<RagdollUtility>();
            }
            return _ragdoll;
        }
    }

    [DebugButton]
    void SetRagdoll(bool active)
    {
        if (active)
            ragdoll.EnableRagdoll();
        else
            ragdoll.DisableRagdoll();
    }
}