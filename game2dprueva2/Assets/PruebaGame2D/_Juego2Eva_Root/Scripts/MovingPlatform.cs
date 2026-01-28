using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    [Header("Waypoints & Movement Configuration")]
    [SerializeField] Transform[] points; 
    [SerializeField] int startingPoint; 
    [SerializeField] float speed; 

    int i; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        transform.position = points[startingPoint].position;
    }

   
    void Update()
    {
        PlatformMovement();
    }

    void PlatformMovement()
    {
        if (Vector2.Distance(transform.position, points[i].position) < 0.02f)
        {
            i++; 
            if (i == points.Length)
            {
                i = 0; 
            }
        }
        
        transform.position = Vector2.MoveTowards(transform.position, points[i].position, speed * Time.deltaTime);

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            if (transform.position.y < collision.transform.position.y) 
            {
                collision.transform.SetParent(transform);
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
    }
}
