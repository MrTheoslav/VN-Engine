using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonBehaviours : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private static ButtonBehaviours selectedButton = null;
    public Animator anim;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (selectedButton != null && selectedButton != this)
        {
            selectedButton.OnPointerExit(null);
        }

        anim.Play("Enter");
        selectedButton = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        anim.Play("Exit");
        selectedButton = null;
    }
}
