using Rest_SikkerApi.interfaces;

namespace Rest_SikkerApi;

public class FirebaseHandler : IFirebaseHandler
{
	private readonly IHttpContextAccessor _httpContextAccessor;

	public FirebaseHandler(IHttpContextAccessor httpContextAccessor)
	{
		_httpContextAccessor = httpContextAccessor;
	}

	public async Task<string> GetFirebaseUid()
	{
		// tilgiver FirebaseUid i både HttpContext og User.Claims for at sikre kompatibilitet med forskellige autentificeringsmetoder
		// var firebaseUid = HttpContext.Items["FirebaseUid"] as string
		// 	?? User.FindFirst("firebase_uid")?.Value
		// 	// ?? image.OwnerUid
		// 	?? string.Empty;
		// return firebaseUid;

		var httpContext = _httpContextAccessor.HttpContext
			?? throw new InvalidOperationException("no active http context");
		var firebaseUid = httpContext.Items["firebaseUid"] as string
			?? httpContext.User?.FindFirst("firebase_uid")?.Value ?? string.Empty;
		
		return firebaseUid;
	}
}
