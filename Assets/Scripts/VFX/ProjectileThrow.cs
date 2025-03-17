using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(TrajectoryPredictor))]
public class ProjectileThrow : MonoBehaviour
{
    private TrajectoryPredictor trajectoryPredictor;

    [Header("Projectile Settings")]
    [SerializeField] private Rigidbody[] projectilePrefabs; // Danh sách 15 loại đạn
    [SerializeField, Range(0.0f, 50.0f)] private float force = 20f;
    [SerializeField] private Transform StartPosition; // Vị trí bắn đạn
    [SerializeField] private float dragMin = 0.0f;
    [SerializeField] private float dragMax = 0.2f;

    [Header("Auto Fire Settings")]
    [SerializeField] private float fireRate = 0.2f; // Bắn mỗi 0.2 giây
    private bool isFiring = false;
    private Coroutine fireCoroutine;

    public InputAction fire; // Hành động bắn từ Input System

    private void OnEnable()
    {
        trajectoryPredictor = GetComponent<TrajectoryPredictor>();

        if (StartPosition == null)
            StartPosition = transform;

        fire.Enable();
        fire.started += StartFiring;
        fire.canceled += StopFiring;
    }

    private void Update()
    {
        Predict();
    }

    private void Predict()
    {
        trajectoryPredictor.PredictTrajectory(ProjectileData());
    }

    private ProjectileProperties ProjectileData()
    {
        Rigidbody selectedProjectile = GetRandomProjectile();
        if (selectedProjectile == null)
        {
            Debug.LogError("⚠ Không tìm thấy Prefab đạn hợp lệ!", this);
            return new ProjectileProperties();
        }

        float randomDrag = Random.Range(dragMin, dragMax);

        return new ProjectileProperties
        {
            direction = StartPosition.forward,
            initialPosition = StartPosition.position,
            initialSpeed = force,
            mass = selectedProjectile.mass,
            drag = randomDrag
        };
    }

    private void StartFiring(InputAction.CallbackContext ctx)
    {
        if (!isFiring)
        {
            isFiring = true;
            fireCoroutine = StartCoroutine(AutoFire());
        }
    }

    private void StopFiring(InputAction.CallbackContext ctx)
    {
        if (isFiring)
        {
            isFiring = false;
            if (fireCoroutine != null)
            {
                StopCoroutine(fireCoroutine);
            }
        }
    }

    private IEnumerator AutoFire()
    {
        while (isFiring)
        {
            ThrowObject();
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void ThrowObject()
    {
        Rigidbody selectedProjectile = GetRandomProjectile();
        if (selectedProjectile == null)
        {
            Debug.LogError("⚠ Không tìm thấy Prefab đạn hợp lệ!", this);
            return;
        }

        // Tạo đạn từ prefab
        Rigidbody thrownObject = Instantiate(selectedProjectile, StartPosition.position, StartPosition.rotation);

        // Tạo lực cản ngẫu nhiên cho từng viên đạn
        float randomDrag = Random.Range(dragMin, dragMax);

        // Kiểm tra nếu có `ProjectileMover`, gán giá trị drag vào
        if (thrownObject.TryGetComponent<ProjectileMover>(out ProjectileMover projectileMover))
        {
            projectileMover.InitializeProjectile(StartPosition.forward * force, randomDrag);
        }
        else
        {
            thrownObject.velocity = StartPosition.forward * force;
        }
    }

    private Rigidbody GetRandomProjectile()
    {
        if (projectilePrefabs.Length == 0)
            return null;

        int randomIndex = Random.Range(0, projectilePrefabs.Length);
        return projectilePrefabs[randomIndex];
    }
}
