using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class BarStatusFeedback : MonoBehaviour
{
    [SerializeField]
    private GroupToBarThing _barThing;
    public GroupToBarThing barThing
    {
        get
        {
            if (_barThing == null)
            {
                _barThing = GetComponent<GroupToBarThing>();
            }
            return _barThing;
        }
    }

    [Header("Feedback objects")]

    public GameObject toBar; // bottles
    public ParticleSystem toBarParts;

    public GameObject alone;
    public ParticleSystem aloneParts;

    public GameObject home;
    public ParticleSystem homeParts;

    private void OnEnable()
    {
        barThing.OnStateChange += BarThing_OnStateChange;
        StartCoroutine(StateChangeUpdate());
    }

    private void OnDisable()
    {
        barThing.OnStateChange -= BarThing_OnStateChange;
    }

    IEnumerator StateChangeUpdate()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);
            BarThing_OnStateChange(barThing.state);
        }
    }
    
    private void BarThing_OnStateChange(GroupToBarThing.GroupStates newState)
    {
        if (home != null)
            home.SetActive(newState == GroupToBarThing.GroupStates.FromBar);
        if (homeParts != null)
        {
            var g = homeParts.emission;
            g.rateOverDistanceMultiplier = GroupToBarThing.GroupStates.FromBar == newState ? 1 : 0;
            g.rateOverTimeMultiplier = GroupToBarThing.GroupStates.FromBar == newState ? 1 : 0;
        }

        if (alone != null)
            alone.SetActive(newState == GroupToBarThing.GroupStates.LostAndAlone);
        if (aloneParts != null)
        {
            var e = aloneParts.emission;
            e.rateOverDistanceMultiplier = GroupToBarThing.GroupStates.LostAndAlone == newState ? 1 : 0;
            e.rateOverTimeMultiplier = GroupToBarThing.GroupStates.LostAndAlone == newState ? 1 : 0;
        }

        if (toBar != null)
            toBar.SetActive(newState == GroupToBarThing.GroupStates.TowardsBar);
        if (toBarParts != null)
        {
            var f = toBarParts.emission;
            f.rateOverDistanceMultiplier = GroupToBarThing.GroupStates.TowardsBar == newState ? 1 : 0;
            f.rateOverTimeMultiplier = GroupToBarThing.GroupStates.TowardsBar == newState ? 1 : 0;
        }
    }

}
