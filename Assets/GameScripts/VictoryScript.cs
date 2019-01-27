using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class VictoryScript : MonoBehaviour
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


    [Header("VICCC")]
    public float victoryForce = 100f;

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
                if (p.ragdoll.groups.leftGrabbed == null && p.ragdoll.groups.rightGrabbed == null)
                {
                    // p is alone!!!
                    p.ragdoll.Ragdoll(true, 50, 0);

                    StartCoroutine(pTween.Wait(3f, () =>
                    {
                        p.ragdoll.Ragdoll(false, 10f);
                    }));
                }
                else
                {
                    var pointerLeft = p.GetGroupStatus();
                    var montecarlo = 100;
                    while (pointerLeft != null)
                    {
                        VictoryForPlayer(pointerLeft.player);
                        pointerLeft = pointerLeft.leftGrabbed;
                        if (montecarlo-- < 0)
                        {
                            break;
                        }
                    }
                    var pointerRight = p.GetGroupStatus().rightGrabbed;
                    montecarlo = 100;
                    while (pointerRight != null)
                    {
                        VictoryForPlayer(pointerRight.player);
                        pointerRight = pointerRight.rightGrabbed;
                        if (montecarlo-- < 0)
                        {
                            break;
                        }
                    }
                }
            }
        }
    }

    private void VictoryForPlayer(Player p)
    {
        if (!p.ragdoll.isRagdoll)
        {
            // VICTORY FOR THIS PLAYER
            p.ragdoll.Ragdoll(true, victoryForce, 0);

            p.ActivateVictoryMesh();

            p.ragdoll.shouldRevive = false;

        }
    }
}