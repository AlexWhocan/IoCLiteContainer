namespace Simple_IoC_Container.Interfaces
{
    public interface IFromType
    {
        IToType Bind<T>();
        IFromType Register<TFrom, TTo>() where TTo : TFrom;
    }
}
