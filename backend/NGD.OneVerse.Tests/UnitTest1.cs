using NGD.OneVerse.Domain.Entities;

namespace NGD.OneVerse.Tests;

public class OrganizationTests
{
    [Fact]
    public void Organization_ShouldHaveDefaultActiveState()
    {
        var organization = new Organization();

        Assert.True(organization.IsActive);
    }

    [Fact]
    public void Organization_ShouldGenerateCreatedAtByDefault()
    {
        var before = DateTime.UtcNow;

        var organization = new Organization();

        var after = DateTime.UtcNow;

        Assert.InRange(
            organization.CreatedAt,
            before,
            after);
    }

    [Fact]
    public void Organization_ShouldStoreBasicInformation()
    {
        var organization = new Organization
        {
            Id = Guid.NewGuid(),
            Name = "NGD Head Office",
            Code = "NGD-HQ",
            Description = "Neural Grid Dynamics Head Office",
            IsActive = true
        };

        Assert.Equal("NGD Head Office", organization.Name);
        Assert.Equal("NGD-HQ", organization.Code);
        Assert.Equal(
            "Neural Grid Dynamics Head Office",
            organization.Description);
        Assert.True(organization.IsActive);
        Assert.NotEqual(Guid.Empty, organization.Id);
    }
}