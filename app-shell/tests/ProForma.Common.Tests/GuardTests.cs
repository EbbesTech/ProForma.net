using Microsoft.AspNetCore.Components;

using ProForma.Common.Guard;

using System.Collections;

namespace ProForma.Common.Tests;

[TestClass]
public sealed class GuardTests
{

    // /// IsNotNull / IsNull

    [TestMethod]
    public void IsNotNull_WithNonNullValue_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsNotNull("Hello");

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNull_WithNullValue_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNull<string?>(null);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNull_WithNullValue_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsNull<string?>(null);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsNull_WithNonNullValue_Fails()
    {
        var result = Guard<ArgumentException>.IsNull("Hello");

        Assert.IsTrue(result.HasFailed);
    }

    // /// Type checking
    [TestMethod]
    public void IsAssignableToType_WithAssignableType_DoesNotFail()
    {
        var result = Guard<ArgumentException>
            .IsAssignableToType<string, string>();

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsAssignableToType_WithNonAssignableType_Fails()
    {
        var result = Guard<ArgumentException>
            .IsAssignableToType<int, Exception>();

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsImplementingInterface_WithImplementingType_DoesNotFail()
    {
        var result = Guard<ArgumentException>
            .IsImplementingInterface<List<string>, IList<string>>();

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsImplementingInterface_WithNonImplementingType_Fails()
    {
        var result = Guard<ArgumentException>
            .IsImplementingInterface<Exception, IAsyncResult>();

        Assert.IsTrue(result.HasFailed);
    }

    // /// Assert
    [TestMethod]
    public void Assert_WhenPredicateIsTrue_DoesNotFail()
    {
        var result = Guard<ArgumentException>
            .Assert(42, x => x > 0);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void Assert_WhenPredicateIsFalse_Fails()
    {
        var result = Guard<ArgumentException>
            .Assert(42, x => x < 0);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void Assert_PassesItemToPredicate()
    {
        var received = 0;

        Guard<ArgumentException>.Assert(42, x =>
        {
            received = x;
            return true;
        });

        Assert.AreEqual(42, received);
    }

    // /// Comparing
    public void IsEqualTo_WhenValuesAreEqual_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsEqualTo(42, 42);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsEqualTo_WhenValuesAreDifferent_Fails()
    {
        var result = Guard<ArgumentException>.IsEqualTo(42, 43);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThan_WhenFirstValueIsSmaller_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsLessThan(1, 2);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThan_WhenValuesAreEqual_Fails()
    {
        var result = Guard<ArgumentException>.IsLessThan(2, 2);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThan_WhenFirstValueIsGreater_Fails()
    {
        var result = Guard<ArgumentException>.IsLessThan(3, 2);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThanOrEqualTo_WhenFirstValueIsSmaller_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsLessThanOrEqualTo(1, 2);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThanOrEqualTo_WhenValuesAreEqual_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsLessThanOrEqualTo(2, 2);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsLessThanOrEqualTo_WhenFirstValueIsGreater_Fails()
    {
        var result = Guard<ArgumentException>.IsLessThanOrEqualTo(3, 2);

        Assert.IsTrue(result.HasFailed);
    }
    [TestMethod]
    public void IsGreaterThan_WhenFirstValueIsGreater_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsGreaterThan(2, 1);

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsGreaterThan_WhenValuesAreEqual_Fails()
    {
        var result = Guard<ArgumentException>.IsGreaterThan(2, 2);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsGreaterThan_WhenFirstValueIsSmaller_Fails()
    {
        var result = Guard<ArgumentException>.IsGreaterThan(1, 2);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrEmpty_WithText_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsNotNullOrEmpty("Hello");

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrEmpty_WithEmptyString_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNullOrEmpty("");

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrEmpty_WithNull_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNullOrEmpty(null!);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrEmpty_WithWhitespace_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsNotNullOrEmpty(" ");

        Assert.IsFalse(result.HasFailed);
    }
    [TestMethod]
    public void IsNotNullOrWhiteSpace_WithText_DoesNotFail()
    {
        var result = Guard<ArgumentException>.IsNotNullOrWhiteSpace("Hello");

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrWhiteSpace_WithEmptyString_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNullOrWhiteSpace("");

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrWhiteSpace_WithWhitespace_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNullOrWhiteSpace("   ");

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    public void IsNotNullOrWhiteSpace_WithNull_Fails()
    {
        var result = Guard<ArgumentException>.IsNotNullOrWhiteSpace(null!);

        Assert.IsTrue(result.HasFailed);
    }

    [TestMethod]
    [DataRow(true, true)]
    [DataRow(false, false)]
    public void IsTrue_ReturnsExpectedResult(
    bool expression,
    bool expectedFailure)
    {
        var result = Guard<ArgumentException>.IsTrue(expression);

        Assert.AreEqual(expectedFailure, result.HasFailed);
    }

    [TestMethod]
    [DataRow(false, true)]
    [DataRow(true, false)]
    public void IsFalse_ReturnsExpectedResult(
        bool expression,
        bool expectedFailure)
    {
        var result = Guard<ArgumentException>.IsFalse(expression);

        Assert.AreEqual(expectedFailure, result.HasFailed);
    }

    [TestMethod]
    public void Throw_WhenGuardPassed_DoesNotThrow()
    {
        var result = Guard<ArgumentException>.IsNotNull("Hello");

        result.Throw();

        Assert.IsFalse(result.HasFailed);
    }

    [TestMethod]
    public void Throw_WhenGuardFailed_ThrowsExpectedException()
    {
        var result = Guard<ArgumentException>.IsNotNull<string?>(null);
        Assert.ThrowsExactly<ArgumentException>(() => result.Throw());
    }

    [TestMethod]
    public void Throw_WhenGuardFailed_ThrowsExpectedException_WithCustomMessage()
    {
        var result = Guard<ArgumentException>.IsNotNull<string?>(null);
        try
        {
            result.Throw("Test");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("Test", ex.Message);
        }
    }

    [TestMethod]
    public void Throw_WhenGuardFailed_ThrowsExpectedException_WithInnerException()
    {
        var result = Guard<ArgumentException>.IsNotNull<string?>(null);
        try
        {
            result.Throw("Test");
        }
        catch (Exception ex)
        {
            Assert.AreEqual("Test", ex.Message);
        }
    }
}
