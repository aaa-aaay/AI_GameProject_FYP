using UnityEngine;
using UnityEngine.InputSystem;

public class ShootInterface : MonoBehaviour
{
    [SerializeField] private Dodgeball dodgeball;

    private void Start()
    {

    }

    public void OnClick(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (dodgeball != null)
            {
                dodgeball.ShootForward(transform);
                dodgeball = null;
            }
        }
    }

    public void Shoot()
    {
        if (dodgeball != null)
        {
            dodgeball.ShootForward(transform);
            dodgeball = null;
        }
    }

    public void SetBall(Dodgeball new_dodgeball)
    {
        dodgeball = new_dodgeball;
    }

    public void SetBall(GameObject player, Dodgeball new_dodgeball)
    {
        if (player == gameObject)
        {
            dodgeball = new_dodgeball;
        }
    }

    public bool CanShoot()
    {
        return dodgeball != null;
    }
}
