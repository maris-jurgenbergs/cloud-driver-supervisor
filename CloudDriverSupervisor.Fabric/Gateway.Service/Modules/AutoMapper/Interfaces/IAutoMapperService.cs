namespace Gateway.Service.Modules.AutoMapper.Interfaces
{
    public interface IAutoMapperService
    {
        T MapObject<T>(object source);
    }
}