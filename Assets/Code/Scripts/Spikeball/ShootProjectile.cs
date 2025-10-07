using UnityEngine;
using UnityEngine.InputSystem;

public class ShootProjectile : MonoBehaviour
{
    [SerializeField] private float distance_offset;
    [SerializeField] private float projectile_force;
    [SerializeField] private SpikedProjectile projectile;


    private void Start()
    {
        EventHandler.RestartGame += restart;
    }

    public void shoot(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            shoot();
        }
    }

    public void shoot()
    {
        if (!projectile.gameObject.activeSelf)
        {
            projectile.transform.position = transform.position + transform.forward * distance_offset;
        }
        projectile.add_force(transform.forward, projectile_force);
    }

    public void restart(GameObject target)
    {
        if (gameObject == target)
        {
            projectile.gameObject.SetActive(false);
        }
    }

    public SpikedProjectile get_projectile()
    {
        return projectile;
    }

    public float get_time_left()
    {
        return projectile.get_time_left();
    }
}
