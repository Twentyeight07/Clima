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

namespace Clima.ViewModel
{
    internal class VMMainMenu : ViewModelBase
    {
        #region VARIABLES
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
            GetLocation();
            ListWeater();
            SetDateTime("Hoy");
            //SetWeater("Caracas");
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
        private void SetDateTime(string day)
        {
            if (day == "Mañana")
            {
                Datetime = DateTime.Today.AddDays(1).ToString("dddd, d MMMM");
            }
            else
            {
                Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
            }
        }
        private async void GetLocation()
        {
            try
            {
                var request = new GeolocationRequest(GeolocationAccuracy.Medium);
                var location = await Geolocation.GetLocationAsync(request);

                if (location != null)
                {
                    double latitude = location.Latitude;
                    double longitude = location.Longitude;

                    // Aquí puedes usar las coordenadas de latitud y longitud para obtener la ciudad
                    string city = await GetCityFromCoordinates(latitude, longitude);

                    // Llamar al método para establecer el clima utilizando la ciudad obtenida
                    SetWeater(city);
                }
                else
                {
                    SetWeater("Caracas");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }
        }

        private async Task<string> GetCityFromCoordinates(double latitude, double longitude)
        {
            try
            {
                var placemarks = await Geocoding.GetPlacemarksAsync(latitude, longitude);
                var placemark = placemarks?.FirstOrDefault();

                if (placemark != null)
                {
                    string city = placemark.Locality;
                    return city;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error" + ex.Message);
            }

            return string.Empty;
        }
        private async void SetWeater(string city)
        {
            try
            {
                string location = city == "" ? "Caracas" : city;
                string apiKey = "96d613ae09f948fdbc1183533233105";
                int numberOfDays = 1;
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

                    // Obtener los campos específicos del JSON
                    var currentTemp = json["current"]["temp_c"].ToString();
                    var feelsLike = json["current"]["feelslike_c"].ToString();
                    var maxTemp = json["forecast"]["forecastday"][0]["day"]["maxtemp_c"].ToString();
                    var minTemp = json["forecast"]["forecastday"][0]["day"]["mintemp_c"].ToString();
                    var chanceOfRain = json["forecast"]["forecastday"][0]["day"]["daily_chance_of_rain"].ToString();
                    var icon = json["current"]["condition"]["icon"].ToString();
                    var isDay = Convert.ToInt32(json["current"]["is_day"]);
                    var condition = json["current"]["condition"]["text"].ToString();

                    // Crear un objeto MWeater con los campos obtenidos
                    
                    Weater[0].CurrentTemp = currentTemp;
                    Weater[0].FeelsLike = feelsLike;
                    Weater[0].MaxTemp = maxTemp;
                    Weater[0].MinTemp = minTemp;
                    Weater[0].ChanceOfRain = chanceOfRain;
                    if(isDay == 1)
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
                    }else if(isDay == 0)
                    {
                        // Tengo que hacer que funcione el modo noche
                        switch (condition)
                        {
                            case "Clear":
                                // Código para el caso de "Clear"
                                break;
                            case "Partly cloudy":
                                // Código para el caso de "Partly cloudy"
                                break;
                            case "Cloudy":
                                // Código para el caso de "Cloudy"
                                break;
                            case "Overcast":
                                // Código para el caso de "Overcast"
                                break;
                            case "Mist":
                                // Código para el caso de "Mist"
                                break;
                            case "Patchy rain possible":
                                // Código para el caso de "Patchy rain possible"
                                break;
                            case "Patchy snow possible":
                                // Código para el caso de "Patchy snow possible"
                                break;
                            case "Patchy sleet possible":
                                // Código para el caso de "Patchy sleet possible"
                                break;
                            case "Patchy freezing drizzle possible":
                                // Código para el caso de "Patchy freezing drizzle possible"
                                break;
                            case "Thundery outbreaks possible":
                                // Código para el caso de "Thundery outbreaks possible"
                                break;
                            case "Blowing snow":
                                // Código para el caso de "Blowing snow"
                                break;
                            case "Blizzard":
                                // Código para el caso de "Blizzard"
                                break;
                            case "Fog":
                                // Código para el caso de "Fog"
                                break;
                            case "Freezing fog":
                                // Código para el caso de "Freezing fog"
                                break;
                            case "Patchy light drizzle":
                                // Código para el caso de "Patchy light drizzle"
                                break;
                            case "Light drizzle":
                                // Código para el caso de "Light drizzle"
                                break;
                            case "Freezing drizzle":
                                // Código para el caso de "Freezing drizzle"
                                break;
                            case "Heavy freezing drizzle":
                                // Código para el caso de "Heavy freezing drizzle"
                                break;
                            case "Patchy light rain":
                                // Código para el caso de "Patchy light rain"
                                break;
                            case "Light rain":
                                // Código para el caso de "Light rain"
                                break;
                            case "Moderate rain at times":
                                // Código para el caso de "Moderate rain at times"
                                break;
                            case "Moderate rain":
                                // Código para el caso de "Moderate rain"
                                break;
                            case "Heavy rain at times":
                                // Código para el caso de "Heavy rain at times"
                                break;
                            case "Heavy rain":
                                // Código para el caso de "Heavy rain"
                                break;
                            case "Light freezing rain":
                                // Código para el caso de "Light freezing rain"
                                break;
                            case "Moderate or heavy freezing rain":
                                // Código para el caso de "Moderate or heavy freezing rain"
                                break;
                            case "Light sleet":
                                // Código para el caso de "Light sleet"
                                break;
                            case "Moderate or heavy sleet":
                                // Código para el caso de "Moderate or heavy sleet"
                                break;
                            case "Patchy light snow":
                                // Código para el caso de "Patchy light snow"
                                break;
                            case "Light snow":
                                // Código para el caso de "Light snow"
                                break;
                            case "Patchy moderate snow":
                                // Código para el caso de "Patchy moderate snow"
                                break;
                            case "Moderate snow":
                                // Código para el caso de "Moderate snow"
                                break;
                            case "Patchy heavy snow":
                                // Código para el caso de "Patchy heavy snow"
                                break;
                            case "Heavy snow":
                                // Código para el caso de "Heavy snow"
                                break;
                            case "Ice pellets":
                                // Código para el caso de "Ice pellets"
                                break;
                            case "Light rain shower":
                                // Código para el caso de "Light rain shower"
                                break;
                            case "Moderate or heavy rain shower":
                                // Código para el caso de "Moderate or heavy rain shower"
                                break;
                            case "Torrential rain shower":
                                // Código para el caso de "Torrential rain shower"
                                break;
                            case "Light sleet showers":
                                // Código para el caso de "Light sleet showers"
                                break;
                            case "Moderate or heavy sleet showers":
                                // Código para el caso de "Moderate or heavy sleet showers"
                                break;
                            case "Light snow showers":
                                // Código para el caso de "Light snow showers"
                                break;
                            case "Moderate or heavy snow showers":
                                // Código para el caso de "Moderate or heavy snow showers"
                                break;
                            case "Light showers of ice pellets":
                                // Código para el caso de "Light showers of ice pellets"
                                break;
                            case "Moderate or heavy showers of ice pellets":
                                // Código para el caso de "Moderate or heavy showers of ice pellets"
                                break;
                            case "Patchy light rain with thunder":
                                // Código para el caso de "Patchy light rain with thunder"
                                break;
                            case "Moderate or heavy rain with thunder":
                                // Código para el caso de "Moderate or heavy rain with thunder"
                                break;
                            default:
                                // Código para el caso en que no coincida con ninguno de los valores anteriores
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
