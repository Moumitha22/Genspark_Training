using System;
using ClinicManagementSystem.Models;

namespace ClinicManagementSystem.Interfaces
{

    public interface IEncryptionService
    {
        public Task<EncryptModel> EncryptData(EncryptModel data);
    }
}