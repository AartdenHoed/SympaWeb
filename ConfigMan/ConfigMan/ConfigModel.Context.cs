﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ConfigMan
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DbEntities : DbContext
    {
        public DbEntities()
            : base("name=DbEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Component> Components { get; set; }
        public virtual DbSet<Computer> Computers { get; set; }
        public virtual DbSet<Documentation> Documentations { get; set; }
        public virtual DbSet<ICON> ICONs { get; set; }
        public virtual DbSet<Installation> Installations { get; set; }
        public virtual DbSet<License> Licenses { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Vendor> Vendors { get; set; }
    }
}
