using UnityEngine;

public class Destroyer : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag=="Ball")
        {
            Destroy(other.gameObject);
        }
    }
}
