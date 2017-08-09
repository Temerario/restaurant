using Microsoft.Practices.Unity;

namespace RestaurantBot.Utils
{
    public class InjectionContainer
    {
        private static InjectionContainer instance;
        private UnityContainer container;

        private InjectionContainer() { }

        public static InjectionContainer Instance => instance ?? (instance = new InjectionContainer());

        public UnityContainer Container => this.container ?? (this.container = new UnityContainer());
    }
}