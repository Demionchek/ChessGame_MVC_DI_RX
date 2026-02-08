using Core.Model;
using Presentation.Controller;
using UnityEngine.EventSystems;

namespace Presentation.View
{
    using UnityEngine;
    using VContainer;

    public class PieceView : MonoBehaviour, IPointerClickHandler
    {
        public Position Position;

        private InputController _input;
        private SpriteRenderer _renderer;

        [Inject]
        public void Construct(InputController input)
        {
            _input = input;
        }

        private void Awake()
        {
            _renderer = GetComponent<SpriteRenderer>();
        }

        public void SetSprite(Sprite sprite)
        {
            _renderer.sprite = sprite;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Piece clicked");
            _input.OnCellClicked(Position);
        }
    }
}