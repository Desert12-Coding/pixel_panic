using UnityEngine;

public class NewMonoBehaviourScript : customDragCoefficient
private int drag value = 0.5f; 
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void FixedUpdate()
{
    // Apply a force opposite to the velocity, scaled by a custom drag coefficient
    float customDragCoefficient = 0.5f; // Adjust this value as needed
    GetComponent<Rigidbody>().AddForce(-GetComponent<Rigidbody>().linearVelocity * customDragCoefficient, ForceMode.Acceleration);
}
}
