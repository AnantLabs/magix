<%@ Application Language="C#" %>

<script runat="server">
/*
 * Ra-Brix - A Modular-based Framework for building 
 * Web Applications Copyright 2009 - Thomas Hansen 
 * thomas@ra-ajax.org. Unless permission is 
 * explicitly given this code is licensed under the 
 * GNU Affero GPL version 3 which can be found in the 
 * license.txt file on disc.
 * 
 */
    void Application_Start(object sender, EventArgs e) 
    {
        Magic.Brix.Loader.AssemblyResourceProvider sampleProvider = new Magic.Brix.Loader.AssemblyResourceProvider();
        System.Web.Hosting.HostingEnvironment.RegisterVirtualPathProvider(sampleProvider);
    }
    
    void Application_End(object sender, EventArgs e)
    {
        //  Code that runs on application shutdown

    }
        
    void Application_Error(object sender, EventArgs e) 
    { 
        // Code that runs when an unhandled error occurs
    }

    void Application_BeginRequest(object sender, EventArgs e)
    {
    }

    void Application_AuthorizeRequest(object sender, EventArgs e)
    {
    }

    void Application_EndRequest(object sender, EventArgs e)
    {
    }
    
    void Session_Start(object sender, EventArgs e) 
    {
    }

    void Session_End(object sender, EventArgs e) 
    {
        // Code that runs when a session ends. 
        // Note: The Session_End event is raised only when the sessionstate mode
        // is set to InProc in the Web.config file. If session mode is set to StateServer 
        // or SQLServer, the event is not raised.
    }
       
</script>
