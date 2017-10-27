using Microsoft.EntityFrameworkCore;
using OrchardNorthwind.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardNorthwind.Data.EntityFrameworkCore
{
    /// <summary>
    /// 
    /// </summary>
    public class EfCoreTestDbContext : DbContext
    {
        public EfCoreTestDbContext(DbContextOptions options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var categoryEntity = modelBuilder.Entity<Category>();
            categoryEntity.ToTable("Categories");
            categoryEntity.Ignore(t => t.Id);
            categoryEntity.Property(t => t.CategoryId).ValueGeneratedOnAdd(); ;
            categoryEntity.HasKey(t => t.CategoryId);
            categoryEntity.Property(t => t.CategoryName).HasColumnName("CategoryName").IsRequired().HasMaxLength(15);
            categoryEntity.Property(t => t.Description).HasColumnName("Description");
            categoryEntity.Property(t => t.Picture).HasColumnName("Picture");


            var customerCustomerDemoEntity = modelBuilder.Entity<CustomerCustomerDemo>();
            customerCustomerDemoEntity.ToTable("CustomerCustomerDemo");
            customerCustomerDemoEntity.Ignore(t => t.Id);
            customerCustomerDemoEntity.HasKey(t => new { t.CustomerId, t.CustomerTypeId });
            customerCustomerDemoEntity.Property(t => t.CustomerId).HasColumnName("CustomerID").IsRequired();
            customerCustomerDemoEntity.Property(t => t.CustomerTypeId).HasColumnName("CustomerTypeID").IsRequired();


            var customerDemographicEntity = modelBuilder.Entity<CustomerDemographic>();
            customerDemographicEntity.ToTable("CustomerDemographics");
            customerDemographicEntity.Ignore(t => t.Id);
            customerDemographicEntity.HasKey(t => t.CustomerTypeId);
            customerDemographicEntity.Property(t => t.CustomerTypeId).HasColumnName("CustomerTypeID");
            customerDemographicEntity.Property(t => t.CustomerDesc).HasColumnName("CustomerDesc");


            var customerEntity = modelBuilder.Entity<Customer>();
            customerEntity.ToTable("Customers");
            customerEntity.Ignore(t => t.Id);
            customerEntity.HasKey(t => t.CustomerId);
            customerEntity.Property(t => t.CustomerId).HasColumnName("CustomerID");
            customerEntity.Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasMaxLength(40);
            customerEntity.Property(t => t.ContactName).HasColumnName("ContactName").HasMaxLength(30);
            customerEntity.Property(t => t.ContactTitle).HasColumnName("ContactTitle").HasMaxLength(30);
            customerEntity.Property(t => t.Address).HasColumnName("Address").HasMaxLength(60);
            customerEntity.Property(t => t.City).HasColumnName("City").HasMaxLength(15);
            customerEntity.Property(t => t.Region).HasColumnName("Region").HasMaxLength(15);
            customerEntity.Property(t => t.PostalCode).HasColumnName("PostalCode").HasMaxLength(10);
            customerEntity.Property(t => t.Country).HasColumnName("Country").HasMaxLength(15);
            customerEntity.Property(t => t.Phone).HasColumnName("Phone").HasMaxLength(24);
            customerEntity.Property(t => t.Fax).HasColumnName("Fax").HasMaxLength(24);
            customerEntity.Property(t => t.Picture).HasColumnName("Picture");


            var employeeEntity = modelBuilder.Entity<Employee>();
            employeeEntity.ToTable("Employees");
            employeeEntity.Ignore(t => t.Id);
            employeeEntity.Property(t => t.EmployeeId);
            employeeEntity.HasKey(t => t.EmployeeId);
            employeeEntity.Property(t => t.LastName).HasColumnName("LastName").IsRequired().HasMaxLength(20);
            employeeEntity.Property(t => t.FirstName).HasColumnName("FirstName").IsRequired().HasMaxLength(10);
            employeeEntity.Property(t => t.Title).HasColumnName("Title").HasMaxLength(30);
            employeeEntity.Property(t => t.TitleOfCourtesy).HasColumnName("TitleOfCourtesy").HasMaxLength(25);
            employeeEntity.Property(t => t.BirthDate).HasColumnName("BirthDate");
            employeeEntity.Property(t => t.HireDate).HasColumnName("HireDate");
            employeeEntity.Property(t => t.Address).HasColumnName("Address").HasMaxLength(60);
            employeeEntity.Property(t => t.City).HasColumnName("City").HasMaxLength(15);
            employeeEntity.Property(t => t.Region).HasColumnName("Region").HasMaxLength(15);
            employeeEntity.Property(t => t.PostalCode).HasColumnName("PostalCode").HasMaxLength(10);
            employeeEntity.Property(t => t.Country).HasColumnName("Country").HasMaxLength(15);
            employeeEntity.Property(t => t.HomePhone).HasColumnName("HomePhone").HasMaxLength(24);
            employeeEntity.Property(t => t.Extension).HasColumnName("Extension").HasMaxLength(4);
            employeeEntity.Property(t => t.Photo).HasColumnName("Photo");
            employeeEntity.Property(t => t.Notes).HasColumnName("Notes");
            employeeEntity.Property(t => t.PhotoPath).HasColumnName("PhotoPath").HasMaxLength(255);


            var employeeTerritoryEntity = modelBuilder.Entity<EmployeeTerritory>();
            employeeTerritoryEntity.ToTable("EmployeeTerritories");
            employeeTerritoryEntity.Ignore(t => t.Id);
            employeeTerritoryEntity.HasKey(t => new { t.EmployeeId, t.TerritoryId });
            employeeTerritoryEntity.Property(t => t.EmployeeId).HasColumnName("EmployeeID").IsRequired();
            employeeTerritoryEntity.Property(t => t.TerritoryId).HasColumnName("TerritoryID").IsRequired();


            var orderDetailEntity = modelBuilder.Entity<OrderDetail>();
            orderDetailEntity.ToTable("Order Details");
            orderDetailEntity.Ignore(t => t.Id);
            orderDetailEntity.HasKey(t => new { t.OrderId, t.ProductId });
            orderDetailEntity.Property(t => t.OrderId).HasColumnName("OrderID").IsRequired();
            orderDetailEntity.Property(t => t.ProductId).HasColumnName("ProductID").IsRequired();
            orderDetailEntity.Property(t => t.UnitPrice).HasColumnName("UnitPrice").IsRequired();
            orderDetailEntity.Property(t => t.Quantity).HasColumnName("Quantity").IsRequired();
            orderDetailEntity.Property(t => t.Discount).HasColumnName("Discount").IsRequired();


            var orderEntity = modelBuilder.Entity<Order>();
            orderEntity.ToTable("Orders");
            orderEntity.Ignore(t => t.Id);
            orderEntity.Property(t => t.OrderId);
            orderEntity.HasKey(t => t.OrderId);
            orderEntity.Property(t => t.OrderDate).HasColumnName("OrderDate");
            orderEntity.Property(t => t.RequiredDate).HasColumnName("RequiredDate");
            orderEntity.Property(t => t.ShippedDate).HasColumnName("ShippedDate");
            orderEntity.Property(t => t.Freight).HasColumnName("Freight");
            orderEntity.Property(t => t.ShipName).HasColumnName("ShipName").HasMaxLength(40);
            orderEntity.Property(t => t.ShipAddress).HasColumnName("ShipAddress").HasMaxLength(60);
            orderEntity.Property(t => t.ShipCity).HasColumnName("ShipCity").HasMaxLength(15);
            orderEntity.Property(t => t.ShipRegion).HasColumnName("ShipRegion").HasMaxLength(15);
            orderEntity.Property(t => t.ShipPostalCode).HasColumnName("ShipPostalCode").HasMaxLength(10);
            orderEntity.Property(t => t.ShipCountry).HasColumnName("ShipCountry").HasMaxLength(15);


            var productEntity = modelBuilder.Entity<Product>();
            productEntity.ToTable("Products");
            productEntity.Ignore(t => t.Id);
            productEntity.Property(t => t.ProductId);
            productEntity.HasKey(t => t.ProductId);
            productEntity.Property(t => t.ProductName).HasColumnName("ProductName").IsRequired().HasMaxLength(40);
            productEntity.Property(t => t.QuantityPerUnit).HasColumnName("QuantityPerUnit").HasMaxLength(20);
            productEntity.Property(t => t.UnitPrice).HasColumnName("UnitPrice");
            productEntity.Property(t => t.UnitsInStock).HasColumnName("UnitsInStock");
            productEntity.Property(t => t.UnitsOnOrder).HasColumnName("UnitsOnOrder");
            productEntity.Property(t => t.ReorderLevel).HasColumnName("ReorderLevel");
            productEntity.Property(t => t.Discontinued).HasColumnName("Discontinued").IsRequired();


            var regionEntity = modelBuilder.Entity<Region>();
            regionEntity.ToTable("Region");
            regionEntity.Ignore(t => t.Id);
            regionEntity.HasKey(t => t.RegionId);
            regionEntity.Property(t => t.RegionId).HasColumnName("RegionID");
            regionEntity.Property(t => t.RegionDescription).HasColumnName("RegionDescription").IsRequired().HasMaxLength(50);


            var shipperEntity = modelBuilder.Entity<Shipper>();
            shipperEntity.ToTable("Shippers");
            shipperEntity.Ignore(t => t.Id);
            shipperEntity.Property(t => t.ShipperId);
            shipperEntity.HasKey(t => t.ShipperId);
            shipperEntity.Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasMaxLength(40);
            shipperEntity.Property(t => t.Phone).HasColumnName("Phone").HasMaxLength(24);


            var supplierEntity = modelBuilder.Entity<Supplier>();
            supplierEntity.ToTable("Suppliers");
            supplierEntity.Ignore(t => t.Id);
            supplierEntity.Property(t => t.SupplierId);
            supplierEntity.HasKey(t => t.SupplierId);
            supplierEntity.Property(t => t.CompanyName).HasColumnName("CompanyName").IsRequired().HasMaxLength(40);
            supplierEntity.Property(t => t.ContactName).HasColumnName("ContactName").HasMaxLength(30);
            supplierEntity.Property(t => t.ContactTitle).HasColumnName("ContactTitle").HasMaxLength(30);
            supplierEntity.Property(t => t.Address).HasColumnName("Address").HasMaxLength(60);
            supplierEntity.Property(t => t.City).HasColumnName("City").HasMaxLength(15);
            supplierEntity.Property(t => t.Region).HasColumnName("Region").HasMaxLength(15);
            supplierEntity.Property(t => t.PostalCode).HasColumnName("PostalCode").HasMaxLength(10);
            supplierEntity.Property(t => t.Country).HasColumnName("Country").HasMaxLength(15);
            supplierEntity.Property(t => t.Phone).HasColumnName("Phone").HasMaxLength(24);
            supplierEntity.Property(t => t.Fax).HasColumnName("Fax").HasMaxLength(24);
            supplierEntity.Property(t => t.HomePage).HasColumnName("HomePage");


            var territoryEntity = modelBuilder.Entity<Territory>();
            territoryEntity.ToTable("Territories");
            territoryEntity.Ignore(t => t.Id);
            territoryEntity.HasKey(t => t.TerritoryId);
            territoryEntity.Property(t => t.TerritoryId).HasColumnName("TerritoryID");
            territoryEntity.Property(t => t.TerritoryDescription).HasColumnName("TerritoryDescription").IsRequired().HasMaxLength(50);



        }
    }
}
