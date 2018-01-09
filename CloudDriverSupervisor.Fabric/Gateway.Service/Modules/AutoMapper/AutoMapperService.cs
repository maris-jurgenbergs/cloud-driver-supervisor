namespace Gateway.Service.Modules.AutoMapper
{
    using System.Collections.Generic;
    using Common.Contracts.Transportation;
    using Controllers.Transportation.Entities;
    using global::AutoMapper;
    using Interfaces;

    public class AutoMapperService : IAutoMapperService
    {
        public AutoMapperService()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<IEnumerable<CapturedLocation>, List<CapturedLocationDto>>();
            });
        }

        public T MapObject<T>(object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}