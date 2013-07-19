//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace BycicleShop.Common.SqlContext.Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class User
    {
        public User()
        {
            this.Baskets = new HashSet<Basket>();
            this.Orders = new HashSet<Order>();
        }
    
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string EmailAdress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Adress { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsApproved { get; set; }
        public int PasswordFailuresSinceLastSuccess { get; set; }
        public Nullable<System.DateTime> LastPasswordFailureDate { get; set; }
        public Nullable<System.DateTime> LastActivityDate { get; set; }
        public Nullable<System.DateTime> LastLockoutDate { get; set; }
        public Nullable<System.DateTime> LastLoginDate { get; set; }
        public string ConfirmationToken { get; set; }
        public Nullable<System.DateTime> CreateDate { get; set; }
        public bool IsLockedOut { get; set; }
        public Nullable<System.DateTime> LastPasswordChangedDate { get; set; }
        public string PasswordVerificationToken { get; set; }
        public Nullable<System.DateTime> PasswordVerificationTokenExpirationDate { get; set; }
    
        public virtual ICollection<Basket> Baskets { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
    }
}