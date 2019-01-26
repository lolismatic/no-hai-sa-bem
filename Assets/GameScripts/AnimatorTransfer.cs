using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class AnimatorTransfer : MonoBehaviour
{
    [Header("Transfers root motion from animator in child")]
    [SerializeField]
    private Animator _anim;
    public Animator anim
    {
        get
        {
            if (_anim == null)
            {
                _anim = GetComponentInChildren<Animator>();
            }
            return _anim;
        }
    }

    public bool fixedUpdate = false;

    void Update()
    {
        if (!fixedUpdate)
        {
            Update_Transfer();
        }
    }

    private void FixedUpdate()
    {
        if (fixedUpdate)
        {
            Update_Transfer();
        }
    }

    private void Update_Transfer()
    {
        // because child is parented, it will also be moved.
        transform.position = anim.rootPosition;
        // move child back to localpos zero.
        anim.rootPosition = transform.position;
    }
}