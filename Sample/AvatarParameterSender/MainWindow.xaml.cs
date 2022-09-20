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
using BuildSoft.VRChat.Osc.Avatar;

namespace AvatarParameterSender;
/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public static readonly DependencyProperty CurrentAvatarNameProperty =
        DependencyProperty.Register(nameof(CurrentAvatarName), typeof(string), typeof(MainWindow), new PropertyMetadata(string.Empty));

    public string CurrentAvatarName
    {
        get { return (string)GetValue(CurrentAvatarNameProperty); }
        set { SetValue(CurrentAvatarNameProperty, value); }
    }

    public MainWindow()
    {
        InitializeComponent();
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        OscAvatarUtility.AvatarChanged += (sender, e) =>
        {
            Dispatcher.Invoke(() => LoadCurrentAvatarConfig());
        };
    }

    private void LoadCurrentAvatarConfig()
    {
        var currentConfig = OscAvatarConfig.CreateAtCurrent();
        if (currentConfig == null)
        {
            return;
        }
        CurrentAvatarName = currentConfig.Name;

        var children = ParameterPanel.Children;
        children.Clear();

        var items = currentConfig.Parameters.Items;
        for (int i = 0; i < items.Length; i++)
        {
            var input = items[i].Input;
            if (input == null)
            {
                continue;
            }

            children.Add(new ParameterSenderItem()
            {
                Address = input.Address,
                ParameterName = items[i].Name,
                Type = input.OscType,
            });
        }
    }
}
