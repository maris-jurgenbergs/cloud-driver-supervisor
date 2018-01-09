namespace Alert.Service.Infrastructure.AutoMapper
{
    using Bootstrapper.Interfaces;
    using Common.Contracts.Alert;
    using Domain.Entities;
    using global::AutoMapper;
    using Interfaces;

    public class AutoMapperService : IAutoMapperService, ISingletonService
    {
        public AutoMapperService()
        {
            Mapper.Initialize(cfg => { cfg.CreateMap<AlertDto, Alert>(); });
        }

        public T MapObject<T>(object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}