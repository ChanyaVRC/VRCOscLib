using System.Windows;
using System.Windows.Input;
using BuildSoft.VRChat.Osc;
using BuildSoft.VRChat.Osc.Input;

namespace GuiController;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        OscUtility.Initialize();
    }

    private void Forward_MouseEnter(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveForward.Press();
    }
    private void Forward_MouseLeave(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveForward.Release();
    }

    private void Left_MouseEnter(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveLeft.Press();
    }
    private void Left_MouseLeave(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveLeft.Release();
    }

    private void Right_MouseEnter(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveRight.Press();
    }
    private void Right_MouseLeave(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveRight.Release();
    }

    private void Backward_MouseEnter(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveBackward.Press();
    }
    private void Backward_MouseLeave(object sender, MouseEventArgs e)
    {
        OscButtonInput.MoveBackward.Release();
    }

    private void Jump_MouseDown(object sender, MouseButtonEventArgs e)
    {
        OscButtonInput.Jump.Press();
    }
    private void Jump_MouseUp(object sender, MouseButtonEventArgs e)
    {
        OscButtonInput.Jump.Release();
    }
}
