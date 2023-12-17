using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class GrapplingHook : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LineRenderer lr = null;
    [SerializeField] private Transform gunTip = null;

    [Header("Grappling Parameters")]
    [SerializeField] private float maxGrappleDistance = 15;
    [SerializeField] private float grappleDuration = 1;
    [SerializeField] private float overShootYAxis = 1;
    [SerializeField] private LayerMask whatIsGrappleable;

    [Header("Cooldown")]
    [SerializeField] private float grapplingCooldown;

    [Header("Camera Effects")]
    [SerializeField] float fovChange = 5f;


    PlayerPhysics player = null;
    private Vector3 grapplePoint;
    private float grapplingCooldownTimer;
    private bool grappling = false;
    private Transform cameraTransform;
    private WaitForSeconds grappleDelay;

    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerPhysics>();
        cameraTransform = GetComponentInChildren<Camera>().transform;
        grappleDelay = new WaitForSeconds(grappleDuration);
    }


    public void StartGrapple(InputAction.CallbackContext context) {
        if (context.phase != InputActionPhase.Performed)
            return;

        if (!player.CanGrapple)
            return;

        if (grappling) {
            Debug.Log("Already grappling");
            return;
        }

        if (grapplingCooldownTimer > 0) {
            return;
        }

        //grappling = true;
        player.ToggleFreeze(true);
        Debug.DrawRay(cameraTransform.position, cameraTransform.forward * maxGrappleDistance, Color.green, 2);
        if (Physics.Raycast(cameraTransform.position, cameraTransform.forward, out RaycastHit hit, maxGrappleDistance)) {
            Debug.Log("hit");
            grapplePoint = hit.point;
            ExectueGrapple(grappleDuration);
            // Invoke grapple
        }
        else {
            grapplePoint = cameraTransform.position + cameraTransform.forward * maxGrappleDistance;
            //Grapple(0, true);
            ExectueGrapple(grappleDuration, false);
            // invoke stop grapple
        }
    }

    private void ExectueGrapple(float grappleTime, bool hit = true) {
        grappling = true;
        lr.enabled = true;
        player.AdjustFOV(-fovChange);
        if (hit) {
            Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

            float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
            float highestPointOnArc = grapplePointRelativeYPos + overShootYAxis;

            if (grapplePointRelativeYPos < 0) highestPointOnArc = overShootYAxis;

            player.GrappleJump(grapplePoint, highestPointOnArc);
        }
        else {
            player.ToggleFreeze(false);
        }

        StartCoroutine(Coroutine_Grapple(grappleTime));
    }

    private IEnumerator Coroutine_Grapple(float grappleTime) {

        //yield return grappleDelay;
        Debug.Log("Starting grapple for " + grappleTime + " Seconds");
        while (grappleTime > 0) {
            grappleTime-= Time.deltaTime;
            yield return null;
        }
        grappling = false;
       // lr.enabled= false;
        player.AdjustFOV(0);
        player.ToggleFreeze(false);
    }

    public Vector3 GetGrapplePoint() {
        return grapplePoint;
    }

    public Vector3 GrappleGunTipPos { get => gunTip.position; }
    public bool IsGrappling { get => grappling; }
}
