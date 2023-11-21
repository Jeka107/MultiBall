using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform playerTramsform;

    void Update()
    {
        if (playerTramsform!=null)
        {
            if (playerTramsform.position.z > transform.position.z)
            {
                transform.position = new Vector3(playerTramsform.position.x,
                    playerTramsform.position.y+9,
                    playerTramsform.position.z-20);
            }
        }
    }
}
