using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CircleController : MonoBehaviour
{
    Rigidbody2D rigidbody;

    void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fall()
    {
        rigidbody.simulated = true;
    }
}
