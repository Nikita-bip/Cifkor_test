using Zenject;

public class FactInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // Регистрация Presenter
        Container.Bind<FactPresenter>().AsTransient();

        // Регистрация View
        Container.Bind<IFactView>().To<FactView>().FromComponentInHierarchy().AsTransient();
    }
}
