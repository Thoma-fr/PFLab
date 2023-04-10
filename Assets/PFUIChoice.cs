using UnityEngine;
using UnityEngine.EventSystems;
public class PFUIChoice : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] private int id;
    private PFUIContainer _container;

    private void Awake()
    {
        _container = GetComponentInParent<PFUIContainer>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(_container)
            _container.SelectPF(id);
    }
}
