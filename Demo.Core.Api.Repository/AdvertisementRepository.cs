using Demo.Core.Api.IRepository;
using System;

namespace Demo.Core.Api.Repository
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        public int Sum(int i, int j)
        {
            return i + j;
        }
    }
}
