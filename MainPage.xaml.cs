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
        public string Token { get; set; }

        public MainPage()
        {
            InitializeComponent();
            Token = string.Empty;
        }

        private async void GetTokenClicked(object sender, EventArgs e)
        {
            await CrossFirebaseCloudMessaging.Current.CheckIfValidAsync();
            var token = await CrossFirebaseCloudMessaging.Current.GetTokenAsync();
            Console.WriteLine($"=================== FCM token: {token}");
            Token = token;
            await DisplayAlert("FCM token", token, "OK");
        }

        private async void SendPushClicked(object sender, EventArgs e)
        {
            Console.WriteLine("==== GETTING CREDENTIALS ====");
            var app = FirebaseApp.Create(new AppOptions
            {
                Credential = await GetCredential()
            });
            Console.WriteLine("==== GETTING MESSAGING ====");

            FirebaseMessaging messaging = FirebaseMessaging.GetMessaging(app);
            Console.WriteLine("==== MAKING MESSAGE ====");

            var message = new Message()
            {
                Token = Token,
                Notification = new Notification { Title = "Hello world!", Body = "It's a message for Android with MAUI" },
                Data = new Dictionary<string, string> { { "greeting", "hello" } },
                Android = new AndroidConfig { Priority = Priority.Normal },
                Apns = new ApnsConfig { Headers = new Dictionary<string, string> { { "apns-priority", "5" } } }
            };
            Console.WriteLine("==== SENDING MESSAGE ====");

            var response = await messaging.SendAsync(message);

            Console.WriteLine("==== DISPLAYING RESPONSE ====");

            await DisplayAlert("Response", response, "OK");
        }

        private async Task<GoogleCredential> GetCredential()
        {
            var path = await FileSystem.OpenAppPackageFileAsync("firebase-adminsdk.json");
            return GoogleCredential.FromStream(path);
        }

    }

}
