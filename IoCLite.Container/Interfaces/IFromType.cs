namespace IoCLite.Container.Interfaces
{
    public interface IFromType
    {
        IToType Bind<T>();
        IFromType Register<TFrom, TTo>() where TTo : TFrom;
    }
}
