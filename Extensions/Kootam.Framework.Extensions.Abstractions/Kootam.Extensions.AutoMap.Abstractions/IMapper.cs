namespace Kootam.Extensions.AutoMap.Abstractions;

public interface IMapper
{
    TDestination Map<TDestination>(object source);
    TDestination Map<TSource, TDestination>(TSource source);
    IQueryable<TDestination> ProjectTo<TDestination>(IQueryable source);

}
