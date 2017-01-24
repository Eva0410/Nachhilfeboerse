using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TutoringMarket.Core.Contracts;
using TutoringMarket.Core.Enities;

namespace TutoringMarket.Core
{
    public class TutoringController
    {
        const string FILENAME = "TestTutors.csv";
        IUnitOfWork _unitOfWork;
        public TutoringController(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public void FillDatabaseFromCsv()
        {
            string[][] csvTutors = FILENAME.ReadStringMatrixFromCsv(true);

            List<Tutor> tutors = csvTutors.Select(l =>
            new Tutor()
            {
                Name = l[0]
            }).ToList();

            _unitOfWork.DeleteDatabase();
            //_unitOfWork.CategoryRepository.InsertMany(categories);
            _unitOfWork.Save();
        }
    }
}
