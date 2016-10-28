using System;
using System.Web;
using Domain;
using Repositories.DBContext;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

using Microsoft.AspNet.Identity.EntityFramework;

using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OAuth;
using Owin;
using Exceptionless;
using Exceptionless.Models;
using LS.WebApp.Utilities;
using System.Configuration;

namespace WebApp
{
    public partial class Startup
    {
        private static readonly string _loginPath = "/login";
        private static readonly string _apiActionBase = "~/api";
        private static readonly string _webapiActionBase = "~/webapi";
        //private static readonly TimeSpan _expirationTimeSpan = TimeSpan.FromMinutes(1);
        private static readonly TimeSpan _expirationTimeSpan = TimeSpan.FromDays(1);
        public static OAuthAuthorizationServerOptions OAuthOptions { get; private set; }


        public static string PublicClientId { get; private set; }

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void OnSubmittingEvent(Object sender, EventSubmittingEventArgs e)
        {
            // If we have turned off logging using the Client Config Key
            if (!e.Client.Configuration.Settings.GetBoolean("LogErrors", true))
            {
                e.Cancel = true;
                return;
            }

            if (!e.IsUnhandledError)
            {
                return;
            }

            // Ignore 404s
            //if (e.Event.IsNotFound()) {
            //    e.Cancel = true;
            //    return;
            //}
            // Ignore 401 (Unauthorized) and request validation errors.
            //if (error.Code == "401" || error.Type == "System.Web.HttpRequestValidationException") {
            //    e.Cancel = true;
            //    return;
            //}

            var error = e.Event.GetError();
            if (error == null) { return; }

            // Dont Log any 'TaskCanceledException in ThrowForNonSuccess A task was canceled.' errors. 
            // We think this is being caused by users closing browers or timeouts. Only see it happening to api/order/uploadBase64ProofImage
            if (error.Message == "A task was canceled." && error.Type == "System.Threading.Tasks.TaskCanceledException")
            {
                e.Cancel = true;
                return;
            }
        }


        public void ConfigureAuth(IAppBuilder app)
        {
           // ExceptionlessClient.Default.Configuration.SetVersion(ConfigurationManager.AppSettings["AppVersion"].ToString());
           
            ExceptionlessClient.Default.SubmittingEvent += OnSubmittingEvent;

            // Configure the db context and user manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationRoleManager>(ApplicationRoleManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString(_loginPath),
                SlidingExpiration = true,
                ExpireTimeSpan = _expirationTimeSpan,
                CookieSecure = CookieSecureOption.SameAsRequest,
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager, DefaultAuthenticationTypes.ApplicationCookie)),
                    OnApplyRedirect = ctx =>
                    {
                        if (!IsWebApiRequest())
                        {
                            ctx.Response.Redirect(ctx.RedirectUri);
                        }
                    }
                },

            });
        }

        private static bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null && (HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_apiActionBase) || HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(_webapiActionBase));
        }
    }
}
