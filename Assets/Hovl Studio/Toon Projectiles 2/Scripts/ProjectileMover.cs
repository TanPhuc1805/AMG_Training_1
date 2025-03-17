using UnityEngine;

public class ProjectileMover : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private float drag;
    private bool hasCollided = false;

    [Header("Effects")]
    public GameObject hitEffect;
    public GameObject muzzleFlash;
    public GameObject[] Detached;

    void Start()
    {
        rb = GetComponent<Rigidbody>();

        // Hiển thị hiệu ứng bắn
        if (muzzleFlash != null)
        {
            GameObject flashInstance = Instantiate(muzzleFlash, transform.position, Quaternion.identity);
            flashInstance.transform.forward = transform.forward;

            if (flashInstance.TryGetComponent<ParticleSystem>(out ParticleSystem flashPs))
                Destroy(flashInstance, flashPs.main.duration);
            else if (flashInstance.transform.childCount > 0 &&
                     flashInstance.transform.GetChild(0).TryGetComponent<ParticleSystem>(out ParticleSystem flashPsParts))
                Destroy(flashInstance, flashPsParts.main.duration);
        }

        rb.useGravity = true; // Sử dụng trọng lực chuẩn của Unity
    }

    void FixedUpdate()
    {
        if (hasCollided) return;

        // Áp dụng công thức giống TrajectoryPredictor
        velocity += Physics.gravity * Time.fixedDeltaTime;
        velocity *= Mathf.Clamp01(1f - drag * Time.fixedDeltaTime);
        rb.velocity = velocity;
    }

    public void InitializeProjectile(Vector3 initialVelocity, float drag)
    {
        this.velocity = initialVelocity;
        this.drag = drag;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (hasCollided) return;
        hasCollided = true;

        rb.velocity = Vector3.zero;
        rb.constraints = RigidbodyConstraints.FreezeAll;

        ContactPoint contact = collision.contacts[0];
        Vector3 pos = contact.point;
        Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);

        if (hitEffect != null)
        {
            GameObject hitInstance = Instantiate(hitEffect, pos, rot);
            if (hitInstance.TryGetComponent<ParticleSystem>(out ParticleSystem hitPs))
                Destroy(hitInstance, hitPs.main.duration);
            else if (hitInstance.transform.childCount > 0 &&
                     hitInstance.transform.GetChild(0).TryGetComponent<ParticleSystem>(out ParticleSystem hitPsParts))
                Destroy(hitInstance, hitPsParts.main.duration);
        }

        foreach (GameObject detachedPrefab in Detached)
        {
            if (detachedPrefab != null)
                detachedPrefab.transform.parent = null;
        }

        Destroy(gameObject);
    }
}
