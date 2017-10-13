using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel.Models
{
    public class TeamModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string FullName { get; set; }
        public string ImageUrl { get; set; }

        public TeamModel() { }

        public TeamModel(string name)
        {
            this.Name = name;
        }

        public Team ToDbObject()
        {
            return new Team { Name = this.Name };                        
        }
    }
}