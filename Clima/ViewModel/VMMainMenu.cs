using Clima.Data;
using Clima.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xamarin.Essentials;
using System.Globalization;
using Plugin.LocalNotification;
using Plugin.LocalNotification.AndroidOption;
using Plugin.LocalNotification.EventArgs;

namespace Clima.ViewModel
{
    internal class VMMainMenu : ViewModelBase
    {
        #region VARIABLES
        string _City;
        string _CitySearched;
        string _DateTime;
        string _Wallpaper;
        int _TomorrowChanceOfRain;
        ObservableCollection<Mday> _Categories;
        ObservableCollection<MWeather> _Weather;
        #endregion
        #region CONSTRUCTOR
        public VMMainMenu(INavigation navigation)
        {
            Navigation = navigation;
            ListCategories();
            SetDateTime("Hoy");
            ListWeather();
        }


        #endregion
        #region OBJETOS
        public ObservableCollection<Mday> Categories
        {
            get { return _Categories; }
            set { SetValue(ref _Categories, value); }
        }
        public ObservableCollection<MWeather> Weather
        {
            get { return _Weather; }
            set { SetValue(ref _Weather, value); }
        }
        public string Datetime
        {
            get { return _DateTime; }
            set { SetValue(ref _DateTime, value); }
        }
        public string Wallpaper
        {
            get { return _Wallpaper; }
            set { SetValue(ref _Wallpaper, value); }
        }
        public string City
        {
            get { return _City; }
            set { SetValue(ref _City, value); }
        }
        public string CitySearched
        {
            get { return _CitySearched; }
            set { SetValue(ref _CitySearched, value); }
        } 
        public class NotificationPerms : Xamarin.Essentials.Permissions.BasePlatformPermission
        {
            public (string androidPermission, bool isRuntime)[] RequiredPermissions => new List<(string androidPermission, bool isRuntime)>
        {
        ("android.permission.POST_NOTIFICATIONS", true),
        }.ToArray();
        }
        #endregion
        #region PROCESOS
        public void ListCategories()
        {
            var function = new Dday();
            Categories = function.ShowDays();
        }
        public void ListWeather()
        {
            var function = new DWeather();
            Weather = function.ShowWeather();
        }
        private void Select(Mday param)
        {
            //We make a list from the days, and find an Index. Just compare the "day" from the 'Dday' and the "day" from the Label I just select
            var index = Categories.ToList().FindIndex(p => p.Day == param.Day);
            if (index > -1)
            {
                //Now we use the index from the var above, and change the properties to make the Label look like selected
                DeSelect();
                Categories[index].Selected = true;
                Categories[index].BackgroundColor = "#686868";
                SetDateTime(Categories[index].Day);
            }
        }
        private void DeSelect()
        {
            //We make a list from the Days list and change properties
            Categories.ForEach((item) =>
            {
                item.Selected = false;
                item.BackgroundColor = "Transparent";
            });
        }
        private async void SetDateTime(string day)
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                string latitude = location.Latitude.ToString().Replace(",", ".");
                string longitude = location.Longitude.ToString().Replace(",", ".");

