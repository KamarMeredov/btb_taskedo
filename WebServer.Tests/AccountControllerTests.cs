using Xunit;
using Moq;
using WebServer.DTO;
using WebServer.DTO.ReponseObjects;
using WebServer.Services;
using Microsoft.AspNetCore.Http;
using WebServer.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace WebServer.Tests
{
    public class AccountControllerTests
    {
        private static TheoryData<LoginUserDTO, int> GetLoginCreds()
        {
            return new TheoryData<LoginUserDTO, int>
            {
                { new LoginUserDTO { Email = "user1@gmail.com", Password = "12345User1_"}, StatusCodes.Status200OK},
                { new LoginUserDTO { Email = "user2@gmail.com", Password = "12345User2_"}, StatusCodes.Status404NotFound },
                { new LoginUserDTO { Email = "user1@gmail.com", Password = "22345User2_"}, StatusCodes.Status401Unauthorized }
            };
        }

        private static TheoryData<RegisterUserDTO, int> GetRegisterCreds()
        {
            return new TheoryData<RegisterUserDTO, int>
            {
                { new RegisterUserDTO { Email = "usergmail.com", Password = "12345User1_", Name = "User1User1" }, StatusCodes.Status400BadRequest },
                { new RegisterUserDTO { Email = "user1@gmail.com", Password = "12345User1_", Name = "User1User1" }, StatusCodes.Status201Created },
                { new RegisterUserDTO { Email = "user2@gmail.com", Password = "12345User2_", Name = "User2User2" }, StatusCodes.Status409Conflict },
            };
        }

        [Theory]
        [MemberData("GetLoginCreds")]
        public async Task LoginTest(LoginUserDTO loginDto, int expectedStatusCode)
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            mock.Setup(x => x.LoginAsync(loginDto))
                .ReturnsAsync((LoginUserDTO dto) =>
                {
                    if (dto.Email == "user1@gmail.com" && dto.Password == "12345User1_")
                    {
                        return (new LoginResponse
                        {
                            AccessToken = "token",
                            Email = dto.Email,
                            Message = "Success"
                        }, StatusCodes.Status200OK);
                    }

                    if (dto.Email == "user1@gmail.com" && dto.Password != "12345User1_")
                    {
                        return (new LoginResponse
                        {
                            Email = dto.Email,
                            Message = "Incorrect password"
                        }, StatusCodes.Status401Unauthorized);
                    }

                    return (new LoginResponse
                    {
                        Email = dto.Email,
                        Message = "User with email not found"
                    }, StatusCodes.Status404NotFound);
                });

            var accountController = new AccountController(null, mock.Object);


            // Act
            var result = await accountController.Login(loginDto);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult.StatusCode == expectedStatusCode);
        }


        [Theory]
        [MemberData("GetRegisterCreds")]
        public async Task RegisterTest(RegisterUserDTO registerDto, int expectedStatusCode)
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            mock.Setup(x => x.RegisterAsync(registerDto))
                .ReturnsAsync((RegisterUserDTO dto) =>
                {
                    if (dto.Email == "usergmail.com" && dto.Password == "12345User1_" && dto.Name == "User1User1")
                    {
                        return (new RegistrationResponse
                        {
                            Email = dto.Email,
                            UserName = dto.Name
                        }, StatusCodes.Status400BadRequest);
                    }

                    if (dto.Email == "user1@gmail.com" && dto.Password == "12345User1_" && dto.Name == "User1User1" )
                    {
                        return (new RegistrationResponse
                        {
                            Email = dto.Email,
                            UserName = dto.Name
                        }, StatusCodes.Status201Created);
                    }

                    return (new RegistrationResponse
                    {
                        Email = dto.Email,
                        UserName = dto.Name
                    }, StatusCodes.Status409Conflict);
                });

            var accountController = new AccountController(null, mock.Object);

            // Act
            var result = await accountController.Register(registerDto);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult.StatusCode == expectedStatusCode);
        }
    }
}