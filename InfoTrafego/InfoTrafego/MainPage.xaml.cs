using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Web.Syndication;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InfoTrafego {
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page {

        SyndicationFeed currentFeed = null;

        public MainPage() {
            this.InitializeComponent();
            GenerateInfo();
            SettingsPane.GetForCurrentView().CommandsRequested += SettingCharmManager_CommandsRequested;
        }

        private async void Button_Click(object sender, RoutedEventArgs e) {

            await GenerateInfo();


        }

        private async System.Threading.Tasks.Task GenerateInfo() {
            SyndicationClient client = new SyndicationClient();
            client.BypassCacheOnRetrieve = true;

            Uri uri = new Uri("http://services.sapo.pt/Traffic/GeoRSS");
            try {
                currentFeed = await client.RetrieveFeedAsync(uri);
            } catch (Exception) {
                var dialog = new Windows.UI.Popups.MessageDialog("Tem de estar conetado à internet para correr esta app");


                dialog.Commands.Add(new UICommand("Close", new UICommandInvokedHandler(this.CloseApp)));
                dialog.DefaultCommandIndex = 0;
                dialog.CancelCommandIndex = 1;
                dialog.ShowAsync();
            }

            if (searchBox.Text == "") {
                DisplayFeed(currentFeed);
            } else {
                DisplayFeed(currentFeed, searchBox.Text);
            }
        }

        private void CloseApp(IUICommand command) {
            Application.Current.Exit();
        }

        private void DisplayFeed(SyndicationFeed currentFeed, string p) {
            List<TrafficInfoItem> results = new List<TrafficInfoItem>();

            foreach (var item in currentFeed.Items) {
                if (item.Title.Text.Contains(p)) {
                    TrafficInfoItem newItem = new TrafficInfoItem();
                    newItem.Title = item.Title.Text;
                    newItem.Discription = item.Summary.Text;
                    newItem.Date = item.PublishedDate.ToString();
                    results.Add(newItem);
                }
            }

            grid.ItemsSource = results;
        }

        private void DisplayFeed(SyndicationFeed currentFeed) {

            List<TrafficInfoItem> results = new List<TrafficInfoItem>();

            foreach (var item in currentFeed.Items) {
                TrafficInfoItem newItem = new TrafficInfoItem();
                newItem.Title = item.Title.Text;
                newItem.Discription = item.Summary.Text;
                newItem.Date = item.PublishedDate.ToString();
                results.Add(newItem);
            }

            grid.ItemsSource = results;

        }

        class TrafficInfoItem {

            public string Title { get; set; }
            public string Date { get; set; }
            public string Discription { get; set; }
        }

        //3) Add my handler that shows the privacy text
        private void SettingCharmManager_CommandsRequested(SettingsPane sender, SettingsPaneCommandsRequestedEventArgs args) {
            args.Request.ApplicationCommands.Add(new SettingsCommand("privacypolicy", "Privacy policy", OpenPrivacyPolicy));
        }


        //4) Add OpenPrivacyPolicy method
        private async void OpenPrivacyPolicy(IUICommand command) {
            Uri uri = new Uri("https://dl.dropboxusercontent.com/u/12121505/privacyHor%C3%A1rios.html");
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }



}
