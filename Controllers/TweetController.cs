using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TwitterClone.Data;
using TwitterClone.Models;
using static System.Net.Mime.MediaTypeNames;
using Microsoft.AspNetCore.Authorization;

namespace TwitterClone.Controllers
{
    [ApiController]
    [Route("tweets")]
    public class TweetController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly UserManager<User> _userManager;

        public TweetController(AppDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateTweet(CreateTweetDto dto)
        {
            var user = await _userManager.FindByNameAsync(dto.Username);
            if (user == null)
            {
                return NotFound("User not found.");
            }

            Tweet tweet = new()
            {
                Content = dto.Content,
                UserId = user.Id
            };
            _context.Tweets.Add(tweet);
            await _context.SaveChangesAsync();

            return Ok(tweet);
        }

        [HttpGet]
        public async Task<IActionResult> GetTweets()
        {
            var tweets = await _context.Tweets.ToListAsync();
            return Ok(tweets);
        }

        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateTweet(int id, UpdateTweetDto dto)
        {
            var tweet = await _context.Tweets.FindAsync(id);
            if (tweet == null)
            {
                return NotFound("Tweet not found.");
            }
            tweet.Content = dto.Content;
            tweet.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return Ok(tweet);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteTweet(int id)
        {
            var tweet = await _context.Tweets.FindAsync(id);
            if (tweet == null)
            {
                return NotFound("Tweet not found.");
            }
            _context.Tweets.Remove(tweet);
            await _context.SaveChangesAsync();
            return Ok(new { Message = "Tweet deleted." });
        }
    }
}