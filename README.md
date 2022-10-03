<h3 align="center">
<img height="100" src="https://raw.githubusercontent.com/Infocaster/.github/main/assets/infocaster_nuget_pink.svg">
</h3>

<h1 align="center">
Umbraco ETag

[![Downloads](https://img.shields.io/nuget/dt/Infocaster.Umbraco.ETag?color=ff0069)](https://www.nuget.org/packages/Infocaster.Umbraco.ETag/)
[![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Infocaster.Umbraco.ETag?color=ffc800)](https://www.nuget.org/packages/Infocaster.Umbraco.ETag/)
![GitHub](https://img.shields.io/github/license/Infocaster/Infocaster.Umbraco.ETag?color=ff0069)

</h1>

Boost your website performance with ETag headers. An ETag header will increase cachability of your content and makes browsers reuse cached content if it hasn't changed.

## Getting Started
You can use the ETag package in two different ways:

1) Opt-in on specific Umbraco pages:
   ```csharp
   using Infocaster.Umbraco.ETag

   [ETag]
   public class HomeController : RenderController
   {
	   
   }
   ```
   Use this method if you only want to use the ETag on specific pages
2) On all Umbraco pages:
   ```csharp
   public class Startup
   {
	   public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
       {
		   app.UseUmbraco()
                .WithMiddleware(u =>
                {
                    u.UseBackOffice();
                    u.UseWebsite();

                    u.UseETag();
                })
	   }
   }
   ```
   Use this method if you want to use ETag on all pages

Do not use both methods at the same time

## Contributing
This package is open for contributions. If you want to contribute to the source code, please check out our [guide to contributing](/docs/CONTRIBUTING.md).  
These people have already contributed to this awesome project:

<a href="https://github.com/Infocaster/ETag/graphs/contributors">
<img src="https://contrib.rocks/image?repo=Infocaster/ETag" />
</a>

*Made with [contributors-img](https://contrib.rocks).*

<a href="https://infocaster.net">
<img align="right" height="200" src="https://raw.githubusercontent.com/Infocaster/.github/main/assets/Infocaster_Corner.png?raw=true">
</a>
