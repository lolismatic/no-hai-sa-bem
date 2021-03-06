using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Player : MonoBehaviour
{
    public static List<Player> allPlayers = new List<Player>();

    [SerializeField]
    private RagdollTool _ragdoll;
    public RagdollTool ragdoll
    {
        get
        {
            if (_ragdoll == null)
            {
                _ragdoll = GetComponent<RagdollTool>();
            }
            return _ragdoll;
        }
    }

    [Header("Core player settings")]
    public int playerId = 0;

    private void OnEnable()
    {
        allPlayers.Add(this);
    }

    private void OnDisable()
    {
        allPlayers.Remove(this);
    }

    public GameObject victoryMesh;

    public void ActivateVictoryMesh()
    {
        if (victoryMesh != null)
        {
            victoryMesh.SetActive(true);
            victoryMesh.transform.SetParent(null);
            victoryMesh.transform.rotation = Quaternion.identity;
            victoryMesh.GetComponent<VictoryFollowObj>().target = ragdoll.ik.references.head;
        }

    }
}