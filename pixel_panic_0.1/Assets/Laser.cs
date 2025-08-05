using UnityEngine;

public class laser : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
       transform.position += (Vector3)(new Vector2(1f, 0) * Time.deltaTime);
    }
}
