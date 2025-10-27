using UnityEngine;

public class ShotTypeTracker : MonoBehaviour
{

    private Racket.ShotType _currentType;


    public Racket.ShotType getShotType()
    {
        return _currentType;
    }

    public void setShotType(Racket.ShotType type)
    {
        _currentType = type;
    }
}
