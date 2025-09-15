using UnityEngine;

public class LobbyWorld : MonoBehaviour
{

    //this Scripts displays if the world is unlocked or not and updates its stats
    [SerializeField] public int levelNo;
    [SerializeField] private bool _isUnlocked;
    [SerializeField] private int _starsUnlocked;

    private WorldSelect _worldSelect;



    [SerializeField] private Material _unlockedMaterial;
    [SerializeField] private Material _lockedMaterial;
    private MeshRenderer MeshRenderer;

    private void Awake()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
        _worldSelect = GetComponentInChildren<WorldSelect>();
        SetState(false);
    }


    public void SetState(bool unlockedworld)
    {
        if (unlockedworld)
        {
            _worldSelect.enabled = true;
            MeshRenderer.material = _unlockedMaterial;
        }
        else
        {
            _worldSelect.enabled = false;
            MeshRenderer.material = _lockedMaterial;
        }
    }

    public void UnlockedWorld()
    {
        _worldSelect.enabled = true;
        // the world in the lobby is now ready to play
    }
}
