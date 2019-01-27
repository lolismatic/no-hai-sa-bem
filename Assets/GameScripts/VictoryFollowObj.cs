using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class VictoryFollowObj : MonoBehaviour
{
    public Transform target;

    public Vector3 offsetWorld = new Vector3(0, 2f, 0);

    private void Update()
    {
        transform.position = target.position + offsetWorld;
    }
}
