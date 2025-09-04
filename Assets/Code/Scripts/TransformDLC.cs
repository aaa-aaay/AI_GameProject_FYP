using UnityEngine;

public class TransformDLC : MonoBehaviour
{
    public static GameObject get_parent(GameObject gameObject, string layer_name)
    {
        GameObject temp = gameObject;
        print(temp.layer + " " + LayerMask.NameToLayer(layer_name));
        while (temp.layer != LayerMask.NameToLayer(layer_name))
        {
            temp = temp.transform.parent.gameObject;
        }
        return temp;
    }
}
