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
        var delta = anim.transform.position - transform.position;
        anim.transform.localPosition = Vector3.zero;
        transform.position += delta;
    }
}