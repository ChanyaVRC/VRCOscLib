using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test.Collections;

[TestOf(typeof(OscParameterCollection))]
public class OscParameterCollectionTests
{
    private static readonly object?[] _valuesForTestCase = [null, 1, 2, 1.0, 1.2, "somevalue", "something"];

    private static IEnumerable<TestCaseData> ValuesCaseSource => _valuesForTestCase.Select(x => new TestCaseData(x));
    private static IEnumerable<TestCaseData> NewOldCaseSource
    {
        get
        {
            var cases = _valuesForTestCase;
            foreach (var item1 in cases)
            {
                foreach (var item2 in cases)
                {
                    yield return new TestCaseData(item1, item2);
                }
            }
        }
    }

    [TestCaseSource(nameof(NewOldCaseSource))]
    public void TestIndexer(object? value1, object? value2)
    {
        const string address = "/test/address";
        var parameters = new OscParameterCollection();

        Assert.Throws<KeyNotFoundException>(() => _ = parameters[address]);
        Assert.That(parameters.Count, Is.EqualTo(0));

        parameters[address] = value1;
        Assert.That(parameters[address], Is.EqualTo(value1));
        Assert.That(parameters.Count, Is.EqualTo(1));

        parameters[address] = value1;
        Assert.That(parameters[address], Is.EqualTo(value1));
        Assert.That(parameters.Count, Is.EqualTo(1));

        parameters[address] = value2;
        Assert.That(parameters[address], Is.EqualTo(value2));
        Assert.That(parameters.Count, Is.EqualTo(1));
    }

    [TestCaseSource(nameof(NewOldCaseSource))]
    public void TestValueChangedEventWithIndexer(object? value1, object? value2)
    {
        const string address = "/test/address";

        ParameterChangedEventArgs? expected = null;
        var isCalledValueChanged = false;

        var parameters = CreateParameterCollectionForTest();
        parameters.ValueChanged += (sender, e) =>
        {
            Assert.That(expected, Is.Not.Null);
            Assert.That(e.NewValue, Is.EqualTo(expected!.NewValue));
            Assert.That(e.OldValue, Is.EqualTo(expected!.OldValue));
            Assert.That(e.Reason, Is.EqualTo(expected!.Reason));
            Assert.That(e.Address, Is.EqualTo(expected!.Address));
            isCalledValueChanged = true;
        };

        expected = new(null, value1, address, ValueChangedReason.Added, ValueSource.Application);
        parameters[address] = value1;
        Assert.That(isCalledValueChanged, Is.True);
        isCalledValueChanged = false;

        expected = null;
        parameters[address] = value1;
        Assert.That(isCalledValueChanged, Is.False);
        isCalledValueChanged = false;

        expected = OscUtility.AreEqual(value1, value2) ? null : new(value1, value2, address, ValueChangedReason.Substituted, ValueSource.Application);
        parameters[address] = value2;
        Assert.That(isCalledValueChanged, Is.Not.EqualTo(OscUtility.AreEqual(value1, value2)));
        isCalledValueChanged = false;
    }

    [TestCaseSource(nameof(ValuesCaseSource))]
    public void TestAddRemove(object? value)
    {
        const string Address1 = "/test/address1";
        const string Address2 = "/test/address2";

        var parameters = new OscParameterCollection();
        Assert.That(parameters.Count, Is.EqualTo(0));

        parameters.Add(Address1, value);
        Assert.That(parameters[Address1], Is.EqualTo(value));
        Assert.That(parameters.Count, Is.EqualTo(1));

        Assert.Throws<ArgumentException>(() => parameters.Add(Address1, value));
        Assert.That(parameters.Count, Is.EqualTo(1));

        Assert.That(parameters.Remove(Address2), Is.False);
        Assert.That(parameters.Count, Is.EqualTo(1));

        parameters.Add(Address2, value);
        Assert.That(parameters.Count, Is.EqualTo(2));
        Assert.That(parameters[Address2], Is.EqualTo(value));

        Assert.That(parameters.Remove(Address2), Is.True);
        Assert.That(parameters.Count, Is.EqualTo(1));

        parameters.Add(Address2, value);
        Assert.That(parameters.Count, Is.EqualTo(2));
        Assert.That(parameters[Address2], Is.EqualTo(value));
    }

    [TestCaseSource(nameof(ValuesCaseSource))]
    public void TestValueChangedEventAddRemove(object? value)
    {
        const string Address1 = "/test/address1";
        const string Address2 = "/test/address2";

        ParameterChangedEventArgs? expected = null;
        var isCalledValueChanged = false;

        var parameters = CreateParameterCollectionForTest();
        parameters.ValueChanged += (sender, e) =>
        {
            Assert.That(e.NewValue, Is.EqualTo(expected!.NewValue));
            Assert.That(e.OldValue, Is.EqualTo(expected!.OldValue));
            Assert.That(e.Reason, Is.EqualTo(expected!.Reason));
            Assert.That(e.Address, Is.EqualTo(expected!.Address));
            isCalledValueChanged = true;
        };

        void TestValueChangedEvent(ParameterChangedEventArgs? expectedEventArgs, Action testAction)
        {
            expected = expectedEventArgs;
            testAction();
            Assert.That(isCalledValueChanged, Is.EqualTo(expectedEventArgs != null));
            isCalledValueChanged = false;
        }

        TestValueChangedEvent(new(null, value, Address1, ValueChangedReason.Added, ValueSource.Application), () => parameters.Add(Address1, value));
        TestValueChangedEvent(null, () => Assert.Throws<ArgumentException>(() => parameters.Add(Address1, value)));
        TestValueChangedEvent(null, () => parameters.Remove(Address2));
        TestValueChangedEvent(new(null, value, Address2, ValueChangedReason.Added, ValueSource.Application), () => parameters.Add(Address2, value));
        TestValueChangedEvent(new(value, null, Address2, ValueChangedReason.Removed, ValueSource.Application), () => parameters.Remove(Address2));
        TestValueChangedEvent(null, () => parameters.Remove(Address2));
    }