                if (day == "Mañana")
                {
                    Datetime = DateTime.Today.AddDays(1).ToString("dddd, d MMMM");
                     await GetWeather(latitude, longitude, "");
                }
                else
                {
                    Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
                     await GetWeather(latitude, longitude, "");
                }
            }
            catch (FeatureNotEnabledException)
            {
                if (day == "Mañana")
                {
                    Datetime = DateTime.Today.AddDays(1).ToString("dddd, d MMMM");
                     await GetWeather("", "", "");
                }
                else
                {
                    Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
                     await GetWeather("", "", "");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        // La función 'GetLocation' y 'GetCityFromCoordinates' quedan pendientes por eliminar
        private async void GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.High);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    string latitude = location.Latitude.ToString().Replace(",", ".");
                    string longitude = location.Longitude.ToString().Replace(",", ".");

                     await GetWeather(latitude, longitude, "");
                }
                else
                {
                     await GetWeather("", "", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
        }

        private async Task GetWeather(string latitude, string longitude, string citySearched)
        {
            try
            {
                string location = latitude + "," + longitude;
                if (latitude == "" || longitude == "")
                {
                    if (citySearched == "")
                    {
                        location = "Caracas";
                    }
                    else
                    {
                        location = citySearched;
                    }
                }
                string apiKey = "96d613ae09f948fdbc1183533233105";
                int numberOfDays = 2;
                string aqi = "no";
                string alerts = "no";

                string apiUrl = string.Format("https://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&days={2}&aqi={3}&alerts={4}", apiKey, location, numberOfDays, aqi, alerts);

                var request = new HttpRequestMessage();
                request.RequestUri = new Uri(apiUrl);
                request.Method = HttpMethod.Get;
                request.Headers.Add("Accept", "application/json");
                var client = new HttpClient();
                HttpResponseMessage response = await client.SendAsync(request);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string content = await response.Content.ReadAsStringAsync();

                    // Deserializar el JSON en un objeto genérico para acceder a los campos deseados
                    var json = JObject.Parse(content);

                    // Obtener los campos específicos del JSON con el clima actual
                    var place = json["location"]["name"].ToString() + ", " + json["location"]["region"].ToString();
                    var currentTemp = json["current"]["temp_c"].ToString();
                    var feelsLike = json["current"]["feelslike_c"].ToString();
                    var maxTemp = json["forecast"]["forecastday"][0]["day"]["maxtemp_c"].ToString();
                    var minTemp = json["forecast"]["forecastday"][0]["day"]["mintemp_c"].ToString();
                    var chanceOfRain = json["forecast"]["forecastday"][0]["day"]["daily_chance_of_rain"].ToString();
                    var isDay = Convert.ToInt32(json["current"]["is_day"]);
                    var condition = json["current"]["condition"]["text"].ToString();
                    var tCurrentTemp = json["forecast"]["forecastday"][0]["day"]["avgtemp_c"].ToString();
                    var tMaxTemp = json["forecast"]["forecastday"][0]["day"]["maxtemp_c"].ToString();
                    var tMinTemp = json["forecast"]["forecastday"][0]["day"]["mintemp_c"].ToString();
                    var tChanceOfRain = json["forecast"]["forecastday"][0]["day"]["daily_chance_of_rain"].ToString();

                    // Buscamos el pronóstico de mañana con un foreach y hacemos coincidir la fecha del JSON con la fecha del día siguiente al actual
                    var forecastDays = json["forecast"]["forecastday"];
                    foreach (var forecastDay in forecastDays)
                    {
                        var forecastDayDate = forecastDay["date"].ToString();
                        var tomorrowDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd");

                        if (forecastDayDate == tomorrowDate)
                        {
                            var forecastDayData = forecastDay["day"];

                            // Acceder a los campos del pronóstico del día siguiente
                            var avgTempC = forecastDayData["avgtemp_c"].ToString();
                            var maxTempC = forecastDayData["maxtemp_c"].ToString();
                            var minTempC = forecastDayData["mintemp_c"].ToString();
                            var tChancOfRain = forecastDayData["daily_chance_of_rain"].ToString();

                            tCurrentTemp = avgTempC;
                            tMaxTemp = maxTempC;
                            tMinTemp = minTempC;
                            tChanceOfRain = tChancOfRain;
                            _TomorrowChanceOfRain = Convert.ToInt32(tChancOfRain);
                        }
                    }

                    // Modificamos los campos de la DataWeather que ya creamos con el objeto 'Weather'
                    City = place;
                    if (Categories[1].Selected == true)
                    {
                        Weather[0].CurrentTemp = tCurrentTemp;
                        Weather[0].MaxTemp = tMaxTemp;
                        Weather[0].MinTemp = tMinTemp;
                        Weather[0].ChanceOfRain = tChanceOfRain;
                    }
                    else
                    {
                        Weather[0].CurrentTemp = currentTemp;
                        Weather[0].FeelsLike = feelsLike;
                        Weather[0].MaxTemp = maxTemp;
                        Weather[0].MinTemp = minTemp;
                        Weather[0].ChanceOfRain = chanceOfRain;
                    }

                    if (isDay == 1)
                    {
                        switch (condition)
                        {
                            case "Sunny":
                                Weather[0].Icon = "sun.png";
                                Wallpaper = "Soleado.jpg";
                                break;
                            case "Partly cloudy":
                                Weather[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Cloudy":
                                Weather[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Overcast":
                                Weather[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Mist":
                                Weather[0].Icon = "mist.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy rain possible":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy snow possible":
                                Weather[0].Icon = "snowyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy sleet possible":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy freezing drizzle possible":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Thundery outbreaks possible":
                                Weather[0].Icon = "storm.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Blowing snow":
                                Weather[0].Icon = "blowingSnow.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Blizzard":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Fog":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Freezing fog":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy light drizzle":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Light drizzle":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Freezing drizzle":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy freezing drizzle":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Patchy light rain":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light rain":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate rain at times":
                                Weather[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Moderate rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy rain at times":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light freezing rain":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy freezing rain":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light sleet":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light snow":
                                Weather[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow":
                                Weather[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy moderate snow":
                                Weather[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate snow":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy heavy snow":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Heavy snow":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Ice pellets":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light rain shower":
                                Weather[0].Icon = "raining.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy rain shower":
                                Weather[0].Icon = "raining.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Torrential rain shower":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light sleet showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow showers":
                                Weather[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy snow showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light showers of ice pellets":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy showers of ice pellets":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light rain with thunder":
                                Weather[0].Icon = "storm.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy rain with thunder":
                                Weather[0].Icon = "storm.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            default:
                                Weather[0].Icon = "sun.png";
                                Wallpaper = "Soleado.jpg";
                                break;
                        }
                    }
                    else if (isDay == 0)
                    {
                        switch (condition)
                        {
                            case "Clear":
                                Weather[0].Icon = "night.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Partly cloudy":
                                Weather[0].Icon = "cloudyNight.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Cloudy":
                                Weather[0].Icon = "cloudyNight.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Overcast":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Mist":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Patchy rain possible":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy snow possible":
                                Weather[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy sleet possible":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy freezing drizzle possible":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Thundery outbreaks possible":
                                Weather[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Blowing snow":
                                Weather[0].Icon = "blowing.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Blizzard":
                                Weather[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Fog":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Freezing fog":
                                Weather[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Patchy light drizzle":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light drizzle":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Freezing drizzle":
                                Weather[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy freezing drizzle":
                                Weather[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy light rain":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light rain":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate rain at times":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy rain at times":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light freezing rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy freezing rain":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light sleet":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy moderate snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy heavy snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Heavy snow":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Ice pellets":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light rain shower":
                                Weather[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy rain shower":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Torrential rain shower":
                                Weather[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light sleet showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy sleet showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light snow showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy snow showers":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light showers of ice pellets":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy showers of ice pellets":
                                Weather[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy light rain with thunder":
                                Weather[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy rain with thunder":
                                Weather[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            default:
                                Weather[0].Icon = "night.png";
                                Wallpaper = "Noche.jpg";
                                break;
                        }
                    }


                }
                else
                {
                    Console.WriteLine("Error en la respuesta HTTP: " + response.StatusCode);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async void Search()
        {
             await GetWeather("", "", CitySearched);
        }
        public async void ScheduleNotification()
        {
            try
            {
                if (await LocalNotificationCenter.Current.AreNotificationsEnabled() == false)
                {
                    var status = await Permissions.RequestAsync<NotificationPerms>();
                    if (status != PermissionStatus.Granted)
                    {
                        await DisplayAlert("Permiso de notificaciones", "No se ha podido acceder al permiso de notificaciones por lo que no se podrán mostrar las mismas", "Ok");
                        return;
                    }
                }
                var tomorrowAt10PM = DateTime.Today.AddDays(1).Date.AddHours(22);
                var androidOptions = new AndroidOptions()
                {
                    IconLargeName = new AndroidIcon("mainIcon"),
                    IconSmallName = new AndroidIcon("mainIcon")
                };
                string description;
                await GetWeather("", "", "");
                if (_TomorrowChanceOfRain >= 50)
                {
                    description = "Hay posibilidades de lluvia para mañana en Caracas. Recuerda llevar un paraguas contigo";
                }
                else
                {
                    description = "Las probabilidades de lluvia para mañana en Caracas son bajas. No hay de que preocuparse";
                }
                
                var notification = new NotificationRequest
                {
                    BadgeNumber = 1,
                    Description = description,
                    Title = "Clima para mañana",
                    NotificationId = 28,
                    Schedule =
                {
                    NotifyTime = tomorrowAt10PM
                },
                    Android = androidOptions
                };

                await LocalNotificationCenter.Current.Show(notification);
            }
            catch (Exception ex)
            {

                Console.WriteLine("HA OCURRIDO UN ERROR!!!! Error: "+ex.Message+ ex.Source);
            }

        }


        #endregion
        #region COMANDOS
        public ICommand Searchcommand => new Command(Search);
        public ICommand Selectcommand => new Command<Mday>(Select);
        #endregion
    }
}
