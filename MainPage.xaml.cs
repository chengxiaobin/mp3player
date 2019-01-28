using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.Media;
using System.Windows;
using System.Diagnostics;
using System.Linq;
using Windows.Storage.Pickers;
using Windows.Storage;
using Windows.Storage.AccessCache;
using System.Text;
using System.Collections.Generic;
using Windows.UI.Xaml.Navigation;// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.Media.Playback;
using Windows.Media.Core;

namespace Mp3Player
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        SystemMediaTransportControls systemControls;
                                                                                       
        List<string> list = new List<string>();
        ListView listView1 = new ListView();

        int lvindex = 0;
        IReadOnlyList<StorageFile> files;
        FileOpenPicker openPicker = new FileOpenPicker();


        public MainPage()
        {
            this.InitializeComponent();
          
            systemControls = SystemMediaTransportControls.GetForCurrentView();
            systemControls.ButtonPressed += SystemControls_ButtonPressed;
            systemControls.IsPlayEnabled = true;
            systemControls.IsPauseEnabled = true;

            PickFilesButton.Click += PickFilesButton_Click;
            listView1.ItemClick += ListView1_SelectionChanged;


        }


        void MusicPlayer_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            switch (musicPlayer.CurrentState)
            {
                case MediaElementState.Playing:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Playing;
                    break;
                case MediaElementState.Paused:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Paused;
                    break;
                case MediaElementState.Stopped:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Stopped;
                    break;
                case MediaElementState.Closed:
                    systemControls.PlaybackStatus = MediaPlaybackStatus.Closed;
                    break;
                default:
                    break;
            }
        }


        void SystemControls_ButtonPressed(SystemMediaTransportControls sender,
            SystemMediaTransportControlsButtonPressedEventArgs args)
        {
            
            switch (args.Button)
            {
                case SystemMediaTransportControlsButton.Play:
                    PlayMedia();
                    break;
                case SystemMediaTransportControlsButton.Pause:
                    PauseMedia();
                    break;
                default:
                    break;
            }
        }

        async void PlayMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                musicPlayer.Play();
            });
        }

        async void PauseMedia()
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                musicPlayer.Pause();
            });
        }


        private async void PickFilesButton_Click(object sender, RoutedEventArgs e)
        {
            IsEnabled = false;
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            files = await openPicker.PickMultipleFilesAsync();
            if (files.Count > 0)
            {
                for (int count = 0; count < files.Count; count++)
                {
                    list.Add(item: files[count].Name);

                    listView1.ItemsSource = list;
                    listView1.SelectionChanged += ListView1_SelectionChanged;

                }
                MainPanel.Children.Add(listView1);
            }
            IsEnabled = true;
        }


        private async void ListView1_SelectionChanged(object sender, RoutedEventArgs e)
        {
            var lvsi = listView1.SelectedItems[0];
            OutputTextBlock.Text = lvsi.ToString();
            string testname = lvsi.ToString();

            for (int count = 0; count < files.Count; count++)
            {

                if (files[count].Name == testname)
                    lvindex= count;
            }
            await SetLocalMedia();

        }

        async private System.Threading.Tasks.Task SetLocalMedia()
        {
                 StorageFile file = files.ElementAt(lvindex);


                if (file != null)
                    {
                        var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                        musicPlayer.SetSource(stream, file.ContentType);

                        musicPlayer.Play();
                    }
                }
        }

    }
