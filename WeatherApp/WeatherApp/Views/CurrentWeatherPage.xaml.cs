using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeatherApp.Helper;
using WeatherApp.Models;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace WeatherApp.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CurrentWeatherPage : ContentPage
    {
        public CurrentWeatherPage()
        {
            InitializeComponent();
            GetCoordinates();
        }
        private string Location { get; set; } = "Pretoria,ZA";
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        private async void GetCoordinates()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Best);
                var location= await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    Latitude = location.Latitude;
                    Longitude = location.Longitude;
                    Location = await GetCity(location);
                    GetWeatherInfo();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
            }
        }

        private async Task<string> GetCity(Location location)
        {
            var place = await Geocoding.GetPlacemarksAsync(location);
            var currentPlace = place?.FirstOrDefault();
            if (currentPlace != null)
                return $"{currentPlace.Locality},{currentPlace.CountryName}";
            return null;
        }

        private async void GetBackground()
        {
            var url = $"https://api.pexels.com/v1/search?query={Location}&per_page=15&page=1";

            var result = await APICaller.Get(url, "563492ad6f9170000100000130c6b4d94a77465e8f9d2911d4154cb8");
            if (result.successful)
            {
                var bgInfo= JsonConvert.DeserializeObject<BackgroundInfo>(result.Response);
                if (bgInfo != null && bgInfo.photos.Length>0)
                {
                    bgImg.Source = ImageSource.FromUri(new Uri(bgInfo.photos[new Random().Next(0, bgInfo.photos.Length-1)].src.medium));
                }
            }
        }


        private async void GetWeatherInfo()
        {
            var url = $"http://api.openweathermap.org/data/2.5/weather?q={Location}&APPID=fa89611d1c180f58495f26737870744a&units=metric";
            var result = await APICaller.Get(url);
            if (result.successful) 
            {
                try
                {
                    var weatherInfo = JsonConvert.DeserializeObject<WeatherInfo>(result.Response);
                    descriptionTxt.Text = weatherInfo.weather[0].description.ToUpper();
                    iconImg.Source = $"w{weatherInfo.weather[0].icon}";
                    cityTxt.Text = weatherInfo.name.ToUpper();
                    temperatureTxt.Text = weatherInfo.main.temp.ToString("0");
                    humidityTxt.Text = $"{weatherInfo.main.humidity}%";
                    pressureTxt.Text = $"{weatherInfo.main.pressure} hpa";
                    windTxt.Text = $"{weatherInfo.wind.speed} m/s";
                    cloudinessTxt.Text = $"{weatherInfo.clouds.all}%";

                    var dt = new DateTime(1970,1,1,0,0,0,DateTimeKind.Utc).ToUniversalTime().AddSeconds(weatherInfo.dt);
                    dateTxt.Text=dt.ToString("dddd, MMM dd").ToUpper();
                    GetForecast();
                    GetBackground();
                }
                catch(Exception ex)
                {
                    await DisplayAlert("Weather Info",ex.Message, "Ok");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No Weather Information Found", "Ok");
            }

        }
        private async void GetForecast()
        {
            var url = $"http://api.openweathermap.org/data/2.5/forecast?q={Location}&APPID=fa89611d1c180f58495f26737870744a&units=metric";
            var result = await APICaller.Get(url);
            if (result.successful)
            {
                try
                {
                    var forcastInfo = JsonConvert.DeserializeObject<ForecastInfo>(result.Response);

                    List<List> allList = new List<List>();

                    foreach (var list in forcastInfo.list)
                    {
                        //var date = DateTime.ParseExact(list.dt_txt, "yyyy-MM-dd hh:mm:ss", CultureInfo.InvariantCulture);
                        var date = DateTime.Parse(list.dt_txt);

                        if (date > DateTime.Now && date.Hour == 0 && date.Minute == 0 && date.Second == 0)
                            allList.Add(list);
                    }

                    dayOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dddd");
                    dateOneTxt.Text = DateTime.Parse(allList[0].dt_txt).ToString("dd MMM");
                    iconOneImg.Source = $"w{allList[0].weather[0].icon}";
                    tempOneTxt.Text = allList[0].main.temp.ToString("0");

                    dayTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dddd");
                    dateTwoTxt.Text = DateTime.Parse(allList[1].dt_txt).ToString("dd MMM");
                    iconTwoTxt.Source = $"w{allList[1].weather[0].icon}";
                    tempTwoTxt.Text = allList[1].main.temp.ToString("0");

                    dayThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dddd");
                    dateThreeTxt.Text = DateTime.Parse(allList[2].dt_txt).ToString("dd MMM");
                    iconThreeTxt.Source = $"w{allList[2].weather[0].icon}";
                    tempThreeTxt.Text = allList[2].main.temp.ToString("0");

                    dayFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dddd");
                    dateFourTxt.Text = DateTime.Parse(allList[3].dt_txt).ToString("dd MMM");
                    iconFourTxt.Source = $"w{allList[3].weather[0].icon}";
                    tempFourTxt.Text = allList[3].main.temp.ToString("0");

                }
                catch (Exception ex)
                {
                    await DisplayAlert("Weather Info", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Weather Info", "No forecast information found", "OK");
            }
        }
    }
}