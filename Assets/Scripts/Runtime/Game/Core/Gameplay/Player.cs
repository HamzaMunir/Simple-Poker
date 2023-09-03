using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks.Linq;
using Game;
using SimplePoker.Core.Environment;
using SimplePoker.GameMessages;
using SimplePoker.Utils;
using UnityEngine;

namespace SimplePoker.Core
{
    public class Player : BasePlayer
    {
        public override void InitPlayer(EPlayerType id, int positionAtTable, PlayerSpot playerSpot)
        {
            base.InitPlayer(id, positionAtTable, playerSpot);
            MessageBroker.Instance.Receive<ExecutePlayerCommandMessage>()
                .Subscribe(OnExecutePlayerCommand, _cancellationToken.Token);
        }

        private void OnExecutePlayerCommand(ExecutePlayerCommandMessage data)
        {
            var command = GameManager.PlayerCommandFactory.GetCommand(data.CommandType);
            command.Execute(this);
        }

        protected override void DecideAndExecuteCommand()
        {
            if (_currentState == EHandCommandType.Fold)
            {
                var command = GameManager.PlayerCommandFactory.GetCommand(EHandCommandType.Fold);
                command.Execute(this);
            }
        }
    }
}