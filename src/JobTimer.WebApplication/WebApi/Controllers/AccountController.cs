using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using Common.Logging;
using JobTimer.Data.Access.Identity;
using JobTimer.Data.Model.Identity;
using JobTimer.WebApplication.Code;
using JobTimer.WebApplication.Hubs;
using JobTimer.WebApplication.Hubs.Models.Notification;
using JobTimer.WebApplication.Security;
using JobTimer.WebApplication.Security.Results;
using JobTimer.WebApplication.ViewModels.WebApi.Account.BindingModels;
using JobTimer.WebApplication.ViewModels.WebApi.Account.ViewModels;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OAuth;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JobTimer.WebApplication.WebApi.Controllers
{
    public class AccountController : BaseApiController
    {
        private ILog Log { get { return LogManager.GetLogger(this.GetType()); } }
        readonly IRequestReader _requestReader;
        readonly IAuthAccess _authAccess;

        public AccountController(IRequestReader requestReader, IAuthAccess authAccess)
        {
            _requestReader = requestReader;
            _authAccess = authAccess;
        }

        private IAuthenticationManager Authentication => Request.GetOwinContext().Authentication;

        [Authorize]
        public IHttpActionResult Logout()
        {
            var result = new LogoutViewModel();
            Authentication.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            result.Result = true;

            if (result.Result)
            {
                var notificationHub = GetHub<NotificationHub, INotificationHub>();
                var connectionId = _requestReader.Read(TypeScript.HttpHeaders.Request.SignalRConnectionId);

                if (!string.IsNullOrEmpty(connectionId))
                {
                    notificationHub.Clients.AllExcept(connectionId).UpdateModel(new NotificationModel()
                    {
                        Username = UserName,
                        Action = string.Format("È Offline!")
                    });
                }
            }

            return Ok(result);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IHttpActionResult> Login(LoginBindingModel model)
        {
            var result = new LoginViewModel();
            var signResult = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: true);
            switch (signResult)
            {
                case SignInStatus.Success:
                    result.Result = true;

                    var notificationHub = GetHub<NotificationHub, INotificationHub>();
                    var connectionId = _requestReader.Read(TypeScript.HttpHeaders.Request.SignalRConnectionId);

                    if (!string.IsNullOrEmpty(connectionId))
                    {
                        notificationHub.Clients.AllExcept(connectionId).UpdateModel(new NotificationModel()
                        {
                            Username = model.Email,
                            Action = string.Format("Is back online!")
                        });
                    }

                    return Ok(result);
                case SignInStatus.LockedOut:
                    result.Message = "Locked Out";
                    result.Result = false;
                    return Ok(result);
                case SignInStatus.RequiresVerification:
                    result.Message = "Requires Verification";
                    result.Result = false;
                    return Ok(result);
                case SignInStatus.Failure:
                    result.Result = false;
                    result.Message = "Wrong password or not existing email";
                    return Ok(result);
                default:
                    result.Result = false;
                    result.Message = "Error";
                    return Ok(result);
            }
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {
            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            var identityResult = await _authAccess.CreateAsync(user, model.Password);

            if (!identityResult.Succeeded)
            {
                return GetErrorResult(identityResult);
            }

            var roleResult = await _authAccess.AddToRoleAsync(user, "TimerUser");

            if (!roleResult.Succeeded)
            {
                return GetErrorResult(identityResult);
            }

            var signResult = await SignInManager.PasswordSignInAsync(model.Email, model.Password, true, shouldLockout: true);

            if (signResult != SignInStatus.Success)
            {
                return BadRequest("Sign in failed");
            }

            RegisterViewModel result = new RegisterViewModel { Result = true };

            Log.Info(x => x("User created: {0}", user.UserName));
            return Ok(result);
        }

        #region oauth
        [HttpGet]
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        public async Task<IHttpActionResult> ExternalLogin(string provider, string error = null)
        {
            string redirectUri = string.Empty;

            if (error != null)
            {
                return BadRequest(Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            var redirectUriValidationResult = ValidateClientAndRedirectUri(this.Request, ref redirectUri);

            if (!string.IsNullOrWhiteSpace(redirectUriValidationResult))
            {
                return BadRequest(redirectUriValidationResult);
            }

            var externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            var user = await _authAccess.FindAsync(new UserLoginInfo(externalLogin.LoginProvider, externalLogin.ProviderKey));
            var hasRegistered = user != null;

            string externalEmail;
            if (!hasRegistered)
            {
                externalEmail = externalLogin.Email;

                if (string.IsNullOrEmpty(externalEmail))
                {
                    externalEmail = await TryGetEmail(externalLogin.LoginProvider, externalLogin.ExternalAccessToken);
                }
            }
            else
            {
                externalEmail = user.Email;
            }

            redirectUri = string.Format("{0}#external_access_token={1}&provider={2}&haslocalaccount={3}&external_email={4}",
                                            redirectUri,
                                            externalLogin.ExternalAccessToken,
                                            externalLogin.LoginProvider,
                                            hasRegistered,
                                            externalEmail);

            return Redirect(redirectUri);
        }

        [AllowAnonymous]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            var result = new RegisterExternalViewModel();

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            var user = await _authAccess.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                return BadRequest("External user is already registered");
            }

            user = new ApplicationUser() { UserName = string.Format("{0}_{1}", model.Provider, model.Email), Email = model.Email };

            var userResult = await _authAccess.CreateAsync(user);

            if (!userResult.Succeeded)
            {
                return GetErrorResult(userResult);
            }

            var roleResult = await _authAccess.AddToRoleAsync(user, "TimerUser");
            if (!roleResult.Succeeded)
            {
                return GetErrorResult(roleResult);
            }

            var info = new ExternalLoginInfo()
            {
                Email = model.Email,
                DefaultUserName = user.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            var loginResult = await _authAccess.AddLoginAsync(user.Id, info.Login);
            if (!loginResult.Succeeded)
            {
                return GetErrorResult(loginResult);
            }

            var signResult = await SignInManager.ExternalSignInAsync(info, false);

            if (signResult != SignInStatus.Success)
            {
                return BadRequest("Sign in failed");
            }

            result.Result = true;

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IHttpActionResult> VerifyExternalLogin(VerifyExternalLoginBindingModel model)
        {
            var result = new VerifyExternalLoginViewModel();
            var verifiedAccessToken = await VerifyExternalAccessToken(model.Provider, model.ExternalAccessToken);
            if (verifiedAccessToken == null)
            {
                return BadRequest("Invalid Provider or External Access Token");
            }

            var user = await _authAccess.FindAsync(new UserLoginInfo(model.Provider, verifiedAccessToken.user_id));

            bool hasRegistered = user != null;

            if (!hasRegistered)
            {
                return BadRequest("External user is not registered");
            }

            var info = new ExternalLoginInfo()
            {
                Email = user.Email,
                DefaultUserName = user.UserName,
                Login = new UserLoginInfo(model.Provider, verifiedAccessToken.user_id)
            };

            var signResult = await SignInManager.ExternalSignInAsync(info, false);

            if (signResult != SignInStatus.Success)
            {
                return BadRequest("Sign in failed");
            }

            result.Result = true;

            return Ok(result);

        }

        #region helper
        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null && (result.Errors != null || !result.Errors.Any()))
                {
                    return BadRequest(result.Errors.First());
                }
                else
                {
                    return BadRequest();
                }
            }

            return null;
        }
        private class ParsedExternalAccessToken
        {
            public string user_id { get; set; }
            public string app_id { get; set; }
        }

        private string ValidateClientAndRedirectUri(HttpRequestMessage request, ref string redirectUriOutput)
        {
            Uri redirectUri;

            var redirectUriString = GetQueryString(request, "redirect_uri");

            if (string.IsNullOrWhiteSpace(redirectUriString))
            {
                return "redirect_uri is required";
            }

            bool validUri = Uri.TryCreate(redirectUriString, UriKind.Absolute, out redirectUri);

            if (!validUri)
            {
                return "redirect_uri is invalid";
            }

            var clientId = GetQueryString(request, "client_id");

            if (string.IsNullOrWhiteSpace(clientId))
            {
                return "client_Id is required";
            }

            var client = _authAccess.FindClient(clientId);

            if (client == null)
            {
                return string.Format("Client_id '{0}' is not registered in the system.", clientId);
            }

            if (!client.Active)
            {
                return string.Format("Client_id '{0}' is not active.", clientId);
            }

            if (client.AllowedOrigin != "*" && !string.Equals(client.AllowedOrigin, redirectUri.GetLeftPart(UriPartial.Authority), StringComparison.OrdinalIgnoreCase))
            {
                return string.Format("The given URL is not allowed by Client_id '{0}' configuration.", clientId);
            }

            redirectUriOutput = redirectUri.AbsoluteUri;

            return string.Empty;
        }
        private string GetQueryString(HttpRequestMessage request, string key)
        {
            var queryStrings = request.GetQueryNameValuePairs();

            if (queryStrings == null) return null;

            var match = queryStrings.FirstOrDefault(keyValue => String.Compare(keyValue.Key, key, StringComparison.OrdinalIgnoreCase) == 0);

            if (string.IsNullOrEmpty(match.Value)) return null;

            return match.Value;
        }
        private async Task<string> TryGetEmail(string provider, string accessToken)
        {
            var email = "";

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/me?fields=email&access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)JsonConvert.DeserializeObject(content);

                if (provider == "Facebook")
                {
                    email = jObj["email"];
                }
            }
            return email;
        }
        private async Task<ParsedExternalAccessToken> VerifyExternalAccessToken(string provider, string accessToken)
        {
            ParsedExternalAccessToken parsedToken = null;

            var verifyTokenEndPoint = "";

            if (provider == "Facebook")
            {
                verifyTokenEndPoint = string.Format("https://graph.facebook.com/debug_token?input_token={0}&access_token={1}", accessToken, SecretKeys.Facebook.AppToken);
            }
            else if (provider == "Google")
            {
                verifyTokenEndPoint = string.Format("https://www.googleapis.com/oauth2/v1/tokeninfo?access_token={0}", accessToken);
            }
            else
            {
                return null;
            }

            var client = new HttpClient();
            var uri = new Uri(verifyTokenEndPoint);
            var response = await client.GetAsync(uri);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();

                dynamic jObj = (JObject)JsonConvert.DeserializeObject(content);

                parsedToken = new ParsedExternalAccessToken();

                if (provider == "Facebook")
                {
                    parsedToken.user_id = jObj["data"]["user_id"];
                    parsedToken.app_id = jObj["data"]["app_id"];

                    if (!string.Equals(SecretKeys.Facebook.AppId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }
                }
                else if (provider == "Google")
                {
                    parsedToken.user_id = jObj["user_id"];
                    parsedToken.app_id = jObj["audience"];

                    if (!string.Equals(SecretKeys.Google.ClientId, parsedToken.app_id, StringComparison.OrdinalIgnoreCase))
                    {
                        return null;
                    }

                }

            }

            return parsedToken;
        }
        #endregion
        #endregion
    }
}