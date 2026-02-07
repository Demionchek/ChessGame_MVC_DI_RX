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

        [Inject]
        public void Construct(InputController input)
        {
            _input = input;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Debug.Log("Piece clicked");
            _input.OnCellClicked(Position);
        }
    }
}