﻿using System.Threading.Tasks;
using Abp.Application.Services;
using FeeSystem.Authorization.Accounts.Dto;

namespace FeeSystem.Authorization.Accounts
{
    public interface IAccountAppService : IApplicationService
    {
        Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input);

        Task<RegisterOutput> Register(RegisterInput input);
    }
}
