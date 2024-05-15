using Firebase;
using Firebase.Messaging;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Plugin.Firebase.CloudMessaging;
using FirebaseApp = FirebaseAdmin.FirebaseApp;
using FirebaseMessaging = FirebaseAdmin.Messaging.FirebaseMessaging;

namespace PushMauiSample
{
    public partial class MainPage : ContentPage
    {
        public string Token { get; set; } = String.Empty;
        public int Count { get; set; } = 0;
        public MainPage()
        {
            InitializeComponent();
            Token = string.Empty;
        }

        private async void GetTokenClicked(object sender, EventArgs e)
        {
            Token = await GetTokenAsync();
            await DisplayAlert("FCM token", Token, "OK");

            // This token is generated for each device
            // Store this in a DB or something to send out notifications to devices
            // It expires after a month
            // Store with a timestamp
        }

        private async Task<string> GetTokenAsync()
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"FCM token: {token}");
            return token;
        }

        private async void SendPushClicked(object sender, EventArgs e)
        {
            var app = FirebaseApp.GetInstance("[DEFAULT]"); // Figure out how to set own names
            if (app == null)
            {
                app = FirebaseApp.Create(new AppOptions
                {
                    Credential = await GetCredential()
                });
            }

            if (String.IsNullOrWhiteSpace(Token))
            {
                Token = await GetTokenAsync();
            }

            FirebaseMessaging messaging = FirebaseMessaging.GetMessaging(app);

            var message = new Message()
            {
                Token = Token,
                Notification = new Notification { Title = "Hello world!", Body = $"This is message number {Count++}" },
                Data = new Dictionary<string, string> { { "greeting", "hello" } },
                Android = new AndroidConfig { Priority = Priority.Normal },
                Apns = new ApnsConfig { Headers = new Dictionary<string, string> { { "apns-priority", "5" } } }
            };

            var response = await messaging.SendAsync(message);

            if (response != null)
            {
                Console.WriteLine("Response: " + response.ToString());
                await DisplayAlert("Response", response, "OK");
            }
            else
            {
                await DisplayAlert("Response Fail", "Response Returned Null", "OK");
            }
        }

        private async Task<GoogleCredential> GetCredential()
        {
            var path = await FileSystem.OpenAppPackageFileAsync("firebase-adminsdk.json");
            return GoogleCredential.FromStream(path);
        }
    }

}
