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

namespace Clima.ViewModel
{
    internal class VMMainMenu : ViewModelBase
    {
        #region VARIABLES
        string _City;
        string _DateTime;
        string _Wallpaper;
        ObservableCollection<Mday> _Categories;
        ObservableCollection<MWeater> _Weater;
        #endregion
        #region CONSTRUCTOR
        public VMMainMenu(INavigation navigation)
        {
            Navigation = navigation;
            ListCategories();
            SetDateTime("Hoy");
            //GetLocation();
            ListWeater();
        }
        #endregion
        #region OBJETOS
        public ObservableCollection<Mday> Categories
        {
            get { return _Categories; }
            set { SetValue(ref _Categories, value); }
        }
        public ObservableCollection<MWeater> Weater
        {
            get { return _Weater; }
            set { SetValue(ref _Weater, value); }
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
        
        #endregion
        #region PROCESOS
        public async Task ProcesoAsyncrono()
        {

        }
        public void ListCategories()
        {
            var function = new Dday();
            Categories = function.ShowDays();
        }
        public void ListWeater()
        {
            var function = new DWeater();
            Weater = function.ShowWeater();
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
                    GetWeater(latitude, longitude);
                }
                else
                {
                    Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
                    GetWeater(latitude, longitude);
                }
            }
            catch(FeatureNotEnabledException)
            {
                if (day == "Mañana")
                {
                    Datetime = DateTime.Today.AddDays(1).ToString("dddd, d MMMM");
                    GetWeater("", "");
                }
                else
                {
                    Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
                    GetWeater("", "");
                }
            }
            catch (Exception ex)
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
                }
                else
                {
                    GetWeater("", "");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
        }

        private async Task<string> GetCityFromCoordinates(double latitude, double longitude)
        {
            var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
            var placemark = placemarks?.FirstOrDefault();

            if (placemark != null)
            {
                string city = placemark.Locality;
                return city;
            }
            else
            {
                return "Caracas";
            }

        }
        private async void GetWeater(string latitude, string longitude)
        {
            try
            {
                string location = latitude + "," + longitude;
                if(latitude == "" || longitude == "")
                {
                    location = "Caracas";
                }
                string apiKey = "96d613ae09f948fdbc1183533233105";
                int numberOfDays = 2;
                string aqi = "no";
                string alerts = "no";

                string apiUrl = string.Format("http://api.weatherapi.com/v1/forecast.json?key={0}&q={1}&days={2}&aqi={3}&alerts={4}", apiKey, location, numberOfDays, aqi, alerts);

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

                            // Utilizar los valores como desees
                            tCurrentTemp = avgTempC;
                            tMaxTemp = maxTempC;
                            tMinTemp = minTempC;
                            tChanceOfRain = tChancOfRain;
                        }
                    }

                    // Modificamos los campos de la DataWeater que ya creamos con el objeto 'Weater'
                    City = place;
                    if (Categories[1].Selected == true)
                    {
                        Weater[0].CurrentTemp = tCurrentTemp;
                        Weater[0].MaxTemp = tMaxTemp;
                        Weater[0].MinTemp = tMinTemp;
                        Weater[0].ChanceOfRain = tChanceOfRain;
                    }
                    else
                    {
                        Weater[0].CurrentTemp = currentTemp;
                        Weater[0].FeelsLike = feelsLike;
                        Weater[0].MaxTemp = maxTemp;
                        Weater[0].MinTemp = minTemp;
                        Weater[0].ChanceOfRain = chanceOfRain;
                    }

                    if (isDay == 1)
                    {
                        switch (condition)
                        {
                            case "Sunny":
                                Weater[0].Icon = "sun.png";
                                Wallpaper = "Soleado.jpg";
                                break;
                            case "Partly cloudy":
                                Weater[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Cloudy":
                                Weater[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Overcast":
                                Weater[0].Icon = "cloudy.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Mist":
                                Weater[0].Icon = "mist.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy rain possible":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy snow possible":
                                Weater[0].Icon = "snowyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy sleet possible":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy freezing drizzle possible":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Thundery outbreaks possible":
                                Weater[0].Icon = "storm.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Blowing snow":
                                Weater[0].Icon = "blowingSnow.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Blizzard":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Fog":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Freezing fog":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Patchy light drizzle":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Light drizzle":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Freezing drizzle":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy freezing drizzle":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Patchy light rain":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light rain":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate rain at times":
                                Weater[0].Icon = "rainyDay.png";
                                Wallpaper = "Nublado.jpg";
                                break;
                            case "Moderate rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy rain at times":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Heavy rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light freezing rain":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy freezing rain":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light sleet":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light snow":
                                Weater[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow":
                                Weater[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy moderate snow":
                                Weater[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate snow":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy heavy snow":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Heavy snow":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Ice pellets":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light rain shower":
                                Weater[0].Icon = "raining.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy rain shower":
                                Weater[0].Icon = "raining.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Torrential rain shower":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Light sleet showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow showers":
                                Weater[0].Icon = "snowyDay.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy snow showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light showers of ice pellets":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy showers of ice pellets":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light rain with thunder":
                                Weater[0].Icon = "storm.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            case "Moderate or heavy rain with thunder":
                                Weater[0].Icon = "storm.png";
                                Wallpaper = "Lluvia.jpg";
                                break;
                            default:
                                Weater[0].Icon = "sun.png";
                                Wallpaper = "Soleado.jpg";
                                break;
                        }
                    }
                    else if (isDay == 0)
                    {
                        switch (condition)
                        {
                            case "Clear":
                                Weater[0].Icon = "night.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Partly cloudy":
                                Weater[0].Icon = "cloudyNight.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Cloudy":
                                Weater[0].Icon = "cloudyNight.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Overcast":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Mist":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Patchy rain possible":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy snow possible":
                                Weater[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy sleet possible":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy freezing drizzle possible":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Thundery outbreaks possible":
                                Weater[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Blowing snow":
                                Weater[0].Icon = "blowing.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Blizzard":
                                Weater[0].Icon = "blizzard.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Fog":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Freezing fog":
                                Weater[0].Icon = "fog.png";
                                Wallpaper = "Noche.jpg";
                                break;
                            case "Patchy light drizzle":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light drizzle":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Freezing drizzle":
                                Weater[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy freezing drizzle":
                                Weater[0].Icon = "blowing.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy light rain":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light rain":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate rain at times":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy rain at times":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Heavy rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light freezing rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy freezing rain":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light sleet":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate or heavy sleet":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy light snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy moderate snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Moderate snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Patchy heavy snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Heavy snow":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Ice pellets":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "Snowy.jpg";
                                break;
                            case "Light rain shower":
                                Weater[0].Icon = "rainyNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy rain shower":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Torrential rain shower":
                                Weater[0].Icon = "heavyRain.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light sleet showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy sleet showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light snow showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy snow showers":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Light showers of ice pellets":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy showers of ice pellets":
                                Weater[0].Icon = "sleet.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Patchy light rain with thunder":
                                Weater[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            case "Moderate or heavy rain with thunder":
                                Weater[0].Icon = "thunderNight.png";
                                Wallpaper = "rainyNight.jpg";
                                break;
                            default:
                                Weater[0].Icon = "night.png";
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
            catch (Exception ex)
            {
                Console.WriteLine("Error al procesar la solicitud HTTP: " + ex.Message);
            }
        }

        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand Selectcommand => new Command<Mday>(Select);
        #endregion
    }
}
