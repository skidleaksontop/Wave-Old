

// Wave, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// Wave.Controls.InstancePanel
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Net.Http;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using Wave.Classes.Implementations;
using Wave.Controls;

namespace Wave.Controls
{
    public partial class InstancePanel : UserControl, IComponentConnector
    {
        public static readonly DependencyProperty UsernameProperty = DependencyProperty.Register("Username", typeof(string), typeof(InstancePanel), new PropertyMetadata("Username"));

        public static readonly DependencyProperty UserIDProperty = DependencyProperty.Register("UserID", typeof(double), typeof(InstancePanel), new PropertyMetadata(0.0));

        public static readonly DependencyProperty ProcessIDProperty = DependencyProperty.Register("ProcessID", typeof(int), typeof(InstancePanel), new PropertyMetadata(0));

        public static readonly DependencyProperty ScriptProperty = DependencyProperty.Register("Script", typeof(string), typeof(InstancePanel), new PropertyMetadata("print('Not Implemented');"));

        public string Username
        {
            get
            {
                return (string)GetValue(UsernameProperty);
            }
            set
            {
                SetValue(UsernameProperty, value);
            }
        }

        public double UserID
        {
            get
            {
                return (double)GetValue(UserIDProperty);
            }
            set
            {
                SetValue(UserIDProperty, value);
                UpdateThumbnail();
            }
        }

        public int ProcessID
        {
            get
            {
                return (int)GetValue(ProcessIDProperty);
            }
            set
            {
                SetValue(ProcessIDProperty, value);
            }
        }

        public string Script
        {
            get
            {
                return (string)GetValue(ScriptProperty);
            }
            set
            {
                SetValue(ScriptProperty, value);
            }
        }

        public InstancePanel()
        {
            InitializeComponent();
        }

        private async void UpdateThumbnail()
        {
            Console.WriteLine("Yes");
            HttpClient client = new HttpClient();
            try
            {
                _ = 1;
                try
                {
                    ThumbnailResponse thumbnailResponse = JsonConvert.DeserializeObject<ThumbnailResponse>(await (await client.GetAsync("https://thumbnails.roblox.com/v1/users/avatar-headshot?userIds=1&size=48x48&format=Png&isCircular=true")).Content.ReadAsStringAsync());
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(thumbnailResponse.Data[0].imageUrl, UriKind.Absolute);
                    bitmapImage.EndInit();
                    PlayerIcon.Background = new ImageBrush
                    {
                        ImageSource = bitmapImage
                    };
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Thumbnail Grab Error");
                }
                finally
                {
                    ((HttpMessageInvoker)client).Dispose();
                }
            }
            finally
            {
                ((IDisposable)client)?.Dispose();
            }
        }

        private void ExecuteButton_Click(object sender, RoutedEventArgs e)
        {
            Roblox.ExecuteSpecific(new int[1] { ProcessID }, Script);
        }
    }
}
