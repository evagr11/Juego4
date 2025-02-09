using UnityEngine;

public class ContantRotationReset : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.identity;
    }
}
