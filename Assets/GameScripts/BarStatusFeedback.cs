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

    private void OnEnable()
    {
        barThing.OnStateChange += BarThing_OnStateChange;
    }


    private void OnDisable()
    {
        barThing.OnStateChange -= BarThing_OnStateChange;
    }

    private void BarThing_OnStateChange(GroupToBarThing.GroupStates newState)
    {
        home.SetActive(newState == GroupToBarThing.GroupStates.FromBar);
        alone.SetActive(newState == GroupToBarThing.GroupStates.LostAndAlone);
        var e = aloneParts.emission;
        e.enabled = GroupToBarThing.GroupStates.LostAndAlone == newState;
        toBar.SetActive(newState == GroupToBarThing.GroupStates.TowardsBar);

    }

}
