using Zenject;

public class WeatherInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // �������� ������ � ��������������� WeatherApiUrl
        Container.Bind<string>()
            .WithId("WeatherApiUrl")
            .FromInstance("https://api.weather.gov/gridpoints/TOP/32,81/forecast");

        // �������� Presenter
        Container.Bind<WeatherPresenter>().AsTransient();

        // �������� View
        Container.Bind<IWeatherView>().To<WeatherView>().FromComponentInHierarchy().AsTransient();
    }
}
