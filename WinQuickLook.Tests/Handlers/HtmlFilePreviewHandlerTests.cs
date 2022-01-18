using System.IO;

using WinQuickLook.Handlers;

using Xunit;

namespace WinQuickLook.Tests.Handlers;

public class HtmlFilePreviewHandlerTests
{
    [Theory]
    [InlineData("test.htm", true)]
    [InlineData("test.html", true)]
    [InlineData("test.xhtml", true)]
    [InlineData("test.HTM", true)]
    [InlineData("test.HTML", true)]
    [InlineData("test.XHTML", true)]
    [InlineData("test.jpg", false)]
    [InlineData("test.JPG", false)]
    [InlineData("test", false)]
    public void CanOpen(string fileName, bool expected)
    {
        var fileInfo = new FileInfo(fileName);

        var handler = new HtmlFilePreviewHandler();

        var actual = handler.CanOpen(fileInfo);

        Assert.Equal(expected, actual);
    }
}
