using UnityEngine;

public class laser : MonoBehaviour
{
    [SerializeField] private float MoveSpeed = 1f;
    
    // Update is called once per frame
    void Update()
    {
       transform.position += (Vector3)(new Vector2(1f, 0) * MoveSpeed * Time.deltaTime);
    }
}