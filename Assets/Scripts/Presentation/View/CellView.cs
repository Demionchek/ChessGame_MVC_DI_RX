using Core.Model;
using Presentation.Controller;
using UnityEngine;
using UnityEngine.EventSystems;
using VContainer;

namespace Presentation.View
{
    public class CellView : MonoBehaviour, IPointerClickHandler
    {
        public Position Position;

        private SpriteRenderer _renderer;

        private InputController _input;

        [Inject]
        public void Construct(InputController input)
        {
            _input = input;
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetColor(Color color)
        {
            _renderer.color = color;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Cell clicked");
            _input.OnCellClicked(Position);
        }
    }
}