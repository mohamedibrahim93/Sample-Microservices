using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ordering.Domain.Entities;

namespace Ordering.Infrastructure.EntityConfigurations;

class OrderEntityTypeConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> orderConfiguration)
    {
        orderConfiguration.ToTable("orders", "dbo");

        orderConfiguration.HasKey(o => o.Id);

        ////Address value object persisted as owned entity type supported since EF Core 2.0
        //orderConfiguration
        //    .OwnsOne(o => o.Address, a =>
        //    {
        //        // Explicit configuration of the shadow key property in the owned type 
        //        // as a workaround for a documented issue in EF Core 5: https://github.com/dotnet/efcore/issues/20740
        //        a.Property<int>("OrderId")
        //        .UseHiLo("orderseq", "dbo");
        //        a.WithOwner();
        //    });

        //orderConfiguration
        //    .Property<int?>("_buyerId")
        //    .UsePropertyAccessMode(PropertyAccessMode.Field)
        //    .HasColumnName("BuyerId")
        //    .IsRequired(false);

        //orderConfiguration
        //    .Property<DateTime>("_orderDate")
        //    .UsePropertyAccessMode(PropertyAccessMode.Field)
        //    .HasColumnName("OrderDate")
        //    .IsRequired();

        //orderConfiguration
        //    .Property<int>("_orderStatusId")
        //    // .HasField("_orderStatusId")
        //    .UsePropertyAccessMode(PropertyAccessMode.Field)
        //    .HasColumnName("OrderStatusId")
        //    .IsRequired();

        //orderConfiguration
        //    .Property<int?>("_paymentMethodId")
        //    .UsePropertyAccessMode(PropertyAccessMode.Field)
        //    .HasColumnName("PaymentMethodId")
        //    .IsRequired(false);

        //orderConfiguration.Property<string>("Description").IsRequired(false);

    }
}
