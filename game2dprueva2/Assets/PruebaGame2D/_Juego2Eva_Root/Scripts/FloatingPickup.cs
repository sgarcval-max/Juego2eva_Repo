using UnityEngine;

public class FloatingPickup : MonoBehaviour
{
    public float rotateSpeed = 90f;
    public float floatSpeed = 2f;
    public float floatAmount = 0.25f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        transform.Rotate(0, 0, rotateSpeed * Time.unscaledDeltaTime);

        float yOffset = Mathf.Sin(Time.unscaledTime * floatSpeed) * floatAmount;
        transform.position = startPos + Vector3.up * yOffset;
    }
}