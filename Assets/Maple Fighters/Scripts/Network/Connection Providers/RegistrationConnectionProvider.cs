﻿using System;
using System.Threading.Tasks;
using Authorization.Client.Common;
using CommonTools.Coroutines;
using CommonTools.Log;
using CommunicationHelper;
using Registration.Common;
using Scripts.Containers;
using Scripts.Services;
using Scripts.UI.Core;
using Scripts.UI.Windows;

namespace Scripts.UI.Controllers
{
    public class RegistrationConnectionProvider : ServiceConnectionProviderBase<RegistrationConnectionProvider>
    {
        public Action onConnected;

        public void Connect(Action onConnected)
        {
            this.onConnected = onConnected;

            var serverConnectionInformation = GetServerConnectionInformation(ServerType.Registration);
            CoroutinesExecutor.StartTask((yield) => Connect(yield, serverConnectionInformation, authorize: false));
        }

        protected override void OnPreConnection()
        {
            // Left blank intentionally
        }

        protected override void OnConnectionFailed()
        {
            var noticeWindow = UserInterfaceContainer.GetInstance().Get<NoticeWindow>().AssertNotNull();
            noticeWindow.Message.text = "Could not connect to a registration server.";
            noticeWindow.OkButton.interactable = true;
        }

        protected override void OnConnectionEstablished()
        {
            onConnected.Invoke();
        }

        protected override Task<AuthorizeResponseParameters> Authorize(IYield yield, AuthorizeRequestParameters parameters)
        {
            throw new NotImplementedException();
        }

        protected override void OnPreAuthorization()
        {
            // Left blank intentionally
        }

        protected override void OnNonAuthorized()
        {
            // Left blank intentionally
        }

        protected override void OnAuthorized()
        {
            // Left blank intentionally
        }

        protected override void SetPeerLogicAfterAuthorization()
        {
            GetServiceBase().SetPeerLogic<RegistrationPeerLogic, RegistrationOperations, EmptyEventCode>();
        }

        protected override IServiceBase GetServiceBase()
        {
            return ServiceContainer.RegistrationService;
        }
    }
}