using Bindito.Core;

namespace Calloatti.HighlightTweaks
{
    [Context("Game")]
    internal class HighlightConfigurator : Configurator
    {
        protected override void Configure()
        {
            Bind<GoldenHighlightSpawner>().AsSingleton();
        }
    }
}
