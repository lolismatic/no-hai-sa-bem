using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObstacolRagdoll : MonoBehaviour
{
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

    [Header("Rare update")]
    public float updateRareDelay = 0.5f;
    private float updateRareTimer;

    public float delayToRevive = 7f;


    private void Update()
    {
        if (Time.time > updateRareTimer)
        {
            updateRareTimer = Time.time + updateRareDelay * Random.Range(1, 1.2f);
            UpdateRare();
        }
    }

    private void UpdateRare()
    {
        for (int i = 0; i < Player.allPlayers.Count; i++)
        {
            var p = Player.allPlayers[i];

            if (!p.ragdoll.isRagdoll)
            {
                if (collider.bounds.Contains(p.ragdoll.ik.references.head.position))
                {
                    p.ragdoll.Ragdoll(true, 10f, 0, delayToRevive);
                }
            }
        }
    }
}