# JWT requirement OWIN Middleware 

Enables APIs to set requirements on the contents of JWTs; for instance that they include certain claims; like `sub` or `client_id`.


# Usage:

```
	
	Func<IEnumerable<Claim>, bool> bananaFunc = c => c.Any(c => c.Type == "Banana");
	var errorMsgWhenNotFound = "No banana claim!";
	var bananaRequirement = new ClaimRequirement(bananaFunc, errorMsgWhenNotFound);
	
	var reqOpts = new RequireClaimsInJwtOptions();
	reqOpts.AddRequirement(bananaRequirement);
	
	api.UseRequireClaimsInJwt>(reqOpts);
```