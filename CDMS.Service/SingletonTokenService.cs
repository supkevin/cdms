using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Caching;
using CDMS.Model.Repository;
using CDMS.Model.UnitOfWork;

namespace CDMS.Service
{
    //https://mirkomaggioni.com/2016/10/15/register-a-singleton-service-with-autofac/
    public class SingletonTokenService : ITokenService
    {
        private Guid _token { get; set; }

        //private readonly IUnitOfWork _UnitOfWork;
        //private readonly IRepository<Model.Code> _CodeRepository;

        //public SingletonTokenService(IUnitOfWork unitofwork, IRepository<Model.Code> repository)
        //{
        //    this._UnitOfWork = unitofwork;
        //    this._CodeRepository = repository;
        //}

        public Guid GetToken()
        {
            if (_token == Guid.Empty)
                _token = Guid.NewGuid();

            return _token;
        }

        //private static MemoryCache _Cache = MemoryCache.Default;
        //private static int _CacheDuration = 600; //second 

        //private CacheItemPolicy CreateCacheItemPolicy()
        //{
        //    CacheItemPolicy policy = new CacheItemPolicy();
        //    policy.AbsoluteExpiration = DateTime.Now.AddMilliseconds(_CacheDuration);
        //    return policy;
        //}

        //public Dictionary<string, String> GetConditionKind()
        //{
        //    string key = "CONDITION_KIND";

        //    if (null == _Cache[key])
        //    {
        //        Dictionary<String, String> categories = new Dictionary<String, String>();
        //        CacheItemPolicy policy = CreateCacheItemPolicy();
                             
        //            this._CodeRepository.GetAll().Where(x => x.CodeType == key)
        //                .ToDictionary(x => x.CodeValue, x => x.CodeName);

        //        _Cache.Set(key, categories, policy);
        //    }

        //    return (Dictionary<string, String>)_Cache[key];
        //}
    }
}
