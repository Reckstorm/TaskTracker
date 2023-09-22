using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class User : BindableBase
    {
        private int id;
        private string name;
        private string email;
        private string password;
        private Role? role;

        public int Id 
        { 
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }
        public string Name 
        { 
            get => name;
            set
            {
                name = value;
                RaisePropertyChanged(nameof(Name));
            }
        }
        public string Email 
        {
            get => email; 
            set 
            {
                email = value;
                RaisePropertyChanged(nameof(Email));
            } 
        }
        public string Password 
        { 
            get => password; 
            set 
            {
                password = value;
                RaisePropertyChanged(nameof(Password));
            } 
        }
        public Role? Role 
        { 
            get => role; 
            set 
            {
                role = value;
                RaisePropertyChanged(nameof(Role));
            } 
        }
        public User() 
        { 
            Id = 0;
            Name = string.Empty;
            Email = string.Empty;
            Password = string.Empty;
            Role = null;
        }

        public User(int id, string name, string email, string password, Role role)
        {
            Id = id;
            Name = name;
            Email = email;
            Password = password;
            Role = role;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }

        public override bool Equals(object? obj)
        {
            obj = obj as User;
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
