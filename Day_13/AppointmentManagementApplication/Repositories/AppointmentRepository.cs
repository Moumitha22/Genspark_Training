using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AppointmentManagementApplication.Exceptions;
using AppointmentManagementApplication.Models;

namespace AppointmentManagementApplication.Repositories
{
    public class AppointmentRepository : Repository<int, Appointment>
    {
        public override int GenerateID()
        {
            if(_items.Count == 0)
            {
                return 101;
            }
            return _items.Max(i => i.Id) + 1;
        }

        public override ICollection<Appointment> GetAll()
        {
            if (_items.Count == 0)
            {
                throw new CollectionEmptyException("No appointments found");
            }
            return _items;
        }

        public override Appointment GetById(int id)
        {
            if (_items.Count == 0)
            {
                throw new CollectionEmptyException("No appointments found");
            }
            var appointment = _items.FirstOrDefault(i => i.Id == id);
            if (appointment == null)
            {
                throw new KeyNotFoundException("Appointment not found");
            }
            return appointment;
        }
    }
}
