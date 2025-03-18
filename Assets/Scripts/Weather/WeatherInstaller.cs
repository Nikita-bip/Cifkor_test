using Zenject;

public class WeatherInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Привязка строки с идентификатором WeatherApiUrl
        Container.Bind<string>()
            .WithId("WeatherApiUrl")
            .FromInstance("https://api.weather.gov/gridpoints/TOP/32,81/forecast");

        // Привязка Presenter
        Container.Bind<WeatherPresenter>().AsTransient();

        // Привязка View
        Container.Bind<IWeatherView>().To<WeatherView>().FromComponentInHierarchy().AsTransient();
    }
}
