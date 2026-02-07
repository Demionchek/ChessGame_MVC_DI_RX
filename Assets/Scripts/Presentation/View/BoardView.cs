using System.Collections.Generic;
using Core.Model;
using UniRx;
using UnityEngine;
using VContainer;

namespace Presentation.View
{
    public class BoardView : MonoBehaviour
    {
        [Inject] private GameModel _game;

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
                    var cellGO = Instantiate(CellPrefab, Root);
                    cellGO.transform.localPosition = new Vector3(x, y, 0);

                    var cell = cellGO.GetComponent<CellView>();
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
                    var pos = new Position(x, y);
                    var piece = board.Get(pos);
                    if (piece == null) continue;

                    var go = Instantiate(PiecePrefab, Root);
                    go.transform.localPosition = new Vector3(x, y);

                    var pieceView = go.GetComponent<PieceView>();
                    pieceView.Position = pos;

                    _pieces[pos] = go;
                }
        }
    }
}