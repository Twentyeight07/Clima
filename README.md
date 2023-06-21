# Clima
### Which technologies did I use? 🔧

* Xamarin Forms
* Plugin.LocalNotifications
* **[WeatherAPI](https://www.weatherapi.com/)**
**************************************************
## What is Clima?
Clima is an app that I made for me, and more than a weather app (It works as a weather app as well) is a weather reminder.

You can search the city you want to know the current weather (or the forecast for the next day), and if you don't want to search you can just activate your location and see the local weather.

The app send a notification at 10PM (22 hrs) everyday to reminds you the Weather in a city (In this case because the app is for me and at the begining don't was intended to be for another person that 'city' is Caracas but is not hard to choice other, if you want to set another city Just edit that variable on the 'ScheduleNotification' function)

**************************************************
## To use this code
To make the app work you just need to register in [WeatherAPI](https://www.weatherapi.com/) get an API Key and put it on the 'GetWeather' function. Let's see step by step:

+ 1: Register in [WeatherAPI](https://www.weatherapi.com/)
+ 2: Get your API Key
+ 3: Go to *Clima -> ViewModel -> VMMainMenu*
+ 4: Search the 'GetWeather' function (line 175) and set your API Key on the apiKey variable

And that's prety much it, now you can generate and APK or make other changes in the code.

**************************************************

If you want to contact me with specific questions or for any reason, you can reach me at [eduardofreites1999@gmail.com](mailto:eduardofreites1999@gmail.com)