using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
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
using BuildSoft.VRChat.Osc.Avatar;

namespace ExpressionAvatarChanger;

/// <summary>
/// TriggerInput.xaml の相互作用ロジック
/// </summary>
public partial class TriggerInputItem : UserControl
{
    public static readonly DependencyProperty AvatarIdProperty =
                    DependencyProperty.Register(nameof(AvatarId), typeof(string), typeof(TriggerInputItem), new PropertyMetadata(""), ValidateAvatarId);

    public static readonly DependencyProperty AvatarNameProperty =
                    DependencyProperty.Register(nameof(AvatarName), typeof(string), typeof(TriggerInputItem), new PropertyMetadata(""));

    public static readonly DependencyProperty ParameterNameProperty =
                    DependencyProperty.Register(nameof(ParameterName), typeof(string), typeof(TriggerInputItem), new PropertyMetadata(""));

    public static readonly DependencyProperty ThresholdTypeProperty =
                    DependencyProperty.Register(nameof(ThresholdType), typeof(OscType), typeof(TriggerInputItem), new FrameworkPropertyMetadata(default(OscType), OnThresholdTypeChanged));

    public static readonly DependencyProperty ThresholdProperty =
                    DependencyProperty.Register(nameof(Threshold), typeof(string), typeof(TriggerInputItem), new FrameworkPropertyMetadata("", OnThresholdChanged, CoerceThreshold));

    public static readonly DependencyProperty IsActiveProperty =
                    DependencyProperty.Register(nameof(IsActive), typeof(bool), typeof(TriggerInputItem), new PropertyMetadata(false, OnIsActiveChanged, CoerceIsActive));

    public static readonly DependencyProperty AvatarListProperty =
                    DependencyProperty.Register(nameof(AvatarList), typeof(AvatarCollection), typeof(TriggerInputItem), new FrameworkPropertyMetadata(new AvatarCollection(), OnAvatarListChanged));

    private static void OnIsActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = (TriggerInputItem)d;

        if (item._parameter != null)
        {
            item._parameter.ValueChanged -= item.ChangeAvatar;
        }

        var isActive = (bool)e.NewValue;
        if (isActive)
        {
            var parameter = OscAvatarParameter.Create(item.ParameterName, item.ThresholdType);
            parameter.ValueChanged += item.ChangeAvatar;
            item._parameter = parameter;
        }
    }

    private static object CoerceIsActive(DependencyObject d, object baseValue)
    {
        var item = (TriggerInputItem)d;
        var isActive = (bool)baseValue;
        if (!isActive)
        {
            return baseValue;
        }

        if (!AvatarIdRegex().IsMatch(item.AvatarId))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(item.ParameterName))
        {
            return false;
        }
        if (string.IsNullOrWhiteSpace(item.Threshold))
        {
            return false;
        }
        return baseValue;
    }

    private static void OnAvatarListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = (TriggerInputItem)d;
        var avatarList = (AvatarCollection)e.NewValue;
        item.DataContext = new
        {
            AvatarIdList = avatarList.Select(a => a.Id),
            AvatarNameList = avatarList.Select(a => a.Name),
            ParameterList = avatarList.SelectMany(a => a.Parameters.Select(v => v.Item1)).Order().Distinct(),
        };
    }

    private static bool ValidateAvatarId(object value)
    {
        return PartOfAvatarIdRegex().IsMatch((string)value);
    }

    private static void OnThresholdTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = (TriggerInputItem)d;
        item.Threshold = e.NewValue switch
        {
            OscType.Bool => false.ToString(),
            OscType.Int => 0.ToString(),
            OscType.Float => 0f.ToString(),
            _ => throw new NotImplementedException(),
        };
    }

    private static void OnThresholdChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var item = (TriggerInputItem)d; 
        
        if (item.ThresholdType == OscType.Bool)
        {
            item.ThresholdComboBox.SelectedIndex = (string)e.NewValue == bool.TrueString ? 0 : 1;
        }
    }

    private static object CoerceThreshold(DependencyObject d, object baseValue)
    {
        var item = (TriggerInputItem)d;
        var value = (string)baseValue;

        var canParse = item.ThresholdType switch
        {
            OscType.Bool => bool.TryParse(value, out _),
            OscType.Int => int.TryParse(value, out _),
            OscType.Float => float.TryParse(value, out _),
            _ => throw new NotImplementedException()
        };
        return canParse ? baseValue : DependencyProperty.UnsetValue;
    }


    private OscAvatarParameter? _parameter;

    public TriggerInputItem()
    {
        InitializeComponent();
        DataContext = new
        {
            AvatarIdList = Array.Empty<string>(),
            AvatarNameList = Array.Empty<string>(),
            ParameterList = Array.Empty<string>(),
        };
    }

    public string AvatarId
    {
        get => (string)GetValue(AvatarIdProperty);
        set => SetValue(AvatarIdProperty, value);
    }

    public string AvatarName
    {
        get => (string)GetValue(AvatarNameProperty);
        set => SetValue(AvatarNameProperty, value);
    }

    public string ParameterName
    {
        get => (string)GetValue(ParameterNameProperty);
        set => SetValue(ParameterNameProperty, value);
    }

    public OscType ThresholdType
    {
        get => (OscType)GetValue(ThresholdTypeProperty);
        set => SetValue(ThresholdTypeProperty, value);
    }

    public string Threshold
    {
        get => (string)GetValue(ThresholdProperty);
        set => SetValue(ThresholdProperty, value);
    }

    public bool IsActive
    {
        get => (bool)GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }


    public AvatarCollection AvatarList
    {
        get => (AvatarCollection)GetValue(AvatarListProperty);
        set => SetValue(AvatarListProperty, value);
    }


    private void ChangeAvatar(OscAvatarParameter parameter, ValueChangedEventArgs e)
    {
        Dispatcher.Invoke(() =>
        {
            var newValue = e.NewValue;
            if (newValue == null || newValue.ToString() != Threshold)
            {
                return;
            }

            var avatarId = AvatarId;
            if (AvatarIdRegex().IsMatch(avatarId))
            {
                OscAvatarUtility.ChangeAvatar(avatarId);
            }
        });
    }

    private void AvatarIdComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = AvatarIdComboBox.SelectedIndex;
        if (index != -1)
        {
            AvatarName = AvatarList[index].Name;
        }
    }

    private void AvatarNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = AvatarNameComboBox.SelectedIndex;
        if (index != -1)
        {
            AvatarId = AvatarList[index].Id;
        }
    }

    private void ParameterNameComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var index = ParameterNameComboBox.SelectedIndex;
        if (index != -1)
        {
            var newParameterName = (string)ParameterNameComboBox.Items[index];
            ThresholdType = AvatarList.SelectMany(v => v.Parameters).First(v => v.Item1 == newParameterName).Item2;
        }
    }

    private void InactiveOnTextChanged(object sender, TextChangedEventArgs e)
    {
        IsActive = false;
    }


    [GeneratedRegex(@"^[avtr_a-f0-9\-]{0,41}$")]
    private static partial Regex PartOfAvatarIdRegex();

    [GeneratedRegex(@"^avtr_[a-f0-9]{8}(-[a-f0-9]{4}){3}-[a-f0-9]{12}$")]
    private static partial Regex AvatarIdRegex();
}
