using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Event_system.Auth.Model;
using Event_system.Data;

namespace Event_system.Auth;

public static class AuthEndpoints
{
    public static void AddAuthApi(this WebApplication app)
    {
        //register
        app.MapPost("api/accounts", async (UserManager<EventUser> userManager, [FromBody] RegisterUserDto dto, EventDbContext _dbContext) =>
        {
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user != null) 
                return Results.UnprocessableEntity("Username already taken");

            var newUser = new EventUser()
            {
                Email = dto.Email,
                UserName = dto.UserName,
            };

            using var transaction = await _dbContext.Database.BeginTransactionAsync();

            var createUserResult = await userManager.CreateAsync(newUser, dto.Password);
            if (!createUserResult.Succeeded)
            {
                var errors = createUserResult.Errors.Select(e => e.Description).ToList();
                return Results.UnprocessableEntity(new { Errors = errors });
            }

            var roleResult = await userManager.AddToRoleAsync(newUser, EventRoles.EventUser);
            if (!roleResult.Succeeded)
            {
                await transaction.RollbackAsync(); // Rollback in case role assignment fails
                var errors = roleResult.Errors.Select(e => e.Description).ToList();
                return Results.UnprocessableEntity(new { Errors = errors });
            }

            await transaction.CommitAsync();
            return Results.Created("api/login", new UserDto(newUser.Id, newUser.UserName, newUser.Email));
        });
        
        //login
        app.MapPost("api/login", async ( UserManager<EventUser> userManager, JwtTokenService jwtTokenService, 
            SessionService sessionService, HttpContext httpContext, [FromBody] LoginDto dto)=>
        {
            var user = await userManager.FindByNameAsync(dto.UserName);
            if (user == null) 
                return Results.UnprocessableEntity("Username does not exist");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if(!isPasswordValid)
                return Results.UnprocessableEntity("Username or password was incorrect.");

            var roles = await userManager.GetRolesAsync(user);

            var sessionId = Guid.NewGuid();
            var expiresAt = DateTime.UtcNow.AddDays(3);
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var refreshToken = jwtTokenService.CreateRefreshToken(sessionId, user.Id, expiresAt);

            await sessionService.CreateSessionAsync(sessionId, user.Id, refreshToken, expiresAt);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                Secure = false // for learning purposes
            };
            
            httpContext.Response.Cookies.Append("RefreshToken", refreshToken, cookieOptions);

            return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken));
        });

        app.MapPost("api/accessToken", async (UserManager<EventUser> userManager,JwtTokenService jwtTokenService, 
            SessionService sessionService, HttpContext httpContext) =>
        {
            if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return Results.UnprocessableEntity("1");
            }

            if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
            {
                return Results.UnprocessableEntity("2");
            }

            var sessionId = claims.FindFirstValue("SessionId");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return Results.UnprocessableEntity("3");
            }
            
            var sessionIdAsGuid = Guid.Parse(sessionId);
            if (!await sessionService.IsSessionValidAsync(sessionIdAsGuid, refreshToken))
            {
                return Results.UnprocessableEntity("4");
            }
            
            var userId = claims.FindFirstValue(JwtRegisteredClaimNames.Sub);
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.UnprocessableEntity("5");
            }
            
            var roles = await userManager.GetRolesAsync(user);
            
            var expiresAt = DateTime.UtcNow.AddDays(3);
            var accessToken = jwtTokenService.CreateAccessToken(user.UserName, user.Id, roles);
            var newRefreshToken = jwtTokenService.CreateRefreshToken(sessionIdAsGuid, user.Id, expiresAt);
            
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                SameSite = SameSiteMode.Lax,
                Expires = expiresAt,
                Secure = false // for learning purposes
            };
            
            httpContext.Response.Cookies.Append("RefreshToken", newRefreshToken, cookieOptions);
            
            await sessionService.ExtendSessionAsync(sessionIdAsGuid, newRefreshToken, expiresAt);
            
            return Results.Ok(new SuccessfulLoginDto(accessToken, refreshToken));
        });
        
        app.MapPost("api/logout", async (UserManager<EventUser> userManager,JwtTokenService jwtTokenService, SessionService sessionService, HttpContext httpContext) =>
        {
            if (!httpContext.Request.Cookies.TryGetValue("RefreshToken", out var refreshToken))
            {
                return Results.UnprocessableEntity();
            }

            if (!jwtTokenService.TryParseRefreshToken(refreshToken, out var claims))
            {
                return Results.UnprocessableEntity();
            }

            var sessionId = claims.FindFirstValue("SessionId");
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                return Results.UnprocessableEntity();
            }
            
            await sessionService.InvalidateSessionAsync(Guid.Parse(sessionId));
            httpContext.Response.Cookies.Delete("RefreshToken");
            
            return Results.Ok();
        });
    }
    
    public record RegisterUserDto(string UserName, string Email, string Password);
    public record UserDto(string UserId, string Username, string Email);
    public record LoginDto(string UserName, string Password);

    public record SuccessfulLoginDto(string AccessToken, string RefreshToken);
}