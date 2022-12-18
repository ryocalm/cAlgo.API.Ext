using System.Collections.Generic;
using Xunit;
using FluentAssertions;
using cAlgo.API.Ext;

namespace cAlgo.API.Ext.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        const string str = "Minute";
        var results = new List<bool>
        {
            str.IsLongerTermBy(TimeFrame.Minute, 0),
            str.IsLongerTermBy(TimeFrame.Minute5, 1),
            str.IsLongerTermBy(TimeFrame.Minute15, 2),
            str.IsLongerTermBy(TimeFrame.Minute30, 3),
            str.IsLongerTermBy(TimeFrame.Hour, 4),
            str.IsLongerTermBy(TimeFrame.Hour4, 5),
            str.IsLongerTermBy(TimeFrame.Daily, 6),
            str.IsLongerTermBy(TimeFrame.Weekly, 7),
            str.IsLongerTermBy(TimeFrame.Monthly, 8),
        };

        results.Should().AllBeEquivalentTo(true);
    }
}