    [Test]
    public void TestClear()
    {
        const string Address1 = "/test/address1";
        const string Address2 = "/test/address2";

        var calledCount = 0;
        var creatingTestSource = false;

        var parameters = CreateParameterCollectionForTest();
        parameters.ValueChanged += (sender, e) =>
        {
            if (creatingTestSource)
            {
                return;
            }
            Assert.That(e.OldValue, Is.Not.Null);
            Assert.That(e.Reason, Is.EqualTo(ValueChangedReason.Removed));
            calledCount++;
        };

        parameters.Clear();
        Assert.That(calledCount, Is.EqualTo(0));
        Assert.That(parameters.Count, Is.EqualTo(0));

        creatingTestSource = true;
        parameters.Add(Address1, 123);
        parameters.Add(Address2, 123.4);
        creatingTestSource = false;

        parameters.Clear();
        Assert.That(calledCount, Is.EqualTo(2));
        Assert.That(parameters.Count, Is.EqualTo(0));
    }


    private static OscParameterCollection CreateParameterCollectionForTest()
    {
        OscParameterCollection parameters = [];
        parameters.ValueChanged += (sender, e) =>
         {
             Assert.That(sender, Is.EqualTo(parameters));
             switch (e.Reason)
             {
                 case ValueChangedReason.Added:
                     Assert.That(e.OldValue, Is.Null);
                     Assert.That(parameters.ContainsKey(e.Address), Is.True);
                     break;
                 case ValueChangedReason.Removed:
                     Assert.That(e.NewValue, Is.Null);
                     Assert.That(parameters.ContainsKey(e.Address), Is.False);
                     break;
                 case ValueChangedReason.Substituted:
                     Assert.That(OscUtility.AreEqual(e.OldValue, e.NewValue), Is.False);
                     Assert.That(parameters.ContainsKey(e.Address), Is.True);
                     break;
                 default:
                     Assert.Fail();
                     break;
             }
         };
        parameters.ValueChanged += (sender, e) => throw new Exception();
        return parameters;
    }

    [Test]
    public void TestAddRemoveValueChangedEventByAddress()
    {
        const string Address1 = "/test/address1";
        const string Address2 = "/test/address2";

        var parameters = CreateParameterCollectionForTest();
        ValueChangedEventArgs? expectedArgs = null;
        var calledCount = 0;
        parameters.ValueChanged += (sender, e) =>
        {
            expectedArgs = e;
            calledCount = 0;
        };
        void TestEvent(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
        {
            Assert.That(sender, Is.EqualTo(parameters));
            Assert.That(e, Is.SameAs(expectedArgs));
            calledCount++;
        }

        parameters.AddValueChangedEventByAddress(Address1, TestEvent);
        parameters[Address1] = 10;
        Assert.That(calledCount, Is.EqualTo(1));

        calledCount = 0;
        parameters[Address1] = 10;
        Assert.That(calledCount, Is.EqualTo(0));

        parameters.AddValueChangedEventByAddress(Address1, TestEvent);
        parameters[Address1] = 20;
        Assert.That(calledCount, Is.EqualTo(2));

        calledCount = 0;
        parameters[Address2] = 30;
        Assert.That(calledCount, Is.EqualTo(0));

        parameters.AddValueChangedEventByAddress(Address2, TestEvent);
        parameters[Address2] = 40;
        Assert.That(calledCount, Is.EqualTo(1));

        Assert.That(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent));
        parameters[Address1] = 40;
        Assert.That(calledCount, Is.EqualTo(1));

        Assert.That(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent));
        parameters[Address1] = 50;
        Assert.That(calledCount, Is.EqualTo(0));

        Assert.That(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent), Is.False);
        parameters[Address1] = 60;
        Assert.That(calledCount, Is.EqualTo(0));
    }

    [Test]
    public void TestLinq()
    {
        var parameters = CreateParameterCollectionForTest();
        parameters["/address/to/parameter1"] = 10;
        parameters["/address/to/parameter2"] = 10f;
        parameters["/address/to/parameter3"] = false;
        Assert.DoesNotThrow(() => parameters.OrderBy(v => v.Key).ToArray());
    }


    [Test]
    public void OnValueChangedByAddress_IgnoreExceptionTest()
    {
        var parameters = CreateParameterCollectionForTest();
        var calledCount = 0;

        const string Address = "/address/to/parameter";
        parameters.AddValueChangedEventByAddress(Address, (_, _) => calledCount++);
        parameters.AddValueChangedEventByAddress(Address, (_, _) => throw new Exception());
        parameters.AddValueChangedEventByAddress(Address, (_, _) => calledCount++);

        parameters[Address] = 1;

        Assert.That(calledCount, Is.EqualTo(2));
    }
}
