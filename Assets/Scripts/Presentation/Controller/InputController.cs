using Core.Model;
using UniRx;
using UnityEngine;
using VContainer;

namespace Presentation.Controller
{
    public class InputController : MonoBehaviour
    {
        private bool _placementMode = true;
        private PieceType _currentType = PieceType.Pawn;
        private PieceColor _currentColor = PieceColor.White;
        private Subject<Position> _clickStream = new();

        [Inject] private GameModel _game;

        private void Start()
        {
            _clickStream
                .Buffer(2)
                .Subscribe(positions =>
                {
                    _game.TryMove(positions[0], positions[1]);
                });
        }

        public void OnCellClicked(Position pos)
        {
            if (_placementMode)
            {
                _game.PlacePiece(pos, new Piece(_currentType, _currentColor));
                return;
            }

            _clickStream.OnNext(pos);
        }
    }
}