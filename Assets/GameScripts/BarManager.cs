using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class BarManager : MonoBehaviour
{
    private static BarManager _instance;
    public static BarManager instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<BarManager>();
            }
            return _instance;
        }
    }

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

    public Transform barPosition;

    private void Awake()
    {
        if (barPosition == null)
        {
            barPosition = transform;
        }
    }

}
