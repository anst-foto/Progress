using System.Windows;

namespace Progress;

public partial class MainWindow : Window
{
    private CancellationTokenSource _cts;

    public MainWindow()
    {
        InitializeComponent();
    }

    private async void ButtonStart_OnClick(object sender, RoutedEventArgs e)
    {
        ButtonStart.IsEnabled = false;
        ButtonStop.IsEnabled = true;
        
        var count = int.Parse(InputCount.Text);
        ProgressFile.Maximum = count;

        try
        {
            _cts = new CancellationTokenSource();

            var progress = new Progress<int>();
            progress.ProgressChanged += (_, i) =>
            {
                ProgressFile.Value = i;
                var percent = (double)i * 100 / count;
                ProgressFileText.Text = $"{percent}% ({i} / {count})";
            };

            await FileCopyAsync(
                count: count,
                progress: progress,
                cancellationToken: _cts.Token);
        }
        catch (OperationCanceledException)
        {
            MessageBox.Show("Canceled (OperationCanceledException)");
        }
        finally
        {
            ButtonStart.IsEnabled = true;
            ButtonStop.IsEnabled = false;
            
            _cts.Dispose();
        }
    }

    private async void ButtonStop_OnClick(object sender, RoutedEventArgs e)
    {
        ButtonStart.IsEnabled = true;
        ButtonStop.IsEnabled = false;

        await _cts.CancelAsync();
    }

    private async Task FileCopyAsync(int count, CancellationToken cancellationToken = default, IProgress<int>? progress = null)
    {
        cancellationToken.Register(() => {MessageBox.Show("Canceled");});
        for (int i = 0; i <= count; i++)
        {
            await Task.Delay(100, cancellationToken);
            progress?.Report(i);
        }
    }

    private async void ButtonStart2_OnClick(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i <= 100; i++)
        {
            await Task.Delay(10);
            ProgressFile.Value = i;
        }
    }

    private void ButtonStart3_OnClick(object sender, RoutedEventArgs e)
    {
        for (int i = 0; i <= 100; i++)
        {
            Thread.Sleep(10);
            ProgressFile.Value = i;
        }
    }
}