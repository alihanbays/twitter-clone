using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TwitterClone.Controllers;
using TwitterClone.Data;
using TwitterClone.Models;
using Xunit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tests
{
    public class UserTweetTests
    {
        private static (AppDbContext context, UserManager<User> userManager) CreateIdentity(string name)
        {
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddDbContext<AppDbContext>(o => o.UseInMemoryDatabase(name));
            services
                .AddIdentityCore<User>(opts =>
                {
                    opts.Password.RequireDigit = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireNonAlphanumeric = false;
                })
                .AddEntityFrameworkStores<AppDbContext>();

            var provider = services.BuildServiceProvider();
            var context = provider.GetRequiredService<AppDbContext>();
            context.Database.EnsureCreated();
            var userManager = provider.GetRequiredService<UserManager<User>>();
            return (context, userManager);
        }

        [Fact]
        public async Task Register_AddsUser()
        {
            var (context, userManager) = CreateIdentity(nameof(Register_AddsUser));
            var controller = new UserController(userManager);
            var dto = new RegisterUserDto { Username = "alice", Password = "Password123" };

            var result = await controller.Register(dto);

            Assert.IsType<OkObjectResult>(result);
            Assert.NotNull(await userManager.FindByNameAsync("alice"));
        }

        [Fact]
        public async Task Register_DuplicateUsername_ReturnsBadRequest()
        {
            var (context, userManager) = CreateIdentity(nameof(Register_DuplicateUsername_ReturnsBadRequest));
            var controller = new UserController(userManager);
            await controller.Register(new RegisterUserDto { Username = "bob", Password = "pass" });

            var result = await controller.Register(new RegisterUserDto { Username = "bob", Password = "pass" });

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Login_ReturnsToken()
        {
            var (context, userManager) = CreateIdentity(nameof(Login_ReturnsToken));
            var controller = new UserController(userManager);
            await controller.Register(new RegisterUserDto { Username = "carol", Password = "pass" });
            Environment.SetEnvironmentVariable("JWT_KEY", "testkey");

            var result = await controller.Login(new LoginDto { Username = "carol", Password = "pass" }) as OkObjectResult;

            Assert.NotNull(result);
            Assert.True(((string?)result.Value?.GetType().GetProperty("Token")?.GetValue(result.Value))?.Length > 0);
        }

        [Fact]
        public async Task GetAllUsers_ReturnsUsers()
        {
            var (context, userManager) = CreateIdentity(nameof(GetAllUsers_ReturnsUsers));
            var controller = new UserController(userManager);
            await controller.Register(new RegisterUserDto { Username = "dave", Password = "pass" });

            var actionResult = await controller.GetAllUsers();
            var result = Assert.IsType<OkObjectResult>(actionResult.Result);

            Assert.NotNull(result);
            var users = Assert.IsAssignableFrom<IEnumerable<User>>(result.Value);
            Assert.Single(users);
        }

        [Fact]
        public async Task DeleteUser_RemovesUser()
        {
            var (context, userManager) = CreateIdentity(nameof(DeleteUser_RemovesUser));
            var controller = new UserController(userManager);
            await controller.Register(new RegisterUserDto { Username = "erin", Password = "pass" });

            var result = await controller.DeleteUser("erin");

            Assert.IsType<OkObjectResult>(result);
            Assert.Null(await userManager.FindByNameAsync("erin"));
        }


        [Fact]
        public async Task UpdateUser_ChangesUsername()
        {
            var (context, userManager) = CreateIdentity(nameof(UpdateUser_ChangesUsername));
            var controller = new UserController(userManager);
            await controller.Register(new RegisterUserDto { Username = "old", Password = "pass" });
            var result = await controller.UpdateUser("old", new UpdateUserDto { Username = "new", Password = "newpass" }) as OkObjectResult;
            Assert.NotNull(result);
            Assert.NotNull(await userManager.FindByNameAsync("new"));
        }

        [Fact]
        public async Task CreateTweet_AddsTweet()
        {
            var (context, userManager) = CreateIdentity(nameof(CreateTweet_AddsTweet));
            var controller = new TweetController(context, userManager);
            await userManager.CreateAsync(new User { UserName = "ed" }, "pass");
            var result = await controller.CreateTweet(new CreateTweetDto { Username = "ed", Content = "hi" }) as OkObjectResult;
            Assert.NotNull(result);
            var tweet = Assert.IsType<Tweet>(result.Value);
            Assert.Equal("hi", tweet.Content);
        }

        [Fact]
        public async Task GetTweets_ReturnsTweets()
        {
            var (context, userManager) = CreateIdentity(nameof(GetTweets_ReturnsTweets));
            var controller = new TweetController(context, userManager);
            await userManager.CreateAsync(new User { UserName = "eve" }, "pass");
            await controller.CreateTweet(new CreateTweetDto { Username = "eve", Content = "first" });
            var result = await controller.GetTweets() as OkObjectResult;
            Assert.NotNull(result);
            var tweets = Assert.IsAssignableFrom<IEnumerable<Tweet>>(result.Value);
            Assert.Single(tweets);
        }


        [Fact]
        public async Task UpdateTweet_UpdatesContent()
        {
            var (context, userManager) = CreateIdentity(nameof(UpdateTweet_UpdatesContent));
            var tweetController = new TweetController(context, userManager);
            await userManager.CreateAsync(new User { UserName = "frank" }, "pass");
            var createResult = await tweetController.CreateTweet(new CreateTweetDto { Username = "frank", Content = "hello" }) as OkObjectResult;
            var tweet = Assert.IsType<Tweet>(createResult!.Value);

            var update = await tweetController.UpdateTweet(tweet.Id, new UpdateTweetDto { Content = "updated" }) as OkObjectResult;

            var updated = Assert.IsType<Tweet>(update!.Value);
            Assert.Equal("updated", updated.Content);
        }

        [Fact]
        public async Task DeleteTweet_RemovesTweet()
        {
            var (context, userManager) = CreateIdentity(nameof(DeleteTweet_RemovesTweet));
            var tweetController = new TweetController(context, userManager);
            await userManager.CreateAsync(new User { UserName = "gary" }, "pass");
            var create = await tweetController.CreateTweet(new CreateTweetDto { Username = "gary", Content = "yo" }) as OkObjectResult;
            var tweet = Assert.IsType<Tweet>(create!.Value);

            var del = await tweetController.DeleteTweet(tweet.Id);

            Assert.IsType<OkObjectResult>(del);
            Assert.Empty(context.Tweets.ToList());
        }
    }
}
