using Zenject;

public class FactInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<FactPresenter>().AsTransient();
        Container.Bind<IFactView>().To<FactView>().FromComponentInHierarchy().AsTransient();
    }
}