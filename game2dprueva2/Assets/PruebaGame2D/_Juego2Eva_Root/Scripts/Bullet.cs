using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Bullet Configuration")]
    [SerializeField] float speed;
    public bool isFacingRight;

    SpriteRenderer bulletSprite;

    private void Start()
    {
        bulletSprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        BulletMove();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        gameObject.SetActive(false);
    }

    void BulletMove()
    {
        if (isFacingRight)
        {
            
            transform.Translate(Vector3.right * speed * Time.deltaTime);
        }
        else 
        {
            bulletSprite.flipX = true;
            transform.Translate(Vector3.left * speed * Time.deltaTime);
        }
        

    }
}
