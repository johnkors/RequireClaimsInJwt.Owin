[![NuGet](https://img.shields.io/nuget/v/RequireClaimsInJwt.Owin.svg)](https://img.shields.io/nuget/v/RequireClaimsInJwt.Owin.svg)


# RequireClaimsInJwt.Owin 
.. a JWT requirement OWIN Middleware.

Enables APIs to set requirements on the contents of JWTs; for instance that they include certain claims; like `sub` or `client_id`.


# Usage:

Require all incoming JWTs to have at least one claim named "Banana":
```
public void Configuration(IAppBuilder app)
{	
	Func<IEnumerable<Claim>, bool> bananaFunc = all => all.Any(c => c.Type == "Banana");
	var errorMsgWhenNotFound = "No banana claim!";
	var bananaRequirement = new ClaimRequirement(bananaFunc, errorMsgWhenNotFound);
	
	var reqOpts = new RequireClaimsInJwtOptions();
	reqOpts.AddRequirement(bananaRequirement);	
	
	app.UseRequireClaimsInJwt(reqOpts);	
}	
```

If any requirement fails, it will return a 403 response with the specified error message in a header.