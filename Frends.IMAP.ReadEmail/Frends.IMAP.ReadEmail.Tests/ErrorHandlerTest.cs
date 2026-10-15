using System;
using System.Threading;
using Frends.IMAP.ReadEmail.Definitions;
using NUnit.Framework;

namespace Frends.IMAP.ReadEmail.Tests;

[TestFixture]
internal class ErrorHandlerTest
{
    private const string CustomErrorMessage = "CustomErrorMessage";

    private static Input DefaultInput() => new() { MaxEmails = 10 };

    private static Connection DefaultConnection() => new()
    {
        Host = "invalid.invalid",
        Port = 993,
        UseSSL = true,
        UserName = "user",
        Password = "password",
    };

    private static Options DefaultOptions() => new();

    [Test]
    public void Should_Throw_Error_When_ThrowErrorOnFailure_Is_True()
    {
        var ex = Assert.Catch<Exception>(() =>
            IMAP.ReadEmail(DefaultInput(), DefaultConnection(), DefaultOptions(), CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
    }

    [Test]
    public void Should_Return_Failed_Result_When_ThrowErrorOnFailure_Is_False()
    {
        var options = DefaultOptions();
        options.ThrowErrorOnFailure = false;
        var result = IMAP.ReadEmail(DefaultInput(), DefaultConnection(), options, CancellationToken.None);
        Assert.That(result.Success, Is.False);
        Assert.That(result.Error, Is.Not.Null);
    }

    [Test]
    public void Should_Use_Custom_ErrorMessageOnFailure()
    {
        var options = DefaultOptions();
        options.ErrorMessageOnFailure = CustomErrorMessage;
        var ex = Assert.Throws<Exception>(() =>
            IMAP.ReadEmail(DefaultInput(), DefaultConnection(), options, CancellationToken.None));
        Assert.That(ex, Is.Not.Null);
        Assert.That(ex.Message, Contains.Substring(CustomErrorMessage));
    }
}
