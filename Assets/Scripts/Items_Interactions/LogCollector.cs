using UnityEngine;
using UnityEngine.InputSystem;

public class LogCollector : MonoBehaviour
{
    [Header("Carry Settings")]
    public Transform carryPoint;
    public float pickupRange = 5f;

    [Header("Dock Settings")]
    public Transform dock; // Assign in Inspector
    public float dockRange = 3f;

    [Header("Dock Tracking")]
    public int logsPlaced = 0;

    private GameObject carriedLog = null;
    private PlayerControls controls;
    private Animator dockAnimator;

    private void Awake()
    {
        controls = new PlayerControls();
        controls.Gameplay.PickUp.performed += ctx => HandleInteract();

        if (dock != null)
        {
            // Try get Animator directly from dock
            dockAnimator = dock.GetComponent<Animator>();

            // If not there, try children
            if (dockAnimator == null)
                dockAnimator = dock.GetComponentInChildren<Animator>();

            if (dockAnimator != null)
                Debug.Log("[LogCollector] ✅ Dock Animator found on: " + dockAnimator.gameObject.name);
            else
                Debug.LogError("[LogCollector] ❌ Dock Animator NOT found! Make sure the dock has an Animator.");
        }
        else
        {
            Debug.LogError("[LogCollector] ❌ Dock reference missing in Inspector!");
        }
    }

    private void OnEnable() => controls.Gameplay.Enable();
    private void OnDisable() => controls.Gameplay.Disable();

    void HandleInteract()
    {
        if (carriedLog == null)
        {
            TryPickupLog();
            return;
        }

        float distanceToDock = Vector2.Distance(transform.position, dock.position);
        Debug.Log($"[LogCollector] Carrying log. Distance to dock: {distanceToDock}");

        if (distanceToDock <= dockRange)
        {
            Debug.Log("[LogCollector] In range -> placing log");
            PlaceLogAtDock();
        }
        else
        {
            Debug.Log("[LogCollector] Not near dock -> dropping log");
            DropLog();
        }
    }

    void TryPickupLog()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, pickupRange);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Log"))
            {
                PickUp(hit.gameObject);
                return;
            }
        }

        Debug.Log("[LogCollector] No logs nearby");
    }

    void PickUp(GameObject log)
    {
        carriedLog = log;
        carriedLog.transform.SetParent(carryPoint);
        carriedLog.transform.localPosition = Vector3.zero;

        Rigidbody2D rb = carriedLog.GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = true;

        Collider2D col = carriedLog.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Debug.Log("[LogCollector] Picked up log " + log.name);
    }

    void PlaceLogAtDock()
    {
        if (carriedLog == null) return;

        // Before destroying it, remove physics and colliders so nothing weird happens
        Rigidbody2D rb = carriedLog.GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = true;

        Collider2D col = carriedLog.GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        string logName = carriedLog.name;

        // Destroy the log object so it is completely removed
        Destroy(carriedLog);
        carriedLog = null;

        logsPlaced++;
        Debug.Log("[LogCollector] Log '" + logName + "' placed at dock and destroyed. Total logs placed: " + logsPlaced);

        // Update dock animation if you have one
        if (dockAnimator != null)
        {
            dockAnimator.SetInteger("PlanksPlaced", logsPlaced);
            Debug.Log("[LogCollector] Dock animation updated. PlanksPlaced = " + logsPlaced);
        }
    }



    void DropLog()
    {
        carriedLog.transform.SetParent(null);
        carriedLog.transform.position = transform.position;

        Rigidbody2D rb = carriedLog.GetComponent<Rigidbody2D>();
        if (rb != null) rb.isKinematic = false;

        Collider2D col = carriedLog.GetComponent<Collider2D>();
        if (col != null) col.enabled = true;

        Debug.Log("[LogCollector] Log dropped: " + carriedLog.name);
        carriedLog = null;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);

        if (dock != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(dock.position, dockRange);
        }
    }
}
