//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace DataModel
{
    using System;
    using System.Collections.Generic;
    
    public partial class Players4Match
    {
        public int Id { get; set; }
        public int PlayerId { get; set; }
        public int MatchId { get; set; }
    
        public virtual Match Match { get; set; }
        public virtual Player Player { get; set; }
    }
}
