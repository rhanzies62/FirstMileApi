//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace firstmile.domain
{
    using System;
    using System.Collections.Generic;
    
    public partial class FmUser : BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public FmUser()
        {
            this.FmCustomerUsers = new HashSet<FmCustomerUser>();
        }
    
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public int Type { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public Nullable<int> CountryId { get; set; }
        public Nullable<int> ProvinceId { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string PasswordRaw { get; set; }
        public bool IsPasswordChange { get; set; }
        public string FrameioToken { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FmCustomerUser> FmCustomerUsers { get; set; }
        public virtual FmProvince FmProvince { get; set; }
    }
}