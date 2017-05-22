using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.ModelBinding;
using Microsoft.AspNet.Identity;
using AspNet.Identity.MySQL;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using api2.Models;
using api2.Providers;
using api2.Results;
using System.Linq;
using System.Web.Http.Cors;


using System.Globalization;
using System.Web.Security;
using Microsoft.Owin.Cors;


namespace api2.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]
    [Authorize]
    [RoutePrefix("api/Account")]


    public class AccountController : ApiController
    {
        
        private apausrEntities3 db = new apausrEntities3();
        private apiapaEntities db2 = new apiapaEntities();
        private rolsEntities db3 = new rolsEntities();
        private apaEntities1 db4 = new apaEntities1();

        private const string LocalLoginProvider = "Local";
        private ApplicationUserManager _userManager;

        public AccountController()
        {
            
        }

        public AccountController(ApplicationUserManager userManager,
            ISecureDataFormat<AuthenticationTicket> accessTokenFormat)
        {
            
            
            UserManager = userManager;
            AccessTokenFormat = accessTokenFormat;
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? Request.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        public ISecureDataFormat<AuthenticationTicket> AccessTokenFormat { get; private set; }

        // GET api/Account/UserInfo
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("UserInfo")]
        public UserInfoViewModel GetUserInfo()
        {
            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            return new UserInfoViewModel
            {
                Email = User.Identity.GetUserName(),
                HasRegistered = externalLogin == null,
                LoginProvider = externalLogin != null ? externalLogin.LoginProvider : null
            };
        }

        /// <summary>
        /// Con el id de usuario (no de cliente) devuelve los roles que tiene
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET /api/Account/Rol
        [AllowAnonymous]
        [Route("Rol/{id}")]
        public IHttpActionResult Get(string id)
        {
            var existe = from UserRoles in db3.UserRoles
                         where UserRoles.UserId == id
                         select UserRoles.RoleId;
            if (existe.ToList().Count() > 0)
            {
                var rol_final = from Roles in db3.Roles
                                where Roles.Id == existe.FirstOrDefault()
                                select  new { Roles.Id, Roles.Name } ;

                if (rol_final.ToList().Count() > 0)
                {
                    return Ok(rol_final);
                }
                else
                {
                    return NotFound();
                }
                               
            }
            else
            {
                return NotFound();
            }
        }

        // POST api/Account/Logout
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            Authentication.SignOut(CookieAuthenticationDefaults.AuthenticationType);
            return Ok();
        }

        // POST api/Account/ChangePassword
        //[AllowAnonymous]
        [Route("ChangePassword")]
        public async Task<IHttpActionResult> ChangePassword(ChangePasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword,
                model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }

        // POST api/Account/SetPassword
        [Route("SetPassword")]
        public async Task<IHttpActionResult> SetPassword(SetPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        
        /// <summary>
        /// Esta api permite cambiar el password de un usuario, recibe email, newpassword y confirmpassword
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("NewPassword")]
        public async Task<IHttpActionResult> NewPassword(NewPasswordBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id_usr = from Users in db.Users
                         where Users.Email == model.Email
                         select Users.Id;
            var id_usr_real = id_usr.FirstOrDefault();

            Users uSERS = db.Users.Find(id_usr_real);
            uSERS.PasswordHash = "";
            db.Entry(uSERS).State = EntityState.Modified;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            IdentityResult result = await UserManager.AddPasswordAsync(id_usr_real, model.NewPassword);

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        //fin cambiar pass

        //////Negrolins code

        //
        // POST: /Account/ResetPassword
        [HttpPost]
        [AllowAnonymous]
        //[ValidateAntiForgeryToken]
        [Route("ResetPassword", Name = "ResetPasswordRoute")]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return Ok("ERROR ModelState");
            }
            var user = await UserManager.FindByNameAsync(model.Email);
            //////if (user == null)
            //////{
            //////    // Don't reveal that the user does not exist
            //////    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //////}
            var result = await UserManager.ResetPasswordAsync(user.Id, model.Code, model.Password);
            return Ok("Todo bien");
            //////if (result.Succeeded)
            //////{
            //////    return RedirectToAction("ResetPasswordConfirmation", "Account");
            //////}
            //////AddErrors(result);
            //////return View();
        }



        public class ForgotPasswordViewModel
        {
            public string Email { get; set; }
        }
        
        //
        // POST: /Account/ForgotPassword

        //////SE USA ASÍ:
        //////{
        //////"email": "hejasako@gmail.com"
        //////}

        [HttpPost]
        [AllowAnonymous]
        [Route("ForgotPassword")]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await UserManager.FindByNameAsync(model.Email);
                // If user has to activate his email to confirm his account, the use code listing below
                if (user == null || !(await UserManager.IsEmailConfirmedAsync(user.Id)))
                {
                    return Ok();
                }
                if (user == null)


                {
                    return Ok();
                }

                //////// Send an email with this link
                //////string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //////var callbackUrl = new Uri(Url.Link("ResetPasswordRoute", new { email = user.Email, code = code }));
                //////await UserManager.SendEmailAsync(user.Id, "Resetear Password", "Por favor, resetea tu password usando esto: <a href=\"" + callbackUrl + "\">here</a>");

                // Send an email with this link
                string code = await UserManager.GeneratePasswordResetTokenAsync(user.Id);
                //var callbackUrl = new Uri(Url.Link("ResetPasswordRoute", new { email = user.Email, code = code }));
                await UserManager.SendEmailAsync(user.Id, "Resetear Password", "Por favor, resetea tu password usando esto: <a href=\"http://192.168.0.25/wordpress/reset/?email=" + user.Email + "&code=" + code + "\">here</a>");


                //await UserManager.SendEmailAsync(user.Id, "Resetear Password", $"Por favor, resetea tu password usando esto {code}");

                //////    var callbackUrl = new Uri(Url.Link("ConfirmEmailRoute", new { UserId = user.Id, code = code }));
                //////    await UserManager.SendEmailAsync(user.Id, "Reset Password",
                //////"Please reset your password by clicking here: <a href=\"" + callbackUrl + "\">link</a>");


                return Ok();
            }

            // If we got this far, something failed, redisplay form
            return BadRequest(ModelState);
        }
        //////    Negrolins code

        /*
        // POST api/Account/AddExternalLogin
        [Route("AddExternalLogin")]
        public async Task<IHttpActionResult> AddExternalLogin(AddExternalLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

            AuthenticationTicket ticket = AccessTokenFormat.Unprotect(model.ExternalAccessToken);

            if (ticket == null || ticket.Identity == null || (ticket.Properties != null
                && ticket.Properties.ExpiresUtc.HasValue
                && ticket.Properties.ExpiresUtc.Value < DateTimeOffset.UtcNow))
            {
                return BadRequest("External login failure.");
            }

            ExternalLoginData externalData = ExternalLoginData.FromIdentity(ticket.Identity);

            if (externalData == null)
            {
                return BadRequest("The external login is already associated with an account.");
            }

            IdentityResult result = await UserManager.AddLoginAsync(User.Identity.GetUserId(),
                new UserLoginInfo(externalData.LoginProvider, externalData.ProviderKey));

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        */
        // POST api/Account/RemoveLogin
        [Route("RemoveLogin")]
        public async Task<IHttpActionResult> RemoveLogin(RemoveLoginBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IdentityResult result;

            if (model.LoginProvider == LocalLoginProvider)
            {
                result = await UserManager.RemovePasswordAsync(User.Identity.GetUserId());
            }
            else
            {
                result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(),
                    new UserLoginInfo(model.LoginProvider, model.ProviderKey));
            }

            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            return Ok();
        }
        /*
        // GET api/Account/ExternalLogin
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalCookie)]
        [AllowAnonymous]
        [Route("ExternalLogin", Name = "ExternalLogin")]
        public async Task<IHttpActionResult> GetExternalLogin(string provider, string error = null)
        {
            if (error != null)
            {
                return Redirect(Url.Content("~/") + "#error=" + Uri.EscapeDataString(error));
            }

            if (!User.Identity.IsAuthenticated)
            {
                return new ChallengeResult(provider, this);
            }

            ExternalLoginData externalLogin = ExternalLoginData.FromIdentity(User.Identity as ClaimsIdentity);

            if (externalLogin == null)
            {
                return InternalServerError();
            }

            if (externalLogin.LoginProvider != provider)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                return new ChallengeResult(provider, this);
            }

            ApplicationUser user = await UserManager.FindAsync(new UserLoginInfo(externalLogin.LoginProvider,
                externalLogin.ProviderKey));

            bool hasRegistered = user != null;

            if (hasRegistered)
            {
                Authentication.SignOut(DefaultAuthenticationTypes.ExternalCookie);

                ClaimsIdentity oAuthIdentity = await user.GenerateUserIdentityAsync(UserManager,
                   OAuthDefaults.AuthenticationType);
                ClaimsIdentity cookieIdentity = await user.GenerateUserIdentityAsync(UserManager,
                    CookieAuthenticationDefaults.AuthenticationType);

                AuthenticationProperties properties = ApplicationOAuthProvider.CreateProperties(user.UserName);
                Authentication.SignIn(properties, oAuthIdentity, cookieIdentity);
            }
            else
            {
                IEnumerable<Claim> claims = externalLogin.GetClaims();
                ClaimsIdentity identity = new ClaimsIdentity(claims, OAuthDefaults.AuthenticationType);
                Authentication.SignIn(identity);
            }

            return Ok();
        }
        */
        /*
        // GET api/Account/ExternalLogins?returnUrl=%2F&generateState=true
        [AllowAnonymous]
        [Route("ExternalLogins")]
        public IEnumerable<ExternalLoginViewModel> GetExternalLogins(string returnUrl, bool generateState = false)
        {
            IEnumerable<AuthenticationDescription> descriptions = Authentication.GetExternalAuthenticationTypes();
            List<ExternalLoginViewModel> logins = new List<ExternalLoginViewModel>();

            string state;

            if (generateState)
            {
                const int strengthInBits = 256;
                state = RandomOAuthStateGenerator.Generate(strengthInBits);
            }
            else
            {
                state = null;
            }

            foreach (AuthenticationDescription description in descriptions)
            {
                ExternalLoginViewModel login = new ExternalLoginViewModel
                {
                    Name = description.Caption,
                    Url = Url.Route("ExternalLogin", new
                    {
                        provider = description.AuthenticationType,
                        response_type = "token",
                        client_id = Startup.PublicClientId,
                        redirect_uri = new Uri(Request.RequestUri, returnUrl).AbsoluteUri,
                        state = state
                    }),
                    State = state
                };
                logins.Add(login);
            }

            return logins;
        }
        */
        /// <summary>
        /// Permite cambiar la direccion de correo, recibe la cedula y el newmail
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [Route("changemail")]
        public async Task<IHttpActionResult> changemail(ChangeMailBindingModel model)
        {
            var old = from Users in db.Users
                      where Users.id_socio == model.id_socio
                      select Users.Id;
            if (old.ToList().Count() == 0)
            {
                return NotFound();
            }
            var nuevo = from Users in db.Users
                        where Users.Email == model.NewMail
                        select Users;
            if (nuevo.ToList().Count() > 0)
            {
                return BadRequest("Mail ya registrado");
            }

            Users uSERS = db.Users.Find(old.FirstOrDefault());
              
            if (uSERS == null)
            {
                return NotFound();
            }
            else
            {
                uSERS.Email = model.NewMail;
                //cambiar esto en el futuro por si se utilice nombre de usuario y mail diferenciado
                uSERS.UserName = model.NewMail;
                db.Entry(uSERS).State = EntityState.Modified;

                try
                {
                    db.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }
            }

            return Ok("Mail reemplazado con exito!");
        }

        //// GET: /Account/ConfirmEmail //NEGROLIN
        [AllowAnonymous]
        [HttpGet]
        [Route("ConfirmEmail", Name = "ConfirmEmailRoute")]
        public async Task<IHttpActionResult> ConfirmEmail(string userId = "", string code = "")
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(code))
            {
                ModelState.AddModelError("", "User Id and Code are required");
                return BadRequest(ModelState);
            }

            IdentityResult result = await UserManager.ConfirmEmailAsync(userId, code);

            if (result.Succeeded)
            {
                return Ok();
            }
            else
            {
                return GetErrorResult(result);
            }
        }

        //inicio eliminar usuario
        [Authorize(Roles = "Admin")]
        [Route("delete")]
        public IHttpActionResult Delete(DeleteUserBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var id_usr = from Users in db.Users
                         where Users.Email == model.Email
                         select Users.Id;
            var id_usr_real = id_usr.FirstOrDefault();

            Users uSERS = db.Users.Find(id_usr_real);
            if (uSERS == null)
            {
                return NotFound();
            }

            db.Users.Remove(uSERS);
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Ok();
        }
        //fin eliminar usuario
        // POST api/Account/Register
        [Authorize(Roles ="Admin")]
        [Route("Register")]
        public async Task<IHttpActionResult> Register(RegisterBindingModel model)
        {

            bool ban_cedula = false;
            bool ban_crear = false;
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            //condiciones de creacion
            //verificar si el socio existe
            if (model.Cedula == "" || model.id_socio != "")
            {
                if (model.Rol == "2")
                {
                    ban_cedula = false;
                    SOCIO sOCIO = db2.SOCIO.Find(model.id_socio);
                    if (sOCIO == null)
                    {
                        ban_crear = false;
                        return BadRequest("Id no pertenece a un socio");
                    }
                    else
                    {
                        ban_crear = true;
                    }
                }
                else if (model.Rol == "1")
                {
                    ban_crear = true;
                }
            }

            //verificar si el usuario ya esta creado
            var id_usr = from Users in db.Users
                         where Users.id_socio == model.id_socio
                         select Users.Id;
            
            if (ban_crear == true && model.Rol == "2")  //verifica el id_socio
            {
                
                if (id_usr.ToList().Count() == 0)
                {
                    ban_crear = true;
                }
                else
                {
                    ban_crear = false;
                    return BadRequest("ID ya registrado");
                }
            }

            if (ban_crear == true) //verifica el Email
            {
                 id_usr = from Users in db.Users
                             where Users.Email == model.Email
                             select Users.Id;
                if (id_usr.ToList().Count() == 0)
                {
                    ban_crear = true;
                }
                else
                {
                    ban_crear = false;
                    return BadRequest("Email ya registrado");
                }
            }
            //verifica si existe en el wordpress
            //db4
            //var hay_en_wp = from gk1hyugy_users in db4.gk1hyugy_users
            //                where gk1hyugy_users.user_email == model.Email
            //                select gk1hyugy_users.ID;
            //if(hay_en_wp.ToList().Count() == 0)
            //{
            //    //quiere decir que el mail fue rechazado en el wp por algun motivo
            //    ban_crear = false;

            //}
            //se verifica si el rol que se solicito existe
            var hay_rol = from Roles in db3.Roles
                          where Roles.Id == model.Rol
                          select Roles.Id;
            if (hay_rol.ToList().Count() == 0)
            {
                ban_crear = false;
                return BadRequest("Rol no existe");
            }

            //se procede a crear el usuario
            if (ban_crear == true)
            {
                var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };
                IdentityResult result = await UserManager.CreateAsync(user, model.Password);
                if (!result.Succeeded)
                {
                    return GetErrorResult(result);
                }
                id_usr = from Users in db.Users
                         where Users.Email == model.Email
                         select Users.Id;



                Users uSERS = db.Users.Find(id_usr.FirstOrDefault());
                SOCIO sOCIO = db2.SOCIO.Find(model.id_socio);

                
                if (model.Rol == "2")
                {
                    uSERS.id_socio = model.id_socio;
                    uSERS.Cedula = sOCIO.CEDULA;
                }
                
                UserRoles rol = new UserRoles();
                rol.UserId = uSERS.Id;
                rol.RoleId = model.Rol;

                db.Entry(uSERS).State = EntityState.Modified;
                db3.UserRoles.Add(rol);

                try
                {
                    db.SaveChanges();
                    db3.SaveChanges();
                }
                catch (DbUpdateConcurrencyException)
                {
                    throw;
                }

                return Ok("Creado exitosamente!");

            }

            //en caso de que no se quedo en ninguna condicion
            return BadRequest("Datos incorrectos");



        }
        /*
        // POST api/Account/RegisterExternal
        [OverrideAuthentication]
        [HostAuthentication(DefaultAuthenticationTypes.ExternalBearer)]
        [Route("RegisterExternal")]
        public async Task<IHttpActionResult> RegisterExternal(RegisterExternalBindingModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = await Authentication.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return InternalServerError();
            }

            var user = new ApplicationUser() { UserName = model.Email, Email = model.Email };

            IdentityResult result = await UserManager.CreateAsync(user);
            if (!result.Succeeded)
            {
                return GetErrorResult(result);
            }

            result = await UserManager.AddLoginAsync(user.Id, info.Login);
            if (!result.Succeeded)
            {
                return GetErrorResult(result); 
            }
            return Ok();
        }
        */
        protected override void Dispose(bool disposing)
        {
            if (disposing && _userManager != null)
            {
                _userManager.Dispose();
                _userManager = null;
            }

            base.Dispose(disposing);
        }

        #region Helpers

        private IAuthenticationManager Authentication
        {
            get { return Request.GetOwinContext().Authentication; }
        }

        private IHttpActionResult GetErrorResult(IdentityResult result)
        {
            if (result == null)
            {
                return InternalServerError();
            }

            if (!result.Succeeded)
            {
                if (result.Errors != null)
                {
                    foreach (string error in result.Errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                }

                if (ModelState.IsValid)
                {
                    // No ModelState errors are available to send, so just return an empty BadRequest.
                    return BadRequest();
                }

                return BadRequest(ModelState);
            }

            return null;
        }

        private class ExternalLoginData
        {
            public string LoginProvider { get; set; }
            public string ProviderKey { get; set; }
            public string UserName { get; set; }

            public IList<Claim> GetClaims()
            {
                IList<Claim> claims = new List<Claim>();
                claims.Add(new Claim(ClaimTypes.NameIdentifier, ProviderKey, null, LoginProvider));

                if (UserName != null)
                {
                    claims.Add(new Claim(ClaimTypes.Name, UserName, null, LoginProvider));
                }

                return claims;
            }

            public static ExternalLoginData FromIdentity(ClaimsIdentity identity)
            {
                if (identity == null)
                {
                    return null;
                }

                Claim providerKeyClaim = identity.FindFirst(ClaimTypes.NameIdentifier);

                if (providerKeyClaim == null || String.IsNullOrEmpty(providerKeyClaim.Issuer)
                    || String.IsNullOrEmpty(providerKeyClaim.Value))
                {
                    return null;
                }

                if (providerKeyClaim.Issuer == ClaimsIdentity.DefaultIssuer)
                {
                    return null;
                }

                return new ExternalLoginData
                {
                    LoginProvider = providerKeyClaim.Issuer,
                    ProviderKey = providerKeyClaim.Value,
                    UserName = identity.FindFirstValue(ClaimTypes.Name)
                };
            }
        }

        private static class RandomOAuthStateGenerator
        {
            private static RandomNumberGenerator _random = new RNGCryptoServiceProvider();

            public static string Generate(int strengthInBits)
            {
                const int bitsPerByte = 8;

                if (strengthInBits % bitsPerByte != 0)
                {
                    throw new ArgumentException("strengthInBits must be evenly divisible by 8.", "strengthInBits");
                }

                int strengthInBytes = strengthInBits / bitsPerByte;

                byte[] data = new byte[strengthInBytes];
                _random.GetBytes(data);
                return HttpServerUtility.UrlTokenEncode(data);
            }
        }

        #endregion
    }
}
