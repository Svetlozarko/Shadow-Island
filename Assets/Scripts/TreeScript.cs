using UnityEngine;

public class Tree : MonoBehaviour
{
    public float holdTimeToChop = 4f;    // How long to hold to chop tree
    private float holdTimer = 0f;        // Timer for holding
    private bool isHolding = false;      // Is the player currently holding?

    public GameObject woodDropPrefab;    // Assign log prefab
    public Transform dropPoint;          // Where the logs appear (optional)

    void Update()
    {
        // If holding, increase timer
        if (isHolding)
        {
            holdTimer += Time.deltaTime;

            // If timer reaches chop time → chop tree
            if (holdTimer >= holdTimeToChop)
            {
                ChopTree();
            }
        }
        else
        {
            // Reset if player stops holding
            holdTimer = 0f;
        }
    }

    void OnMouseDown()
    {
        // Start holding when mouse button is pressed
        isHolding = true;
    }

    void OnMouseUp()
    {
        // Stop holding when mouse button is released
        isHolding = false;
    }

    void ChopTree()
    {
        // Drop wood logs
        if (woodDropPrefab != null)
        {
            Instantiate(woodDropPrefab, dropPoint ? dropPoint.position : transform.position, Quaternion.identity);
        }

        Debug.Log("Tree chopped after holding!");

        Destroy(gameObject); // Remove the tree
    }
}
