using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Gardu : MonoBehaviour
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


    public float updateRareDelay = 0.5f;
    private float updateRareTimer;

    public Transform restartPos;

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

            if (collider.bounds.Contains(p.transform.position))
            {
                if (!p.ragdoll.isRagdoll)
                {
                    BarManager.instance.RespawnAt(p.GetGroupStatus(), restartPos);

                }
            }
        }
    }
}