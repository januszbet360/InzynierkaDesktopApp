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
    
    public partial class Match
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Match()
        {
            this.Scores = new HashSet<Score>();
        }
    
        public int Id { get; set; }
        public int HomeId { get; set; }
        public int AwayId { get; set; }
        public System.DateTime Date { get; set; }
        public Nullable<int> HomeGoalsPredicted { get; set; }
        public Nullable<int> AwayGoalsPredicted { get; set; }
        public string Season { get; set; }
        public int Matchweek { get; set; }
    
        public virtual Team Team { get; set; }
        public virtual Team Team1 { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Score> Scores { get; set; }
    }
}
