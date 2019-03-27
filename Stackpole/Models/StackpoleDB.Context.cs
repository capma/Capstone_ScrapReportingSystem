﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Stackpole.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class StackpoleEntities : DbContext
    {
        public StackpoleEntities()
            : base("name=StackpoleEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Department> Departments { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Location> Locations { get; set; }
        public virtual DbSet<Machine> Machines { get; set; }
        public virtual DbSet<OpDept> OpDepts { get; set; }
        public virtual DbSet<OperationMachine> OperationMachines { get; set; }
        public virtual DbSet<Operation> Operations { get; set; }
        public virtual DbSet<OpScrapReason> OpScrapReasons { get; set; }
        public virtual DbSet<PartCustomer> PartCustomers { get; set; }
        public virtual DbSet<PartDepartment> PartDepartments { get; set; }
        public virtual DbSet<Part> Parts { get; set; }
        public virtual DbSet<PlantEmployee> PlantEmployees { get; set; }
        public virtual DbSet<Plant> Plants { get; set; }
        public virtual DbSet<ScrapDetail> ScrapDetails { get; set; }
        public virtual DbSet<ScrapReason> ScrapReasons { get; set; }
        public virtual DbSet<Scrap> Scraps { get; set; }
    }
}