using Kootam.Extensions.AutoMap.Abstractions;
using Mapster;
namespace Kootam.Extensions.AutoMap.Mapster;

public class MapsterMapperAdapter(MapsterMapper.IMapper mapper) : IMapper
{
    public TDestination Map<TDestination>(object source)
    {
        return mapper.Map<TDestination>(source);
    }

    public TDestination Map<TSource, TDestination>(TSource source)
    {
        return mapper.Map<TSource, TDestination>(source);
    }

    public IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source)
    {
        return source.ProjectToType<TDestination>();
    }
}
