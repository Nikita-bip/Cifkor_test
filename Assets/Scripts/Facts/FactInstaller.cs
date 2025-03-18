using Zenject;

public class FactInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        // ����������� Presenter
        Container.Bind<FactPresenter>().AsTransient();

        // ����������� View
        Container.Bind<IFactView>().To<FactView>().FromComponentInHierarchy().AsTransient();
    }
}
