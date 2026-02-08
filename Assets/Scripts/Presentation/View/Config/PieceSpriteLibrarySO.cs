using System;
using System.Collections.Generic;
using Core.Model;
using UnityEngine;

namespace Presentation.View.Config
{
    [CreateAssetMenu(menuName = "Chess/Piece Sprite Library")]
    public class PieceSpriteLibrarySO : ScriptableObject
    {
        [Serializable]
        public struct Entry
        {
            public PieceType Type;
            public PieceColor Color;
            public Sprite Sprite;
        }

        [SerializeField] private List<Entry> _entries;

        private Dictionary<(PieceType, PieceColor), Sprite> _map;

        public Sprite Get(PieceType type, PieceColor color)
        {
            _map ??= BuildMap();
            return _map[(type, color)];
        }

        private Dictionary<(PieceType, PieceColor), Sprite> BuildMap()
        {
            var dict = new Dictionary<(PieceType, PieceColor), Sprite>();
            foreach (var e in _entries)
                dict[(e.Type, e.Color)] = e.Sprite;
            return dict;
        }
    }
}