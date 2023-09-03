using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimplePoker.Core.Commands
{
    public interface IPlayerCommand
    {
        public EHandCommandType CommandType { get; }
        public void Execute(BasePlayer player);
    }
}