using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Test.Models;
using System.Net.Mime;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Authentication.Cookies;
using API_Test.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using System.Text.Json;

namespace API_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private IConfiguration Configuration { get; }
        private readonly PortfolioGenDBContext _context;

        public ProfileController(PortfolioGenDBContext context, IConfiguration config)
        {
            _context = context;
            Configuration = config;
        }

        // GET: api/Profile
        [HttpGet]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            return await _context.Profiles.ToListAsync();
        }


        // GET: api/Profile/5
        [HttpGet("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<Profile>> GetProfile(int id)
        {
            var profiles = await _context.Profiles.FindAsync(id);

            if (profiles == null)
            {
                return NotFound("Not found");
            }

            return profiles;
        }


        [HttpGet("/github/oauth/token/success")]
        [Produces("application/json")]
        public async Task<ActionResult<String>> GitHubSignInData([FromQuery] String code)
        {
            string token = "";
            string clientID = Configuration["Github:ClientId"];
            string clientSecret = Configuration["Github:ClientSecret"];
            using (HttpClient client = new HttpClient())
            {
                var parameters = new Dictionary<string, string> { { "client_id", clientID }, { "client_secret", clientSecret }, { "code", code } }; 
                var encodedContent = new FormUrlEncodedContent(parameters);
                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://github.com/login/oauth/access_token", encodedContent);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    return responseBody;

                }
                catch (HttpRequestException e)
                {
                    return e.Message;
                };
            }

        }


        [HttpGet("profiles/{name}")]
        [Produces("application/json")]
        public async Task<ActionResult<Profile>> GetProfileByName(String name)
        {
            var profiles = await _context.Profiles.Where(profile => profile.ProfileName == name).FirstOrDefaultAsync();

            if (profiles == null)
            {
                return NotFound("Could not find user with that profilename");
            }

            return profiles;
        }
        // PUT: api/Profile/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProfile(int id, Profile profiles)
        {
            if (id != profiles.Id)
            {
                return BadRequest();
            }

            _context.Entry(profiles).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProfilesExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Profile
        [HttpPost]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        [Produces("application/json")]
        public async Task<ActionResult<Profile>> PostProfile([FromForm]Profile profiles)

        {
            if (ProfileNameExists(profiles.ProfileName))
            {
                this.ModelState.AddModelError("Error", "Profilename already exists");
                BadRequestObjectResult conflictResult = this.BadRequest(this.ModelState);
                conflictResult.StatusCode = 409;
                return conflictResult; 
            }

            _context.Profiles.Add(profiles);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetProfile", new { id = profiles.Id }, profiles);
        }

        // DELETE: api/Profile/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Profile>> DeleteProfile(int id)
        {
            var profiles = await _context.Profiles.FindAsync(id);
            if (profiles == null)
            {
                return NotFound();
            }

            _context.Profiles.Remove(profiles);
            await _context.SaveChangesAsync();

            return profiles;
        }

        private bool ProfilesExists(int id)
        {
            return _context.Profiles.Any(e => e.Id == id);
        }

        private bool ProfileNameExists(String profileName)
        {
            return _context.Profiles.Any(e => e.ProfileName == profileName);
        }
    }
}