<%@ Application Language="C#" %>

<script runat="server">

    /*
    * MagicBRIX - A Web Application Framework for ASP.NET
    * Copyright 2010 - Ra-Software, Inc. - info@rasoftwarefactory.com
    * MagicBRIX is licensed as GPLv3.
    */

    void Application_Start(object sender, EventArgs e) 
    {
        Magix.Brix.Loader.AssemblyResourceProvider sampleProvider = 
            new Magix.Brix.Loader.AssemblyResourceProvider();
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
    }
       
</script>
