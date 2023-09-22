using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Status : BindableBase
    {
        private int id;
        private string statusName;

        public int Id 
        { 
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }
        public string StatusName 
        { 
            get => statusName;
            set
            {
                statusName = value;
                RaisePropertyChanged(nameof(StatusName));
            }
        }

        public Status()
        {
            Id = 0;
            StatusName = string.Empty;
        }

        public Status(int id, string statusName)
        {
            this.Id = id;
            StatusName = statusName;
        }

        public override string ToString() => $"{StatusName}";

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            obj = obj as Status;
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
