using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using Xunit;

namespace Rest_SikkerApi.Tests;

public class FirebaseAuthIntegrationTests
{
    private readonly HttpClient _client = new();

    [Fact]
    public async Task Login_WithFirebaseToken_CanAccessProtectedBackendEndpoint()
    {
        // Arrange
        var firebaseApiKey = "AIzaSyAhfN8fZnZnT8NZKjxSp3MdUO06UqLXM8U";
        var backendBaseUrl = "http://localhost:5180/";
        var email = "test@test.pas123456.dk";
        var password = "123456";

        var firebaseLoginUrl =
            $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={firebaseApiKey}";

        var loginRequest = new
        {
            email,
            password,
            returnSecureToken = true
        };

        // Act 1: Login to Firebase and get token
        var firebaseResponse = await _client.PostAsJsonAsync(firebaseLoginUrl, loginRequest);

        Assert.Equal(HttpStatusCode.OK, firebaseResponse.StatusCode);

        var loginResult = await firebaseResponse.Content.ReadFromJsonAsync<FirebaseLoginResponse>();

        Assert.NotNull(loginResult);
        Assert.False(string.IsNullOrWhiteSpace(loginResult.IdToken));

        // Act 2: Send token to backend
        var backendClient = new HttpClient
        {
            BaseAddress = new Uri(backendBaseUrl)
        };

        backendClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", loginResult.IdToken);

        var backendResponse = await backendClient.GetAsync("api/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, backendResponse.StatusCode);

        var backendResult = await backendResponse.Content.ReadFromJsonAsync<AuthMeResponse>();

        Assert.NotNull(backendResult);
        Assert.Equal(email, backendResult.Email);
        Assert.False(string.IsNullOrWhiteSpace(backendResult.FirebaseUid));
    }

    private sealed class FirebaseLoginResponse
    {
        public string IdToken { get; set; } = string.Empty;
    }

    private sealed class AuthMeResponse
    {
        public string FirebaseUid { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
}