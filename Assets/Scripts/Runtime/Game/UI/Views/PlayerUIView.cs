using System;
using System.Collections;
using System.Collections.Generic;
using Game;
using Kit;
using SimplePoker.Core;
using SimplePoker.GameMessages;
using SimplePoker.UI.Data;
using UnityEngine;
using UnityEngine.UI;

namespace SimplePoker.Views
{
    public class PlayerUIView : UiBaseView
    {
        private PlayerUIData _uiData => Data as PlayerUIData;
        [SerializeField] private Transform _initialRoundInteractables;
        [SerializeField] private Transform _otherRoundInteractables;
        [SerializeField] private Button _foldBtn;
        [SerializeField] private Button _callBtn;
        [SerializeField] private Button _raiseBtn;
        [SerializeField] private Button _checkBtn;
        [SerializeField] private Button _betBtn;

        private void OnEnable()
        {
            _foldBtn.onClick.AddListener(OnFold);
            _callBtn.onClick.AddListener(OnCall);
            _raiseBtn.onClick.AddListener(OnRaise);
            _checkBtn.onClick.AddListener(OnCheck);
            _betBtn.onClick.AddListener(OnBet);
        }

        private void OnDisable()
        {
            _foldBtn.onClick.RemoveListener(OnFold);
            _callBtn.onClick.RemoveListener(OnCall);
            _raiseBtn.onClick.RemoveListener(OnRaise);
            _checkBtn.onClick.RemoveListener(OnCheck);
            _betBtn.onClick.RemoveListener(OnBet);
        }

        private void OnBet()
        {
            SendMessageToPlayer(EHandCommandType.Bet);
        }

        private void OnCheck()
        {
            SendMessageToPlayer(EHandCommandType.Check);
        }

        private void OnRaise()
        {
            SendMessageToPlayer(EHandCommandType.Raise);
        }

        private void OnCall()
        {
            SendMessageToPlayer(EHandCommandType.Call);
        }

        private void OnFold()
        {
            SendMessageToPlayer(EHandCommandType.Fold);
        }

        public void SendMessageToPlayer(EHandCommandType commandType)
        {
            AudioManager.Instance.PlayUI(GameManager.Instance.UiManager.ClickedSound);
            Hide();
            MessageBroker.Instance.Publish(new ExecutePlayerCommandMessage()
            {
                CommandType = commandType
            });
        }
        
        protected override void RefreshView()
        {
            _initialRoundInteractables.gameObject.SetActive(_uiData.GameRound == EGameRound.Initial);
            _otherRoundInteractables.gameObject.SetActive(_uiData.GameRound != EGameRound.Initial);
            if (_uiData.GameRound != EGameRound.Initial)
            {
                _checkBtn.interactable = !_uiData.HasSomeoneBet;
                _betBtn.interactable = _uiData.CanBet;
            }
            else
            {
                _callBtn.interactable = _uiData.CanCall;
                _raiseBtn.interactable = _uiData.CanRaise;
            }
        }
        

        protected override void ResetView()
        {
        }
    }
}