using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core;
using TutoringMarket.Persistence;

namespace TutoringMarket.FillDbTest
{
    class Program
    {
        static void Main(string[] args)
        {
            using (UnitOfWork uow = new UnitOfWork())
            {
                TutoringController ctrl = new TutoringController(uow);
                ctrl.FillDatabaseFromCsv();

                var tutors = uow.TutorRepository.Get();
                foreach (var tutor in tutors)
                {
                    Console.WriteLine(tutor.LastName);
                }
                Console.ReadKey();
            }
        }
    }
}
