using UnityEngine;

public class CustomDrag : MonoBehaviour
{
    [SerializeField] [Range(0, 1)] private float dragCoefficient = 0.5f;
    [SerializeField] private bool relativeToMass = true;
    
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("No Rigidbody found on " + gameObject.name);
            enabled = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;
        
        // Calculate drag force
        Vector3 dragForce = -rb.linearVelocity * dragCoefficient;
        
        // Optionally scale by mass for more consistent behavior
        if (relativeToMass) dragForce *= rb.mass;
        
        // Apply the force
        rb.AddForce(dragForce, ForceMode.Force);
    }
}