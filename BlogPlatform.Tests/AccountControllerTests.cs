using Xunit;
using Moq;
using BlogPlatform.DTO;
using BlogPlatform.DTO.ReponseObjects;
using BlogPlatform.Services;
using Microsoft.AspNetCore.Http;
using BlogPlatform.Controllers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogPlatform.Data.Models;

namespace BlogPlatform.Tests
{
    public class AccountControllerTests
    {
        private static TheoryData<LoginUserDTO, int> GetLoginCreds()
        {
            return new TheoryData<LoginUserDTO, int>
            {
                { new LoginUserDTO { Email = "user1@gmail.com", Password = "12345User1_"}, StatusCodes.Status200OK},
                { new LoginUserDTO { Email = "user2@gmail.com", Password = "12345User2_"}, StatusCodes.Status401Unauthorized },
                { new LoginUserDTO { Email = "user1@gmail.com", Password = "22345User2_"}, StatusCodes.Status401Unauthorized }
            };
        }

        private static TheoryData<RegisterUserDTO, int> GetRegisterCreds()
        {
            return new TheoryData<RegisterUserDTO, int>
            {
                { new RegisterUserDTO { Email = "usergmail.com", Password = "12345User1_", Name = "User1User1" }, StatusCodes.Status500InternalServerError },
                { new RegisterUserDTO { Email = "user1@gmail.com", Password = "12345User1_", Name = "User1User1" }, StatusCodes.Status201Created },
                { new RegisterUserDTO { Email = "user2@gmail.com", Password = "12345User2_", Name = "User2User2" }, StatusCodes.Status500InternalServerError },
            };
        }

        [Theory]
        [MemberData("GetLoginCreds")]
        public async Task LoginTest(LoginUserDTO loginDto, int expectedStatusCode)
        {
            // Arrange
            var mock = new Mock<IAccountService>();
            mock.Setup(x => x.LoginAsync(It.IsAny<LoginUserDTO>()))
                .ReturnsAsync((LoginUserDTO dto) =>
                {
                    if (dto.Email == "user1@gmail.com" && dto.Password == "12345User1_")
                    {
                        return new LoginResponse
                        {
                            AccessToken = "token",
                            Email = dto.Email,
                            Message = "Success"
                        };
                    }

                    if (dto.Email == "user1@gmail.com" && dto.Password != "12345User1_")
                    {
                        return new LoginResponse
                        {
                            Email = dto.Email,
                            Message = "Incorrect password"
                        };
                    }

                    return new LoginResponse
                    {
                        Email = dto.Email,
                        Message = "User with email not found"
                    };
                });

            mock.Setup(x => x.GetUserByEmail(It.IsAny<string>()))
                .ReturnsAsync((string x) =>
                {
                    if (x == "user1@gmail.com")
                    {
                        return new ApplicationUser();
                    }

                    return null;
                });

            mock.Setup(x => x.CheckUserPassword(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                .ReturnsAsync((ApplicationUser user, string password) =>
                {
                    if (password == "12345User1_")
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });

            var accountController = new AccountController(mock.Object);


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
            mock.Setup(x => x.RegisterAsync(It.IsAny<RegisterUserDTO>()))
                .ReturnsAsync((RegisterUserDTO dto) =>
                {
                    if (dto.Email == "usergmail.com" && dto.Password == "12345User1_" && dto.Name == "User1User1")
                    {
                        return (new RegistrationResponse
                        {
                            Email = dto.Email,
                            UserName = dto.Name
                        }, false);
                    }

                    if (dto.Email == "user1@gmail.com" && dto.Password == "12345User1_" && dto.Name == "User1User1" )
                    {
                        return (new RegistrationResponse
                        {
                            Email = dto.Email,
                            UserName = dto.Name
                        }, true);
                    }

                    return (new RegistrationResponse
                    {
                        Email = dto.Email,
                        UserName = dto.Name
                    }, false);
                });

            var accountController = new AccountController(mock.Object);

            // Act
            var result = await accountController.Register(registerDto);

            // Assert
            var jsonResult = result as JsonResult;
            Assert.NotNull(jsonResult);
            Assert.True(jsonResult.StatusCode == expectedStatusCode);
        }
    }
}