using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class CircleController : MonoBehaviour
{
    Rigidbody2D circleRigidbody;

    void Start()
    {
        circleRigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Fall()
    {
        circleRigidbody.simulated = true;
    }
}
