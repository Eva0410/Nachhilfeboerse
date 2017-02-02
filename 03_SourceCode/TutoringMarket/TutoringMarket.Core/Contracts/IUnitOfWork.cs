﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Core.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        //TODO add further repositories

        IGenericRepository<Tutor> TutorRepository { get; }

        void Save();

        void DeleteDatabase();
    }
}
