using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

public class BadmintonStamina : MonoBehaviour
{

    [SerializeField] private float maxStamina;
    [SerializeField] private Image staminaFillImage;


    [Header("Stamina costs for actions")]
    [SerializeField] private float running;
    [SerializeField] private float smashShot;
    [SerializeField] private float dropShot;
    [SerializeField] private float lobShot;
    [SerializeField] private float dash;
    [SerializeField] private float resting;

    [Header("running stamina checkpoints")]
    [SerializeField] private float checkpoint1;
    [SerializeField] private float checkpoint2;


    public enum actions
    {
        Running, SmashShot, DropShot, lobShot, Dash, Rest
    }

    private float currentStamina;



    void Start()
    {

        currentStamina = maxStamina;

        SetNewStamina();
    }

    public bool UseStamina(actions actionUsed)
    {
        float staminaCost = 0;
        switch (actionUsed) { 
            case actions.Running: staminaCost = running; break;
            case actions.SmashShot: staminaCost = smashShot; break;
            case actions.DropShot: staminaCost = dropShot; break;
            case actions.lobShot: staminaCost = lobShot; break;
            case actions.Dash: staminaCost = dash; break;
            case actions.Rest:
                currentStamina += resting;
                if (currentStamina > maxStamina) currentStamina = maxStamina;
                SetNewStamina();
                return true;

        }

        if (currentStamina < staminaCost) return false; //not enough stamina

        currentStamina -= staminaCost; //use stamina
        SetNewStamina();
        return true;

    }

    private void SetNewStamina()
    {
        if(staminaFillImage != null)
        staminaFillImage.fillAmount = currentStamina / maxStamina;
    }
    public float GetStamina()
    {
        return currentStamina;
    }

    public float GetStaminaLimit(int checkPointNo)
    {
        if (checkPointNo == 1) { return checkpoint1; }
        if (checkPointNo == 2) { return checkpoint2; }
        else return 0;
    }
}
