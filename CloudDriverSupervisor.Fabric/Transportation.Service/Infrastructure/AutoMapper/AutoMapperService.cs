namespace Transportation.Service.Infrastructure.AutoMapper
{
    using System;
    using System.Collections.Generic;
    using Bootstrapper.Interfaces;
    using Common.Contracts.Transportation;
    using Domain.Entities;
    using global::AutoMapper;
    using Interfaces;

    public class AutoMapperService : IAutoMapperService, ISingletonService
    {
        public AutoMapperService()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<List<CapturedLocationDto>, List<CapturedLocation>>();
                cfg.CreateMap<CapturedLocationDto, CapturedLocation>().ForMember(
                    location => location.CapturedDateTimeUtc,
                    dto => dto.MapFrom(locationDto =>
                        locationDto.CapturedDateTimeUtc.Subtract(DateTime.MinValue.AddYears(1969)).TotalSeconds));
            });
        }

        public T MapObject<T>(object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}