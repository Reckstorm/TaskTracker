using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Card : BindableBase
    {
        private int id;
        private string title;
        private string description;
        private User? assignee;
        private Status? status;
        private DateTime dateTimeCreated;
        private DateTime dateTimeModified;

        public int Id 
        { 
            get => id;
            set
            {
                id = value;
                RaisePropertyChanged(nameof(Id));
            }
        }
        public string Title 
        { 
            get => title;
            set
            {
                title = value;
                RaisePropertyChanged(nameof(Title));
            }
        }
        public string Description 
        { 
            get => description; 
            set 
            {
                description = value;
                RaisePropertyChanged(nameof(Description));
            } 
        }
        public User? Assignee 
        { 
            get => assignee;
            set
            {
                assignee = value;
                RaisePropertyChanged(nameof(Assignee));
            }
        }
        public Status? Status 
        { 
            get => status; 
            set 
            {
                status = value;
                RaisePropertyChanged(nameof(Status));
            } 
        }
        public DateTime DateTimeCreated 
        { 
            get => dateTimeCreated; 
            set 
            {
                dateTimeCreated = value;
                RaisePropertyChanged(nameof(DateTimeCreated));
            } 
        }
        public DateTime DateTimeModified 
        { 
            get => dateTimeModified; 
            set 
            {
                dateTimeModified = value;
                RaisePropertyChanged(nameof(DateTimeModified));
            } 
        }
        public Card() 
        { 
            Id = 0;
            Title = string.Empty;
            Description = string.Empty;
            Status = null;
            DateTimeCreated = DateTime.MinValue;
            DateTimeModified = DateTime.MinValue;
        }

        public Card(string title, string description, User assignee, Status status, DateTime dateTimeCreated, DateTime dateTimeModified)
        {
            Title = title;
            Description = description;
            Assignee = assignee;
            Status = status;
            DateTimeCreated = dateTimeCreated;
            DateTimeModified = dateTimeModified;
        }

        public override string ToString() => $"{Id}:{Title}:{Status}";

        public override int GetHashCode() => Id.GetHashCode();
        public override bool Equals(object? obj)
        {
            obj = obj as Card;
            if (obj == null) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return this.GetHashCode().Equals(obj.GetHashCode());
        }
    }
}
