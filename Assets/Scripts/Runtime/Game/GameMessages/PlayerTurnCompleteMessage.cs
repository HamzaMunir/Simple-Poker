using System.Collections;
using System.Collections.Generic;
using SimplePoker.Core;
using UnityEngine;

namespace SimplePoker.GameMessages
{
    public class PlayerTurnCompleteMessage
    {
        public EPlayerType PlayerType;
        public int Index;
        public EHandCommandType CommandType;
    }
}