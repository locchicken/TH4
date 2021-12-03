using ASC.DataAccess.Interfaces;
using ASC.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ASC.Business.Interface
{
    public class MasterDataOperations : IMasterDataOperations
    {
        private readonly IUnitOfWork _unitOfWork;
        public MasterDataOperations(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<List<MasterDataKey>> GetAllMasterKeysAsync()
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataKey>().FindAllAsync();
            return masterKeys.ToList();
        }
        public async Task<List<MasterDataKey>> GetMaserKeyByNameAsync(string name)
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataKey>().FindAllByPartitionKeyAsync(name);
            return masterKeys.ToList();
        }
        public async Task<bool> InsertMasterKeyAsync(MasterDataKey key)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<MasterDataKey>().AddAsync(key);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValue>> GetAllMasterValuesByKeyAsync(string key)
        {
            var masterKeys = await _unitOfWork.Responitory<MasterDataValue>().FindAllByPartitionKeyAsync(key);
            return masterKeys.ToList();
        }
        public async Task<MasterDataValue> GetMasterValueByNameAsync(string key, string name)
        {
            var masterValues = await _unitOfWork.Responitory<MasterDataValue>().
           FindAsync(key, name);
            return masterValues;
        }
        public async Task<bool> InsertMasterValueAsync(MasterDataValue value)
        {
            using (_unitOfWork)
            {
                await _unitOfWork.Responitory<MasterDataValue>().AddAsync(value);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterKeyAsync(string orginalPartitionKey,
       MasterDataKey key)
        {
            using (_unitOfWork)
            {
                var masterKey = await _unitOfWork.Responitory<MasterDataKey>().
               FindAsync(orginalPartitionKey, key.RowKey);
                masterKey.IsActive = key.IsActive;
                masterKey.IsDeleted = key.IsDeleted;
                masterKey.Name = key.Name;
                await _unitOfWork.Responitory<MasterDataKey>().UpdateAsync(masterKey);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<bool> UpdateMasterValueAsync(string originalPartitionKey, string
       originalRowKey, MasterDataValue value)
        {
            using (_unitOfWork)
            {
                var masterValue = await _unitOfWork.Responitory<MasterDataValue>().
               FindAsync(originalPartitionKey, originalRowKey);
                masterValue.IsActive = value.IsActive;
                masterValue.IsDeleted = value.IsDeleted;
                masterValue.Name = value.Name;
                await _unitOfWork.Responitory<MasterDataValue>().UpdateAsync(masterValue);
                _unitOfWork.CommitTransaction();
                return true;
            }
        }
        public async Task<List<MasterDataValue>> GetAllMasterValuesAsync()
        {
            var masterValues = await _unitOfWork.Responitory<MasterDataValue>().
           FindAllAsync();
            return masterValues.ToList();
        }
    }
        }
