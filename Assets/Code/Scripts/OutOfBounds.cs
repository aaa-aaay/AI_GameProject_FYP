using Grpc.Core;
using UnityEngine;

public class OutOfBounds : ScoringArea
{
    protected override void OnCollisionEnter(Collision collision)
    {
        if (balls.Contains(collision.gameObject))
        {
            EventHandler.InvokeOutOfBounds(collision.gameObject);
        }
    }
}
