using Xunit;

public class AuthorizationTests
{
    [Fact]
    public void Viewer_ShouldNotHaveEditAccess()
    {
        var role = "Viewer";

        bool canEdit = role == "Admin" || role == "Inspector";

        Assert.False(canEdit);
    }

    [Fact]
    public void Inspector_ShouldHaveCreateAccess()
    {
        var role = "Inspector";

        bool canCreate = role == "Admin" || role == "Inspector";

        Assert.True(canCreate);
    }
}