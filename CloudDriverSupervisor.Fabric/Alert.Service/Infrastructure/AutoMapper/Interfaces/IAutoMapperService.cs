﻿namespace Alert.Service.Infrastructure.AutoMapper.Interfaces
{
    public interface IAutoMapperService
    {
        T MapObject<T>(object source);
    }
}