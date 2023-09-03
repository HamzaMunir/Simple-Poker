using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimplePoker;
using SimplePoker.Core;
using SimplePoker.Core.Commands;
using SimplePoker.Utils;
using UnityEngine;

public class EnemyPlayer : BasePlayer
{
    private EnemyAi _enemyAi = new();

    public void Start()
    {
        _enemyAi = new EnemyAi();
    }
    
    protected override void DecideAndExecuteCommand()
    {
        if (_currentState == EHandCommandType.Fold)
        {
            new FoldCommand().Execute(this);
            return;
        }
        
        var decisionType = _enemyAi.GetADecision(GetCardsWeight(),(int) _moneyInBank, (int)_moneySpent);
        IPlayerCommand command = GameManager.PlayerCommandFactory.GetCommand(decisionType);
        command.Execute(this);
        _currentState = decisionType;
    }
    
}
