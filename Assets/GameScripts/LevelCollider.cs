using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public class LevelCollider : MonoBehaviour
{
    private static LevelCollider _instance;
    public static LevelCollider instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<LevelCollider>();
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


}

