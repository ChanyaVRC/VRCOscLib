using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using BuildSoft.VRChat.Osc.Avatar;

namespace ExpressionAvatarChanger;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    AvatarCollection? _avatars;
    AvatarCollection AvatarList => _avatars ??= new(OscAvatarConfig.CreateAll());

    public MainWindow()
    {
        InitializeComponent();
    }

    private void AddContentButton_Click(object sender, RoutedEventArgs e)
    {
        var triggerInput = new TriggerInputItem() { AvatarList = AvatarList };
        if (InputPanel.Children.Count > 0)
        {
            var lastItem = (TriggerInputItem)InputPanel.Children[^1];
            triggerInput.AvatarId = lastItem.AvatarId;
            triggerInput.AvatarName = lastItem.AvatarName;
            triggerInput.ParameterName = lastItem.ParameterName;
            triggerInput.ThresholdType = lastItem.ThresholdType;
            triggerInput.Threshold = lastItem.Threshold;
        }
        InputPanel.Children.Add(triggerInput);
    }

    private void RemoveContentButton_Click(object sender, RoutedEventArgs e)
    {
        if (InputPanel.Children.Count <= 0)
        {
            return;
        }
        var item = (TriggerInputItem)InputPanel.Children[^1];
        item.IsActive = false;

        InputPanel.Children.Remove(item);
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        foreach (var item in InputPanel.Children)
        {
            if (item is not TriggerInputItem triggerInput)
            {
                continue;
            }
            triggerInput.AvatarList = AvatarList;
        }
    }
}
