using Prism.Mvvm;

namespace Models
{
    public class Role : BindableBase
    {
        private int id;
        private string roleName;

        public int Id 
        { 
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }
        public string RoleName 
        { 
            get => roleName;
            set
            {
                roleName = value;
                RaisePropertyChanged(nameof(RoleName));
            }
        }

        public Role()
        {
            Id = 0;
            RoleName = string.Empty;
        }

        public Role(int id, string roleName)
        {
            this.Id = id;
            RoleName = roleName;
        }

        public override string ToString() => $"{RoleName}";

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            obj = obj as Role;
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}