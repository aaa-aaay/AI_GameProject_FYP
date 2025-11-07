using UnityEngine;
using UnityEngine.VFX;

public class CatchItem : MonoBehaviour
{
    //[SerializeField] private VisualEffect effect;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //effect.transform.parent = null;

        EventHolder.OnRestart += Restart;
    }

    private void OnDestroy()
    {
        EventHolder.OnRestart -= Restart;
    }

    public void Restart()
    {
        gameObject.SetActive(false);
    }

    public void Restart(GameObject player)
    {
        if (player == gameObject)
        {
            Restart(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        EventHolder.OnHit(collision.gameObject, gameObject, 1);
        gameObject.SetActive(false);
    }
}
