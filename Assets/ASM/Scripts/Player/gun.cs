using UnityEngine;

public class Bullet : MonoBehaviour
{
    public float lifetime = 1f; 

    void Start()
    {
       
        Destroy(gameObject, lifetime);
    }
}
