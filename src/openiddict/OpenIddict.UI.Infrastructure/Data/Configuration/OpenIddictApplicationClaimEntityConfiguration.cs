using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OpenIddict.UI.Infrastructure;

public class OpenIddictApplicationClaimConfiguration : IEntityTypeConfiguration<OpenIddictApplicationClaim>
{
  public void Configure(EntityTypeBuilder<OpenIddictApplicationClaim> builder)
  {
    // table
    builder.ToTable("OpenIddictApplicationClaim");

    // columns
    builder.HasKey(x => x.Id);
    builder.Property(x=>x.ApplicationId).IsRequired();
    builder.Property(x => x.ClaimType).IsRequired();
    builder.Property(x => x.ClaimValue).IsRequired();
  }
}
