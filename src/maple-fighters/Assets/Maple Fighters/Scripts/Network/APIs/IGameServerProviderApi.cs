﻿using System.Threading.Tasks;
using CommonTools.Coroutines;
using GameServerProvider.Client.Common;

namespace Scripts.Services
{
    public interface IGameServerProviderApi : IApiBase
    {
        Task<GameServersProviderResponseParameters> ProvideGameServersAsync(
            IYield yield);
    }
}