using System.Web;
using API_Test.Models;
using API_Test.DBContext; 
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // GET: api/Profile/profiles
        [HttpGet("profiles")]
        public async Task<ActionResult<IEnumerable<Profile>>> GetProfiles()
        {
            var list = await _context.Profiles.ToListAsync<Profile>();
            return Ok(list); 
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


        [HttpGet("/github/oauth/generate/token")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult> GitHubSignInData([FromQuery] String code, [FromQuery]int id)
        {
            Token token = new Token(); 
            string clientID = Configuration["Github:ClientId"];
            string clientSecret = Configuration["Github:ClientSecret"];
            var activeProfile = await _context.Profiles.FindAsync(id); 
            using (HttpClient client = new HttpClient())
            {
                
                var parameters = new Dictionary<string, string> { { "client_id", clientID }, { "client_secret", clientSecret }, { "code", code }}; 
                var encodedContent = new FormUrlEncodedContent(parameters);
                try
                {
                    HttpResponseMessage response = await client.PostAsync("https://github.com/login/oauth/access_token", encodedContent);
                    response.EnsureSuccessStatusCode();
                    string responseBody = await response.Content.ReadAsStringAsync();
                    var decodedURL = HttpUtility.UrlDecode(responseBody);
                    var dict = HttpUtility.ParseQueryString(decodedURL);


                    var resultObj = dict.AllKeys.ToDictionary(key => key, key => dict[key]);
                    token.AccessToken = resultObj["access_token"];
                    token.TokenType = resultObj["token_type"];
                    token.Id = id;
                    await PostToken(id, token); 
                    return Ok(resultObj);

                    //var requestURL = "https://api.github.com/user"; 
                    //var request = new HttpRequestMessage(HttpMethod.Get, requestURL);
                    //request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    ///request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", headers["access_token"]);
                    //request.Headers.Add("User-Agent", "API-Test"); 
                    //request.Headers.Add("authorization", $"Bearer {headers["access_token"]}");
                    //HttpResponseMessage res = await client.SendAsync(request);

                    //if (res != null)
                    //{
                    //    var jsonString = await res.Content.ReadAsStringAsync();
                    //    var jsonObj = new JsonResult(jsonString);
                    //    return Ok(jsonString);  
                    //}
                    //return null;


                }
                catch (HttpRequestException e)
                {
                    return NotFound(e);
                };
            }

        }

        [HttpGet("/tokens")]
        [Produces("application/json")]
        public async Task<ActionResult<IEnumerable<Token>>> GetTokens()
        {
            var list = await _context.Tokens.ToListAsync<Token>();
            return Ok(list);
        }

        [HttpGet("/tokens/{id}")]
        public async Task<ActionResult<Token>> GetToken(int id)
        {
            var token = await _context.Tokens.FindAsync(id);
            if (token == null)
            {
                return NotFound("Token could not be found");
            }
            return token; 
        }

        [HttpPost("/tokens/{profileID}")]
        [Consumes("application/json")]
        [Produces("application/json")]
        public async Task<ActionResult<Token>> PostToken(int profileID, [FromBody]Token token)
        {
            token.ProfileId = profileID;
            _context.Tokens.Add(token);
            await _context.SaveChangesAsync();
            return CreatedAtAction("GetToken", new { id = token.Id }, token);
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
        [Produces("application/json")]
        public async Task<IActionResult> PutProfile(int id, [FromForm]Profile profiles)
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
                    return NotFound("Profile does not exist in the DB");
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