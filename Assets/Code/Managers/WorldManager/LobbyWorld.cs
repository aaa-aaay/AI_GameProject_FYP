using UnityEngine;

public class LobbyWorld : MonoBehaviour
{

    [SerializeField] private GameObject lockedModel;
    [SerializeField] private GameObject UnlockedModel;

    private WorldSelect worldSelect;

    private void Awake()
    {
        lockedModel.SetActive(false);
        UnlockedModel.SetActive(false);
        worldSelect = GetComponentInChildren<WorldSelect>();
    }

    public void SetState(bool unlockedworld)
    {
        lockedModel.SetActive(!unlockedworld);
        UnlockedModel.SetActive(unlockedworld);
    }
}
