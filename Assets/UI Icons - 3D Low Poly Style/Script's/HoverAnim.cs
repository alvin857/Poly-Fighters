using UnityEngine;
using UnityEngine.EventSystems;

namespace FWC
{
    public class HoverAnim : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {

        [SerializeField] float scaleChange = 1.1f;

        [SerializeField] AudioSource source;

        private Vector3 originalScale;

        void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            transform.localScale *= scaleChange;

            if (source.clip == null) return;

            source.PlayOneShot(source.clip);

        }

        public void OnPointerExit(PointerEventData eventData)
        {
            transform.localScale = originalScale;

        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = originalScale;
        }
    }

}