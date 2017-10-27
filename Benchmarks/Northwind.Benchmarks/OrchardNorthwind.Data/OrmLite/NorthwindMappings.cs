using OrchardNorthwind.Common.Entities;
using ServiceStack;
using ServiceStack.DataAnnotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrchardNorthwind.Data.OrmLite
{
    public class NorthwindMappings
    {

        /// <summary>
        /// 
        /// </summary>
        public static void InitAdminOrmLiteMapping()
        {
            #region IsFullMapAttribute_True


            var categoryType = typeof(Category);
            categoryType.AddAttributes(new Attribute[] { new AliasAttribute("Categories") });
            categoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            categoryType.GetProperty("CategoryId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("CategoryID") });
            categoryType.GetProperty("CategoryName").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(15), new RequiredAttribute() });


            var customerCustomerDemoType = typeof(CustomerCustomerDemo);
            customerCustomerDemoType.AddAttributes(new Attribute[] { new AliasAttribute("CustomerCustomerDemo"), new CompositeIndexAttribute(true, "CustomerId", "CustomerTypeId") });
            customerCustomerDemoType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            customerCustomerDemoType.GetProperty("CustomerId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Customer)), new AliasAttribute("CustomerID"), new RequiredAttribute() });
            customerCustomerDemoType.GetProperty("CustomerTypeId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(CustomerDemographic)), new AliasAttribute("CustomerTypeID"), new RequiredAttribute() });


            var customerDemographicType = typeof(CustomerDemographic);
            customerDemographicType.AddAttributes(new Attribute[] { new AliasAttribute("CustomerDemographics") });
            customerDemographicType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            customerDemographicType.GetProperty("CustomerTypeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AliasAttribute("CustomerTypeID"), new RequiredAttribute() });


            var customerType = typeof(Customer);
            customerType.AddAttributes(new Attribute[] { new AliasAttribute("Customers") });
            customerType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            customerType.GetProperty("CustomerId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AliasAttribute("CustomerID"), new RequiredAttribute() });
            customerType.GetProperty("CompanyName").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(40), new RequiredAttribute() });
            customerType.GetProperty("ContactName").AddAttributes(new Attribute[] { new StringLengthAttribute(30) });
            customerType.GetProperty("ContactTitle").AddAttributes(new Attribute[] { new StringLengthAttribute(30) });
            customerType.GetProperty("Address").AddAttributes(new Attribute[] { new StringLengthAttribute(60) });
            customerType.GetProperty("City").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(15) });
            customerType.GetProperty("Region").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(15) });
            customerType.GetProperty("PostalCode").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(10) });
            customerType.GetProperty("Country").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            customerType.GetProperty("Phone").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });
            customerType.GetProperty("Fax").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });


            var employeeType = typeof(Employee);
            employeeType.AddAttributes(new Attribute[] { new AliasAttribute("Employees") });
            employeeType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            employeeType.GetProperty("EmployeeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("EmployeeID") });
            employeeType.GetProperty("LastName").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(20), new RequiredAttribute() });
            employeeType.GetProperty("FirstName").AddAttributes(new Attribute[] { new StringLengthAttribute(10), new RequiredAttribute() });
            employeeType.GetProperty("Title").AddAttributes(new Attribute[] { new StringLengthAttribute(30) });
            employeeType.GetProperty("TitleOfCourtesy").AddAttributes(new Attribute[] { new StringLengthAttribute(25) });
            employeeType.GetProperty("Address").AddAttributes(new Attribute[] { new StringLengthAttribute(60) });
            employeeType.GetProperty("City").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            employeeType.GetProperty("Region").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            employeeType.GetProperty("PostalCode").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(10) });
            employeeType.GetProperty("Country").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            employeeType.GetProperty("HomePhone").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });
            employeeType.GetProperty("Extension").AddAttributes(new Attribute[] { new StringLengthAttribute(4) });
            employeeType.GetProperty("PhotoPath").AddAttributes(new Attribute[] { new StringLengthAttribute(255) });
            employeeType.GetProperty("ReportsTo").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Employee)) });


            var employeeTerritoryType = typeof(EmployeeTerritory);
            employeeTerritoryType.AddAttributes(new Attribute[] { new AliasAttribute("EmployeeTerritories"), new CompositeIndexAttribute(true, "EmployeeId", "TerritoryId") });
            employeeTerritoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            employeeTerritoryType.GetProperty("EmployeeId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Employee)), new AliasAttribute("EmployeeID"), new RequiredAttribute() });
            employeeTerritoryType.GetProperty("TerritoryId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Territory)), new AliasAttribute("TerritoryID"), new StringLengthAttribute(20), new RequiredAttribute() });


            var orderDetailType = typeof(OrderDetail);
            orderDetailType.AddAttributes(new Attribute[] { new AliasAttribute("Order Details"), new CompositeIndexAttribute(true, "OrderId", "ProductId") });
            orderDetailType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            orderDetailType.GetProperty("OrderId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Order)), new AliasAttribute("OrderID"), new RequiredAttribute() });
            orderDetailType.GetProperty("ProductId").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Product)), new AliasAttribute("ProductID"), new RequiredAttribute() });
            orderDetailType.GetProperty("UnitPrice").AddAttributes(new Attribute[] { new RequiredAttribute() });
            orderDetailType.GetProperty("Quantity").AddAttributes(new Attribute[] { new RequiredAttribute() });
            orderDetailType.GetProperty("Discount").AddAttributes(new Attribute[] { new RequiredAttribute() });


            var orderType = typeof(Order);
            orderType.AddAttributes(new Attribute[] { new AliasAttribute("Orders") });
            orderType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            orderType.GetProperty("OrderId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("OrderID") });
            orderType.GetProperty("OrderDate").AddAttributes(new Attribute[] { new IndexAttribute(false) });
            orderType.GetProperty("ShippedDate").AddAttributes(new Attribute[] { new IndexAttribute(false) });
            orderType.GetProperty("ShipName").AddAttributes(new Attribute[] { new StringLengthAttribute(40) });
            orderType.GetProperty("ShipAddress").AddAttributes(new Attribute[] { new StringLengthAttribute(60) });
            orderType.GetProperty("ShipCity").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            orderType.GetProperty("ShipRegion").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            orderType.GetProperty("ShipPostalCode").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(10) });
            orderType.GetProperty("ShipCountry").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            orderType.GetProperty("CustomerId").AddAttributes(new Attribute[] { new AliasAttribute("CustomerID"), new ForeignKeyAttribute(typeof(Customer)) });
            orderType.GetProperty("EmployeeId").AddAttributes(new Attribute[] { new AliasAttribute("EmployeeID"), new ForeignKeyAttribute(typeof(Employee)) });
            orderType.GetProperty("ShipVia").AddAttributes(new Attribute[] { new ForeignKeyAttribute(typeof(Shipper)) });


            var productType = typeof(Product);
            productType.AddAttributes(new Attribute[] { new AliasAttribute("Products") });
            productType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            productType.GetProperty("ProductId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("ProductID") });
            productType.GetProperty("ProductName").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(40), new RequiredAttribute() });
            productType.GetProperty("QuantityPerUnit").AddAttributes(new Attribute[] { new StringLengthAttribute(20) });
            productType.GetProperty("Discontinued").AddAttributes(new Attribute[] { new RequiredAttribute() });
            productType.GetProperty("CategoryId").AddAttributes(new Attribute[] { new AliasAttribute("CategoryID"), new ForeignKeyAttribute(typeof(Category)) });
            productType.GetProperty("SupplierId").AddAttributes(new Attribute[] { new AliasAttribute("SupplierID"), new ForeignKeyAttribute(typeof(Supplier)) });


            var regionType = typeof(Region);
            regionType.AddAttributes(new Attribute[] { new AliasAttribute("Region") });
            regionType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            regionType.GetProperty("RegionId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AliasAttribute("RegionID"), new RequiredAttribute() });
            regionType.GetProperty("RegionDescription").AddAttributes(new Attribute[] { new RequiredAttribute() });


            var shipperType = typeof(Shipper);
            shipperType.AddAttributes(new Attribute[] { new AliasAttribute("Shippers") });
            shipperType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            shipperType.GetProperty("ShipperId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("ShipperID") });
            shipperType.GetProperty("CompanyName").AddAttributes(new Attribute[] { new StringLengthAttribute(40), new RequiredAttribute() });
            shipperType.GetProperty("Phone").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });


            var supplierType = typeof(Supplier);
            supplierType.AddAttributes(new Attribute[] { new AliasAttribute("Suppliers") });
            supplierType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            supplierType.GetProperty("SupplierId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute(), new AliasAttribute("SupplierID") });
            supplierType.GetProperty("CompanyName").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(40), new RequiredAttribute() });
            supplierType.GetProperty("ContactName").AddAttributes(new Attribute[] { new StringLengthAttribute(30) });
            supplierType.GetProperty("ContactTitle").AddAttributes(new Attribute[] { new StringLengthAttribute(30) });
            supplierType.GetProperty("Address").AddAttributes(new Attribute[] { new StringLengthAttribute(60) });
            supplierType.GetProperty("City").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            supplierType.GetProperty("Region").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            supplierType.GetProperty("PostalCode").AddAttributes(new Attribute[] { new IndexAttribute(false), new StringLengthAttribute(10) });
            supplierType.GetProperty("Country").AddAttributes(new Attribute[] { new StringLengthAttribute(15) });
            supplierType.GetProperty("Phone").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });
            supplierType.GetProperty("Fax").AddAttributes(new Attribute[] { new StringLengthAttribute(24) });


            var territoryType = typeof(Territory);
            territoryType.AddAttributes(new Attribute[] { new AliasAttribute("Territories") });
            territoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            territoryType.GetProperty("TerritoryId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AliasAttribute("TerritoryID"), new StringLengthAttribute(20), new RequiredAttribute() });
            territoryType.GetProperty("TerritoryDescription").AddAttributes(new Attribute[] { new RequiredAttribute() });
            territoryType.GetProperty("RegionId").AddAttributes(new Attribute[] { new AliasAttribute("RegionID"), new ForeignKeyAttribute(typeof(Region)) });



            #endregion IsFullMapAttribute_True

            #region IsFullMapAttribute_False
            //var categoryType = typeof(Category);
            //categoryType.AddAttributes(new Attribute[] { new AliasAttribute("Categories") });
            //categoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //categoryType.GetProperty("CategoryId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var customerCustomerDemoType = typeof(CustomerCustomerDemo);
            //customerCustomerDemoType.AddAttributes(new Attribute[] { new AliasAttribute("CustomerCustomerDemo") });
            //customerCustomerDemoType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //customerCustomerDemoType.GetProperty("CustomerId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });
            //customerCustomerDemoType.GetProperty("CustomerTypeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var customerDemographicType = typeof(CustomerDemographic);
            //customerDemographicType.AddAttributes(new Attribute[] { new AliasAttribute("CustomerDemographics") });
            //customerDemographicType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //customerDemographicType.GetProperty("CustomerTypeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var customerType = typeof(Customer);
            //customerType.AddAttributes(new Attribute[] { new AliasAttribute("Customers") });
            //customerType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //customerType.GetProperty("CustomerId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var employeeType = typeof(Employee);
            //employeeType.AddAttributes(new Attribute[] { new AliasAttribute("Employees") });
            //employeeType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //employeeType.GetProperty("EmployeeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var employeeTerritoryType = typeof(EmployeeTerritory);
            //employeeTerritoryType.AddAttributes(new Attribute[] { new AliasAttribute("EmployeeTerritories") });
            //employeeTerritoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //employeeTerritoryType.GetProperty("EmployeeId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });
            //employeeTerritoryType.GetProperty("TerritoryId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var orderDetailType = typeof(OrderDetail);
            //orderDetailType.AddAttributes(new Attribute[] { new AliasAttribute("Order Details") });
            //orderDetailType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //orderDetailType.GetProperty("OrderId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });
            //orderDetailType.GetProperty("ProductId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var orderType = typeof(Order);
            //orderType.AddAttributes(new Attribute[] { new AliasAttribute("Orders") });
            //orderType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //orderType.GetProperty("OrderId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var productType = typeof(Product);
            //productType.AddAttributes(new Attribute[] { new AliasAttribute("Products") });
            //productType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //productType.GetProperty("ProductId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var regionType = typeof(Region);
            //regionType.AddAttributes(new Attribute[] { new AliasAttribute("Region") });
            //regionType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //regionType.GetProperty("RegionId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });

            //var shipperType = typeof(Shipper);
            //shipperType.AddAttributes(new Attribute[] { new AliasAttribute("Shippers") });
            //shipperType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //shipperType.GetProperty("ShipperId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var supplierType = typeof(Supplier);
            //supplierType.AddAttributes(new Attribute[] { new AliasAttribute("Suppliers") });
            //supplierType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //supplierType.GetProperty("SupplierId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute(), new AutoIncrementAttribute() });

            //var territoryType = typeof(Territory);
            //territoryType.AddAttributes(new Attribute[] { new AliasAttribute("Territories") });
            //territoryType.GetProperty("Id").AddAttributes(new Attribute[] { new IgnoreAttribute() });
            //territoryType.GetProperty("TerritoryId").AddAttributes(new Attribute[] { new PrimaryKeyAttribute() });
            #endregion IsFullMapAttribute_False

        }
    }
}
