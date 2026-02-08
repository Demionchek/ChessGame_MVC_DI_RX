using System.Collections.Generic;
using Core.Model;
using Presentation.View.Config;
using UniRx;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Presentation.View
{
    public class BoardView : MonoBehaviour
    {
        [SerializeField] private PieceSpriteLibrarySO _spriteLibrary;

        [Inject] private GameModel _game;
        [Inject] private IObjectResolver _objectResolver;

        public GameObject CellPrefab;
        public GameObject PiecePrefab;

        public Transform Root;

        private Dictionary<Position, GameObject> _pieces = new();

        private void Awake()
        {
            GenerateBoard();
        }

        private void GenerateBoard()
        {
            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    GameObject cellGO = _objectResolver.Instantiate(CellPrefab, Root);
                    cellGO.transform.localPosition = new Vector3(x, y, 0);

                    CellView cell = cellGO.GetComponent<CellView>();
                    cell.Position = new Position(x, y);

                    bool isWhite = (x + y) % 2 == 0;
                    cell.SetColor(isWhite ? Color.white : Color.gray);
                }
        }

        private void Start()
        {
            _game.OnBoardChanged.Subscribe(UpdateView);

            _game.SetupInitialPosition();
        }

        private void UpdateView(BoardState board)
        {
            foreach (var kv in _pieces)
                Destroy(kv.Value);

            _pieces.Clear();

            for (int x = 0; x < 8; x++)
                for (int y = 0; y < 8; y++)
                {
                    Position pos = new Position(x, y);
                    Piece piece = board.Get(pos);
                    if (piece == null) continue;

                    GameObject go = _objectResolver.Instantiate(PiecePrefab, Root);
                    go.transform.localPosition = new Vector3(x, y, 0);

                    PieceView pieceView = go.GetComponent<PieceView>();
                    pieceView.Position = pos;

                    Sprite sprite = _spriteLibrary.Get(piece.Type, piece.Color);
                    pieceView.SetSprite(sprite);

                    _pieces[pos] = go;
                }
        }
    }
}