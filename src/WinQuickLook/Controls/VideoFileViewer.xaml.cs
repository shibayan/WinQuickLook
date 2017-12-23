using System;
using System.Windows;
using System.Windows.Threading;

namespace WinQuickLook.Controls
{
    public partial class VideoFileViewer
    {
        public VideoFileViewer()
        {
            InitializeComponent();
        }

        private bool _isSeeking;

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        public Uri Source
        {
            get => (Uri)GetValue(SourceProperty);
            set => SetValue(SourceProperty, value);
        }

        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(Uri), typeof(VideoFileViewer), new PropertyMetadata(null));
        
        private void MediaElement_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (!mediaElement.NaturalDuration.HasTimeSpan)
            {
                duration.Text = "00:00";
                slider.IsEnabled = false;

                return;
            }

            var timeSpan = mediaElement.NaturalDuration.TimeSpan;

            duration.Text = $"{(int)timeSpan.TotalMinutes:D2}:{timeSpan.Seconds:D2}";

            slider.IsEnabled = true;
            slider.Maximum = timeSpan.TotalSeconds;

            _timer.Interval = TimeSpan.FromMilliseconds(250);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();

            playButton.Visibility = Visibility.Collapsed;
            pauseButton.Visibility = Visibility.Visible;
        }

        private void PauseButton_Click(object sender, RoutedEventArgs e)
        {
            mediaElement.Pause();

            playButton.Visibility = Visibility.Visible;
            pauseButton.Visibility = Visibility.Collapsed;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (!_isSeeking)
            {
                slider.Value = mediaElement.Position.TotalSeconds;
            }
        }

        private void Slider_DragStarted(object sender, RoutedEventArgs e)
        {
            _isSeeking = true;
        }

        private void Slider_DragCompleted(object sender, RoutedEventArgs e)
        {
            _isSeeking = false;

            mediaElement.Position = TimeSpan.FromSeconds(slider.Value);
        }

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            mediaElement.Play();
        }
    }
}
