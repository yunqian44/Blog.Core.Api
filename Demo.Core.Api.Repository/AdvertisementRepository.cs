using Demo.Core.Api.IRepository;
using Demo.Core.Api.Model.Entity;
using Demo.Core.Api.Model.Seed;
using SqlSugar;
using System;

namespace Demo.Core.Api.Repository
{
    public class AdvertisementRepository :BaseRepository<Advertisement>,
        IAdvertisementRepository
    {
        public int Sum(int i, int j)
        {
            return i + j;
        }
    }
}
