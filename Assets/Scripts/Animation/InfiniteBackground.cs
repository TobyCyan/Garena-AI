using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private Transform backgroundA;
    [SerializeField] private Transform backgroundB;
    public float scrollSpeed = 0.5f;

    private float backgroundWidth;


    void Start()
    {
        SpriteRenderer sr = backgroundA.GetComponent<SpriteRenderer>();
        backgroundWidth = sr.bounds.size.x;
    }

    void Update()
    {
        Vector3 movement = Vector3.left * scrollSpeed * Time.deltaTime;

        backgroundA.position += movement;
        backgroundB.position += movement;

        if (backgroundA.position.x <= -backgroundWidth)
        {
            backgroundA.position = new Vector3(
                backgroundB.position.x + backgroundWidth,
                backgroundA.position.y,
                backgroundA.position.z
            );
        }

        if (backgroundB.position.x <= -backgroundWidth)
        {
            backgroundB.position = new Vector3(
                backgroundA.position.x + backgroundWidth,
                backgroundB.position.y,
                backgroundB.position.z
            );
        }
    }
}

