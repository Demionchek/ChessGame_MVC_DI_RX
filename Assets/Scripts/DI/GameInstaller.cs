using System;
using Core.Model;
using Core.Rules;
using Presentation.Controller;
using Presentation.View;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace DI
{
    public class GameInstaller : LifetimeScope
    {
        [SerializeField] private BoardView _board;
        [SerializeField] private InputController _input;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<ChessRules>(Lifetime.Singleton);
            builder.Register<GameModel>(Lifetime.Singleton);

            builder.RegisterComponent(_board);
            builder.RegisterComponent(_input);
        }
    }
}