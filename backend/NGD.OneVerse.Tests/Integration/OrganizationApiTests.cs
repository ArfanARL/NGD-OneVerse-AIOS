using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Tests.Integration;

public class OrganizationApiTests
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public OrganizationApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetOrganizations_ShouldReturnSuccess()
    {
        var response = await _client.GetAsync("/api/Organizations");

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreateOrganization_ShouldReturnCreated()
    {
        var organization = new Organization
        {
            Name = "Integration Test Organization",
            Code = $"TEST-{Guid.NewGuid():N}",
            Description = "Created by automated integration test",
            IsActive = true
        };

        var response = await _client.PostAsJsonAsync(
            "/api/Organizations",
            organization);

        Assert.Equal(
            HttpStatusCode.Created,
            response.StatusCode);

        var created =
            await response.Content.ReadFromJsonAsync<Organization>();

        Assert.NotNull(created);
        Assert.NotEqual(Guid.Empty, created.Id);
        Assert.Equal(
            organization.Name,
            created.Name);
        Assert.Equal(
            organization.Code,
            created.Code);
    }
}