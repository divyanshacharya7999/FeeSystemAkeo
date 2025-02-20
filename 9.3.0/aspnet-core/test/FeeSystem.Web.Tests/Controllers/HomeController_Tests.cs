﻿using System.Threading.Tasks;
using FeeSystem.Models.TokenAuth;
using FeeSystem.Web.Controllers;
using Shouldly;
using Xunit;

namespace FeeSystem.Web.Tests.Controllers
{
    public class HomeController_Tests: FeeSystemWebTestBase
    {
        [Fact]
        public async Task Index_Test()
        {
            await AuthenticateAsync(null, new AuthenticateModel
            {
                UserNameOrEmailAddress = "admin",
                Password = "123qwe"
            });

            //Act
            var response = await GetResponseAsStringAsync(
                GetUrl<HomeController>(nameof(HomeController.Index))
            );

            //Assert
            response.ShouldNotBeNullOrEmpty();
        }
    }
}