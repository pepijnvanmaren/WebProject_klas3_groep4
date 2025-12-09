using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using WebProject_klas3_groep4;
using WebProject_klas3_groep4.Controllers;
using WebProject_klas3_groep4.models;
using WebProject_klas3_groep4.DTO;
using WebProject_klas3_groep4.Tests.TestHelpers;
using Xunit;

namespace WebProject_klas3_groep4.Tests
{
    public class VeilingProcessTest
    {
        private readonly VeilingProcessController? _controller;
        private readonly DatabaseContext _context;



        [Fact]
        public async Task VeilingProcess_SuccesfullyCompletes()
        {
            // Arrange
            var Actief = TestSpawner.CreateValidProductStatus(1, VeilingStatus.Actief);

            _context.Producten.AddRange(Actief);
            await _context.SaveChangesAsync();

            // Act
            var result = await _controller.StartVeiling();

            // Assert

            var badRequest = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal("Er is al een actieve veiling", badRequest.Value);
        }
    }
}
