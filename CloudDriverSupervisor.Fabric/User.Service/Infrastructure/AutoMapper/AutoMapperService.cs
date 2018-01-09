namespace User.Service.Infrastructure.AutoMapper
{
    using System.Collections.Generic;
    using System.Linq;
    using Bootstrapper.Interfaces;
    using Common.Contracts.User;
    using Domain.Entities;
    using global::AutoMapper;
    using Interfaces;

    public class AutoMapperService : IAutoMapperService, ISingletonService
    {
        public AutoMapperService()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<List<dynamic>, List<UserDto>>();
                cfg.CreateMap<dynamic, UserDto>()
                    .ForMember(dto => dto.Roles,
                        dyn => dyn.ResolveUsing(o =>
                            ((List<Role>) o.roles).Select(role => role.RoleType.ToString()).ToArray()))
                    .ForMember(dto => dto.AzureId, dyn => dyn.ResolveUsing(o => o.user.AzureId))
                    .ForMember(dto => dto.CreatedAt, dyn => dyn.ResolveUsing(o => o.user.CreatedAt))
                    .ForMember(dto => dto.Email, dyn => dyn.ResolveUsing(o => o.user.Email))
                    .ForMember(dto => dto.Name, dyn => dyn.ResolveUsing(o => o.user.Name))
                    .ForMember(dto => dto.Surname, dyn => dyn.ResolveUsing(o => o.user.Surname))
                    .ForMember(dto => dto.Phone, dyn => dyn.ResolveUsing(o => o.user.Phone));
            });
        }

        public T MapObject<T>(object source)
        {
            return Mapper.Map<T>(source);
        }
    }
}