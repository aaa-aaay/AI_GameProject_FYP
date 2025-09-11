using System.Buffers;
using Unity.VisualScripting;
using UnityEngine;

public class TeamMaterials : MonoBehaviour
{
    [SerializeField] Material player_material;
    [SerializeField] Material enemy_material;

    private Material start_material;
    private MeshRenderer mesh_renderer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        mesh_renderer = GetComponent<MeshRenderer>();
        start_material = mesh_renderer.material;

        EventHandler.GotKill += on_kill;
        EventHandler.EndScenario += reset_materials;
    }

    public void on_kill(GameObject killer, GameObject target)
    {
        if (target == gameObject)
        {
            //print(gameObject.name + " from team " + (int)TeamSingleton.instance.get_team(gameObject) + " to " + (int)TeamSingleton.instance.get_team(killer));
            if (TeamSingleton.instance.get_team(killer) != Teams.None)
            {
                switch (TeamSingleton.instance.get_team(killer))
                {
                    case Teams.Player:
                        mesh_renderer.material = player_material;
                        break;
                    case Teams.Enemy:
                        mesh_renderer.material = enemy_material;
                        break;
                    default:
                        print("Huh???");
                        break;
                }
            }
        }
    }

    public void reset_materials()
    {
        mesh_renderer.material = start_material;
    }
}
