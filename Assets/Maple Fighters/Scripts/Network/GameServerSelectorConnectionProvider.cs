﻿using System;
using System.Threading.Tasks;
using Authorization.Client.Common;
using CommonCommunicationInterfaces;
using CommonTools.Coroutines;
using CommonTools.Log;
using CommunicationHelper;
using GameServerProvider.Client.Common;
using Scripts.Containers;
using Scripts.UI.Core;
using Scripts.UI.Windows;
using Scripts.Utils;

namespace Scripts.Services
{
    public class GameServerSelectorConnectionProvider : ServiceConnectionProviderBase<GameServerSelectorConnectionProvider>
    {
        private Action onAuthorized;
        private AuthorizationStatus authorizationStatus = AuthorizationStatus.Failed;

        public void Connect(Action onAuthorized)
        {
            this.onAuthorized = onAuthorized;

            var serverConnectionInformation = GetServerConnectionInformation(ServerType.GameServerProvider);
            CoroutinesExecutor.StartTask((yield) => Connect(yield, serverConnectionInformation));
        }

        protected override void OnPreConnection()
        {
            // Left blank intentionally
        }

        protected override void OnConnectionFailed()
        {
            var noticeWindow = UserInterfaceContainer.Instance.Get<NoticeWindow>().AssertNotNull();
            noticeWindow.Message.text = "Could not connect to a game server provider.";
            noticeWindow.OkButton.interactable = true;
            noticeWindow.OkButtonClickedAction = LoadedObjects.DestroyAll;
        }

        protected override void OnConnectionEstablished()
        {
            ServiceContainer.GameServerProviderService.SetPeerLogic<AuthorizationService, AuthorizationOperations, EmptyEventCode>(new AuthorizationService());

            CoroutinesExecutor.StartTask(Authorize);
        }

        protected override void OnDisconnected(DisconnectReason reason, string details)
        {
            base.OnDisconnected(reason, details);

            GoBackToLogin();
        }

        private void GoBackToLogin()
        {
            if (authorizationStatus == AuthorizationStatus.Failed)
            {
                OnNonAuthorized();
            }
        }

        protected override Task<AuthorizeResponseParameters> Authorize(IYield yield, AuthorizeRequestParameters parameters)
        {
            var authorizationService = ServiceContainer.GameServerProviderService.GetPeerLogic<IAuthorizationServiceAPI>().AssertNotNull();
            return authorizationService.Authorize(yield, parameters);
        }

        protected override void OnPreAuthorization()
        {
            // Left blank intentionally
        }

        public void OnNonAuthorized()
        {
            var noticeWindow = UserInterfaceContainer.Instance.Get<NoticeWindow>().AssertNotNull();
            noticeWindow.Message.text = "Authorization with game server provider failed.";
            noticeWindow.OkButton.interactable = true;
            noticeWindow.OkButtonClickedAction = LoadedObjects.DestroyAll;
        }

        protected override void OnAuthorized()
        {
            ServiceContainer.GameServerProviderService.SetPeerLogic<GameServerProviderService, GameServerProviderOperations, EmptyEventCode>(new GameServerProviderService());

            var noticeWindow = UserInterfaceContainer.Instance.Get<NoticeWindow>().AssertNotNull();
            noticeWindow.Hide();

            authorizationStatus = AuthorizationStatus.Succeed;
            onAuthorized?.Invoke();
        }

        protected override IServiceBase GetServiceBase()
        {
            return ServiceContainer.GameServerProviderService;
        }
    }
}