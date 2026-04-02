using Xunit;
using oop_s2_2_mvc_78368.Models;

public class FollowUpTests
{
    [Fact]
    public void FollowUp_CannotCloseWithoutClosedDate()
    {
        var followUp = new FollowUp
        {
            Status = "Closed",
            ClosedDate = null
        };

        bool isValid = !(followUp.Status == "Closed" && followUp.ClosedDate == null);

        Assert.False(isValid);
    }
}