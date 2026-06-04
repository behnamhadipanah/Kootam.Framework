
using AutoMapper;
using AutoMapper.QueryableExtensions;
using IMapper = AutoMapper.IMapper;

namespace Kootam.Extensions.AutoMap.AutoMapper;

public class AutoMapperAdapter : Abstractions.IMapper
{
    private readonly IMapper _mapper;
    private readonly IConfigurationProvider _configuration;
    public AutoMapperAdapter(IMapper mapper)
    {
        _mapper = mapper;
        _configuration = mapper.ConfigurationProvider;
    }
    public TDestination Map<TDestination>(object source)
    {
        return _mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return _mapper.Map<TSource, TDestination>(source);
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
    {
        return source.ProjectTo<TDestination>(_configuration);
    }
}
