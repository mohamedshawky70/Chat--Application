using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApplication.API.EntitiesConfigs;

public class UploadFileConfig : IEntityTypeConfiguration<UploadFile>
{
	public void Configure(EntityTypeBuilder<UploadFile> builder)
	{
		builder.Property(x=>x.ContentType).HasMaxLength(50);
		builder.Property(x=>x.StoredFileName).HasMaxLength(250);
		builder.Property(x=>x.FileExtension).HasMaxLength(250);
		builder.Property(x=>x.FileName).HasMaxLength(250);
	}
}
