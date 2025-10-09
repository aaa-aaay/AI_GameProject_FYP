using UnityEngine;

public class LobbyWorld : MonoBehaviour
{

    //this Scripts displays if the world is unlocked or not and updates its stats
    [SerializeField] public int levelNo;
    [SerializeField] private bool _isUnlocked;
    [SerializeField] private int _starsUnlocked;

    private WorldSelect _worldSelect;

    [SerializeField] private GameObject lockedModel;
    [SerializeField] private GameObject UnlockedModel;

    //[SerializeField] private Material _unlockedMaterial;
    //[SerializeField] private Material _lockedMaterial;
    //private MeshRenderer MeshRenderer;

    private void Awake()
    {
       // MeshRenderer = GetComponent<MeshRenderer>();
        _worldSelect = GetComponentInChildren<WorldSelect>();
        //SetState(false);

        lockedModel.SetActive(false);
        UnlockedModel.SetActive(false);
    }


    private void Start()
    {
    }

    public void SetState(bool unlockedworld)
    {
        if (unlockedworld)
        {
            _worldSelect.Activate(true);
            lockedModel.SetActive(false);
            UnlockedModel.SetActive(true);
            // MeshRenderer.material = _unlockedMaterial;
        }
        else
        {
            _worldSelect.Activate(false);
            lockedModel.SetActive(true);
            UnlockedModel.SetActive(false);
            //MeshRenderer.material = _lockedMaterial;
        }
    }

    public void UnlockedWorld()
    {
        _worldSelect.enabled = true;
        // the world in the lobby is now ready to play
    }
}
