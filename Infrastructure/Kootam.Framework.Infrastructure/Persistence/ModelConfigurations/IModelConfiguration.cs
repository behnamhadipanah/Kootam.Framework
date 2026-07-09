using Microsoft.EntityFrameworkCore;

namespace Kootam.Framework.Infrastructure.Persistence.ModelConfigurations;

public interface IModelConfiguration
{
  void Configure(ModelBuilder builder);

}