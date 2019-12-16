using Demo.Core.Api.Core.AttributeExtension;
using Demo.Core.Api.IRepository;
using Demo.Core.Api.IRepository.UnitOfWork;
using Demo.Core.Api.IServices;
using Demo.Core.Api.Model.Entity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Core.Api.Services
{
    public class GuestbookService : BaseService<Guestbook>, IGuestbookService
    {
        private readonly IGuestbookRepository _dal;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordLibRepository _passwordLibRepository;
        public GuestbookService(IUnitOfWork unitOfWork, IGuestbookRepository dal, IPasswordLibRepository passwordLibRepository)
        {
            this._dal = dal;
            base.BaseDal = dal;
            _unitOfWork = unitOfWork;
            _passwordLibRepository = passwordLibRepository;
        }

        [UseTran]
        public async Task<bool> TestTranInRepository()
        {
            try
            {
                Console.WriteLine($"");
                Console.WriteLine($"Begin Transaction");
                _unitOfWork.BeginTran();
                Console.WriteLine($"");


                var passwords = await _passwordLibRepository.Query();
                Console.WriteLine($"first time : the count of passwords is :{passwords.Count}");


                Console.WriteLine($"insert a data into the table PasswordLib now.");
                var insertPassword = await _passwordLibRepository.Add(new PasswordLib()
                {
                    IsDeleted = false,
                    AccountName = "aaa",
                    CreateTime = DateTime.Now
                });


                passwords = await _passwordLibRepository.Query(d => d.IsDeleted == false);
                Console.WriteLine($"second time : the count of passwords is :{passwords.Count}");

                //......

                Console.WriteLine($"");
                var guestbooks = await _dal.Query();
                Console.WriteLine($"first time : the count of guestbooks is :{guestbooks.Count}");

                int ex = 0;
                Console.WriteLine($"\nThere's an exception!!");
                int throwEx = 1 / ex;

                Console.WriteLine($"insert a data into the table Guestbook now.");
                var insertGuestbook = await _dal.Add(new Guestbook()
                {
                    UserName = "bbb",
                    BlogId = 1,
                    CreateTime = DateTime.Now,
                    IsShow = true
                });

                guestbooks = await _dal.Query();
                Console.WriteLine($"second time : the count of guestbooks is :{guestbooks.Count}");


                _unitOfWork.CommitTran();

                return true;
            }
            catch (Exception)
            {
                _unitOfWork.RollbackTran();
                var passwords = await _passwordLibRepository.Query();
                Console.WriteLine($"third time : the count of passwords is :{passwords.Count}");

                var guestbooks = await _dal.Query();
                Console.WriteLine($"third time : the count of guestbooks is :{guestbooks.Count}");

                return false;
            }
        }

        [UseTran]
        public async Task<bool> TestTranInRepositoryAOP()
        {
            var passwords = await _passwordLibRepository.Query();
            Console.WriteLine($"first time : the count of passwords is :{passwords.Count}");


            Console.WriteLine($"insert a data into the table PasswordLib now.");
            var insertPassword = await _passwordLibRepository.Add(new PasswordLib()
            {
                IsDeleted = false,
                AccountName = "aaa",
                CreateTime = DateTime.Now
            });


            passwords = await _passwordLibRepository.Query(d => d.IsDeleted == false);
            Console.WriteLine($"second time : the count of passwords is :{passwords.Count}");

            //......

            Console.WriteLine($"");
            var guestbooks = await _dal.Query();
            Console.WriteLine($"first time : the count of guestbooks is :{guestbooks.Count}");

            int ex = 0;
            Console.WriteLine($"\nThere's an exception!!");
            int throwEx = 1 / ex;

            Console.WriteLine($"insert a data into the table Guestbook now.");
            var insertGuestbook = await _dal.Add(new Guestbook()
            {
                UserName = "bbb",
                BlogId = 1,
                CreateTime = DateTime.Now,
                IsShow = true
            });

            guestbooks = await _dal.Query();
            Console.WriteLine($"second time : the count of guestbooks is :{guestbooks.Count}");

            return true;
        }

    }
}
