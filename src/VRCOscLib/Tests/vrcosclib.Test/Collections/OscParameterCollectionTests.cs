﻿using NUnit.Framework;

namespace BuildSoft.VRChat.Osc.Test;

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
        Assert.AreEqual(0, parameters.Count);

        parameters[address] = value1;
        Assert.AreEqual(value1, parameters[address]);
        Assert.AreEqual(1, parameters.Count);

        parameters[address] = value1;
        Assert.AreEqual(value1, parameters[address]);
        Assert.AreEqual(1, parameters.Count);

        parameters[address] = value2;
        Assert.AreEqual(value2, parameters[address]);
        Assert.AreEqual(1, parameters.Count);
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
            Assert.IsNotNull(expected);
            Assert.AreEqual(expected!.NewValue, e.NewValue);
            Assert.AreEqual(expected!.OldValue, e.OldValue);
            Assert.AreEqual(expected!.Reason, e.Reason);
            Assert.AreEqual(expected!.Address, e.Address);
            isCalledValueChanged = true;
        };

        expected = new(null, value1, address, ValueChangedReason.Added, ValueSource.Application);
        parameters[address] = value1;
        Assert.IsTrue(isCalledValueChanged);
        isCalledValueChanged = false;

        expected = null;
        parameters[address] = value1;
        Assert.IsFalse(isCalledValueChanged);
        isCalledValueChanged = false;

        expected = OscUtility.AreEqual(value1, value2) ? null : new(value1, value2, address, ValueChangedReason.Substituted, ValueSource.Application);
        parameters[address] = value2;
        Assert.AreNotEqual(OscUtility.AreEqual(value1, value2), isCalledValueChanged);
        isCalledValueChanged = false;
    }

    [TestCaseSource(nameof(ValuesCaseSource))]
    public void TestAddRemove(object? value)
    {
        const string Address1 = "/test/address1";
        const string Address2 = "/test/address2";

        var parameters = new OscParameterCollection();
        Assert.AreEqual(0, parameters.Count);

        parameters.Add(Address1, value);
        Assert.AreEqual(parameters[Address1], value);
        Assert.AreEqual(1, parameters.Count);

        Assert.Throws<ArgumentException>(() => parameters.Add(Address1, value));
        Assert.AreEqual(1, parameters.Count);

        Assert.IsFalse(parameters.Remove(Address2));
        Assert.AreEqual(1, parameters.Count);

        parameters.Add(Address2, value);
        Assert.AreEqual(2, parameters.Count);
        Assert.AreEqual(parameters[Address2], value);

        Assert.IsTrue(parameters.Remove(Address2));
        Assert.AreEqual(1, parameters.Count);

        parameters.Add(Address2, value);
        Assert.AreEqual(2, parameters.Count);
        Assert.AreEqual(parameters[Address2], value);
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
            Assert.AreEqual(expected!.NewValue, e.NewValue);
            Assert.AreEqual(expected!.OldValue, e.OldValue);
            Assert.AreEqual(expected!.Reason, e.Reason);
            Assert.AreEqual(expected!.Address, e.Address);
            isCalledValueChanged = true;
        };

        void TestValueChangedEvent(ParameterChangedEventArgs? expectedEventArgs, Action testAction)
        {
            expected = expectedEventArgs;
            testAction();
            Assert.AreEqual(expectedEventArgs != null, isCalledValueChanged);
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

        int calledCount = 0;
        bool creatingTestSource = false;

        var parameters = CreateParameterCollectionForTest();
        parameters.ValueChanged += (sender, e) =>
        {
            if (creatingTestSource)
            {
                return;
            }
            Assert.IsNotNull(e.OldValue);
            Assert.AreEqual(ValueChangedReason.Removed, e.Reason);
            calledCount++;
        };

        parameters.Clear();
        Assert.AreEqual(0, calledCount);
        Assert.AreEqual(0, parameters.Count);

        creatingTestSource = true;
        parameters.Add(Address1, 123);
        parameters.Add(Address2, 123.4);
        creatingTestSource = false;

        parameters.Clear();
        Assert.AreEqual(2, calledCount);
        Assert.AreEqual(0, parameters.Count);
    }


    private static OscParameterCollection CreateParameterCollectionForTest()
    {
        OscParameterCollection parameters = [];
        parameters.ValueChanged += (sender, e) =>
         {
             Assert.AreEqual(parameters, sender);
             switch (e.Reason)
             {
                 case ValueChangedReason.Added:
                     Assert.IsNull(e.OldValue);
                     Assert.IsTrue(parameters.ContainsKey(e.Address));
                     break;
                 case ValueChangedReason.Removed:
                     Assert.IsNull(e.NewValue);
                     Assert.IsFalse(parameters.ContainsKey(e.Address));
                     break;
                 case ValueChangedReason.Substituted:
                     Assert.IsFalse(OscUtility.AreEqual(e.OldValue, e.NewValue));
                     Assert.IsTrue(parameters.ContainsKey(e.Address));
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
        int calledCount = 0;
        parameters.ValueChanged += (sender, e) =>
        {
            expectedArgs = e;
            calledCount = 0;
        };
        void TestEvent(IReadOnlyOscParameterCollection sender, ParameterChangedEventArgs e)
        {
            Assert.AreSame(parameters, sender);
            Assert.AreSame(expectedArgs, e);
            calledCount++;
        }

        parameters.AddValueChangedEventByAddress(Address1, TestEvent);
        parameters[Address1] = 10;
        Assert.AreEqual(1, calledCount);

        calledCount = 0;
        parameters[Address1] = 10;
        Assert.AreEqual(0, calledCount);

        parameters.AddValueChangedEventByAddress(Address1, TestEvent);
        parameters[Address1] = 20;
        Assert.AreEqual(2, calledCount);

        calledCount = 0;
        parameters[Address2] = 30;
        Assert.AreEqual(0, calledCount);

        parameters.AddValueChangedEventByAddress(Address2, TestEvent);
        parameters[Address2] = 40;
        Assert.AreEqual(1, calledCount);

        Assert.IsTrue(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent));
        parameters[Address1] = 40;
        Assert.AreEqual(1, calledCount);

        Assert.IsTrue(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent));
        parameters[Address1] = 50;
        Assert.AreEqual(0, calledCount);

        Assert.IsFalse(parameters.RemoveValueChangedEventByAddress(Address1, TestEvent));
        parameters[Address1] = 60;
        Assert.AreEqual(0, calledCount);
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
        int calledCount = 0;

        const string Address = "/address/to/parameter";
        parameters.AddValueChangedEventByAddress(Address, (_, _) => calledCount++);
        parameters.AddValueChangedEventByAddress(Address, (_, _) => throw new Exception());
        parameters.AddValueChangedEventByAddress(Address, (_, _) => calledCount++);

        parameters[Address] = 1;

        Assert.AreEqual(2, calledCount);
    }
}
