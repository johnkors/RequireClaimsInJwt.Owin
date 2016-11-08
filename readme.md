# JWT requirement OWIN Middleware 

Enables APIs to set requirements on the contents of JWTs; for instance that they include certain claims; like `sub` or `client_id`.


# Usage:

```
	var reqOpts = new RequireClaimsInJwtOptions();
	reqOpts.Requirements.Add(new ClaimRequirement(allClaims => allClaims.Any(c => c.Type == "Banana"),  "No banana claim!"));
	api.UseRequireClaimsInJwt>(reqOpts);
```