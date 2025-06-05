// using DocShareApi.Interfaces;
// using Microsoft.AspNetCore.Authentication;
// using Microsoft.AspNetCore.Authentication.Cookies;
// using Microsoft.AspNetCore.Authentication.Google;
// using Microsoft.AspNetCore.Mvc;

// namespace DocShareApi.Controllers
// {
//     [Route("api/[controller]")]
//     [ApiController]
//     public class GoogleAuthController : ControllerBase
//     {
//         private readonly IGoogleAuthService _googleAuthService;
 
//         public GoogleAuthController(IGoogleAuthService googleAuthService)
//         {
//             _googleAuthService = googleAuthService;
//         }
 
//         [HttpGet]
//         public async Task<IActionResult> LoginWithGoogle()
//         {
//             await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
 
//             var properties = new AuthenticationProperties { RedirectUri = "api/GoogleAuth/google-callback" };
//             properties.Items["prompt"] = "select_account";
 
//             return Challenge(properties, GoogleDefaults.AuthenticationScheme);
//         }
 
//         [HttpGet("google-callback")]
//         public async Task<IActionResult> GoogleCallback()
//         {
//             AuthenticateResult result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
 
//             if (!result.Succeeded)
//                 return Unauthorized();
 
 
//             var claims = _googleAuthService.AuthenticateUser(result);
 
//             return Ok(claims);
//         }
 
 
//     }
// }
 
