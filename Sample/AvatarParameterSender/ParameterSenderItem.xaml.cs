using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BuildSoft.VRChat.Osc;

namespace AvatarParameterSender;
/// <summary>
/// Interaction logic for ParameterSenderItem.xaml
/// </summary>
public partial class ParameterSenderItem : UserControl
{
    public static readonly DependencyProperty ParameterNameProperty =
        DependencyProperty.Register(nameof(ParameterName), typeof(string), typeof(ParameterSenderItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty AddressProperty =
        DependencyProperty.Register(nameof(Address), typeof(string), typeof(ParameterSenderItem), new PropertyMetadata(string.Empty));

    public static readonly DependencyProperty TypeProperty =
        DependencyProperty.Register(nameof(Type), typeof(OscType), typeof(ParameterSenderItem), new PropertyMetadata(OscType.Int, OnTypeChanged));

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(string), typeof(ParameterSenderItem), new PropertyMetadata(string.Empty, null, CoerceValue));

    private static void OnTypeChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
    {
        var senderItem = (ParameterSenderItem)dependencyObject;

        var items = senderItem.ValueBox.Items;
        items.Clear();

        senderItem.ValueBox.IsEditable = true;

        if ((OscType)e.NewValue == OscType.Bool)
        {
            senderItem.ValueBox.IsEditable = false;
            items.Add(bool.TrueString);
            items.Add(bool.FalseString);
        }
        dependencyObject.CoerceValue(ValueProperty);
    }

    private static object CoerceValue(DependencyObject dependencyObject, object value)
    {
        var senderItem = (ParameterSenderItem)dependencyObject;
        string strValue = (string)value;

        if (strValue == "")
        {
            return "";
        }

        bool canConvert = false;
        switch (senderItem.Type)
        {
            case OscType.Bool:
                canConvert = bool.TryParse(strValue, out _);
                break;
            case OscType.Int:
                canConvert = int.TryParse(strValue, out _);
                break;
            case OscType.Float:
                canConvert = float.TryParse(strValue, out _);
                break;
        }
        return canConvert ? value : "";
    }


    public string ParameterName
    {
        get { return (string)GetValue(ParameterNameProperty); }
        set { SetValue(ParameterNameProperty, value); }
    }

    public string Address
    {
        get { return (string)GetValue(AddressProperty); }
        set { SetValue(AddressProperty, value); }
    }

    public OscType Type
    {
        get { return (OscType)GetValue(TypeProperty); }
        set { SetValue(TypeProperty, value); }
    }

    public string Value
    {
        get { return (string)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }


    public ParameterSenderItem()
    {
        InitializeComponent();
    }


    private void ValueBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
    {
        switch (Type)
        {
            case OscType.Int:
                e.Handled = !int.TryParse(ValueBox.Text + e.Text, out _);
                break;
            case OscType.Float:
                e.Handled = !float.TryParse(ValueBox.Text + e.Text, out _);
                break;
        }
    }

    private void SendButton_Click(object sender, RoutedEventArgs e)
    {
        var address = Address;
        var value = Value;
        if (string.IsNullOrEmpty(address) || string.IsNullOrEmpty(value))
        {
            return;
        }

        switch (Type)
        {
            case OscType.Bool:
                if (bool.TryParse(value, out bool boolValue))
                {
                    OscParameter.SendValue(address, boolValue);
                }
                break;
            case OscType.Int:
                if (int.TryParse(value, out int intValue))
                {
                    OscParameter.SendValue(address, intValue);
                }
                break;
            case OscType.Float:
                if (float.TryParse(value, out float floatValue))
                {
                    OscParameter.SendValue(address, floatValue);
                }
                break;
        }
    }
}
