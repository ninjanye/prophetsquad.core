using System;
using System.Threading.Tasks;

namespace ProphetSquad.Core.Mappers
{
    public interface IMapper<TSource, TDestination>
    {
        Task<TDestination> MapAsync(TSource source);
    }
}