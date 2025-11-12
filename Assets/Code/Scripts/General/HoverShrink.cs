using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverShrink : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private Animator animator;

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool("Hovering", true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool("Hovering", false);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Animator temp_animator = GetComponent<Animator>();
        animator = temp_animator;
    }

    
}
