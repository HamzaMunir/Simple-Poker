using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimplePoker.Core.Commands;
using SimplePoker.UI;
using SimplePoker.Views;
using UnityEngine;

namespace SimplePoker.Core
{
    public class GameManager : Singleton<GameManager>
    {
        [Range(1,10)]
        public int EnemyPlayerCount = 1;

        public GameplayManager GameplayManager;
        public TableManager TableManager;
        public UiManager UiManager;
        public static readonly PlayerCommandFactory PlayerCommandFactory = new();
        [SerializeField] public CardView CardViewPrefab;
        void Start()
        {
            TableManager.InitTable();
        }

    }
}