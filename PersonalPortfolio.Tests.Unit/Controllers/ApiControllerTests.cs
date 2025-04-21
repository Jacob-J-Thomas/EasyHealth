﻿using System.Threading.Tasks;
using PersonalPortfolio.API;
using PersonalPortfolio.Business;
using PersonalPortfolio.Controllers;
using PersonalPortfolio.Host.Config;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace PersonalPortfolio.Tests.Unit.Controllers
{
    public class ApiControllerTests
    {
        private readonly Mock<IAuthLogic> _mockAuthLogic;
        private readonly Mock<IPublicApiLogic> _mockPromptFlowLogic;
        private readonly Mock<StripeSettings> _stripeSettings;
        private readonly ApiController _apiController;

        public ApiControllerTests()
        {
            _mockAuthLogic = new Mock<IAuthLogic>();
            _mockPromptFlowLogic = new Mock<IPublicApiLogic>();
            _stripeSettings = new Mock<StripeSettings>();
            _apiController = new ApiController(_mockAuthLogic.Object, _mockPromptFlowLogic.Object, _stripeSettings.Object);
        }
    }
}