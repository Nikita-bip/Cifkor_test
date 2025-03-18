using Zenject;

public class WeatherInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<string>()
            .WithId("WeatherApiUrl")
            .FromInstance("https://api.weather.gov/gridpoints/TOP/32,81/forecast");

        Container.Bind<WeatherPresenter>().AsTransient();
        Container.Bind<IWeatherView>().To<WeatherView>().FromComponentInHierarchy().AsTransient();
    }
}
