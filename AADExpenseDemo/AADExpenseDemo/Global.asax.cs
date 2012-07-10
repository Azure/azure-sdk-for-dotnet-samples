using System;
using System.Data.Entity;
using System.Web.Mvc;
using System.Web.Routing;
using Microsoft.Online.Demos.Aadexpense.ActionFilters;
using Microsoft.Online.Demos.Aadexpense.Models;


namespace Microsoft.Online.Demos.Aadexpense
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new IdentityClaimsAttribute());
            filters.Add(new RequireAuthenticatedAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            // Route for the Signup scenario, which puts the Global Administrator through the signup process
            // Allowed routes:
            // /singup/index -- this will provide instructions for how to use PowerShell to create a ServicePrincipal
            // /signup/authorize --- this will not be used until we have the capabiltly of an "opt-in" page from AAD

            routes.MapRoute(
                "Signup",
                "signup/{action}",
                new {controller = "Signup", action = "index"}
                );

            // Route for the Admin scenario, which allows the Global Administrator to add authorized users to the signup process
            // Allowed routes:
            // /admin/list
            // /admin/adduser
            // /admin/removeuser


            routes.MapRoute(
                "Admin",
                "admin/{action}/{employee}",
                new {controller = "Admin", action = "index", employee = UrlParameter.Optional }
                );

            routes.MapRoute(
                "Invoice",
                "invoice/{action}/{id}",
                new {  controller = "Invoice", action="index", id = UrlParameter.Optional }

    );
            
            // Default route
            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Home", action = "Index", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
           
            Database.SetInitializer(new DropCreateDatabaseIfModelChanges<DemoDatabase>());
            //FederatedAuthentication.ServiceConfigurationCreated += OnServiceConfigurationCreated;
            AreaRegistration.RegisterAllAreas();

            ModelBinders.Binders.Add(typeof(decimal), new MoneyModelBinder());
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
        }

        protected void Application_End(object sender, EventArgs e)
        {
            //  Code that runs on application shutdown

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            // Code that runs when an unhandled error occurs

        }

        protected void Session_Start(object sender, EventArgs e)
        {
            // Code that runs when a new session is started

        }

        protected void Session_End(object sender, EventArgs e)
        {
            // Code that runs when a session ends. 
            // Note: The Session_End event is raised only when the sessionstate mode
            // is set to InProc in the Web.config file. If session mode is set to StateServer 
            // or SQLServer, the event is not raised.

        }
    }
}