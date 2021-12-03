using ASC.Business.Interface;
using ASC.Models.BaseTypes;
using ASC.Models.Models;
using ASC.Utilities;
using AutoMapper;
using Lap1.Controllers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Webb.Areas.Configuration.Models;
using Webb.Models;

namespace Webb.Areas.Configuration.Controllers
{

    //[Authorize(Roles = "Admin")]
    [Area("Configuration")]
    public class MasterDataController : BaseController
        {
            private readonly IMasterDataOperations _masterData;
            private readonly IMapper _mapper;
        public MasterDataController(IMasterDataOperations masterData, IMapper mapper)
            {
                _masterData = masterData;
            _mapper = mapper;
            }
        [HttpGet]
        public async Task<IActionResult> MasterKeys()
        {
            var masterKeys = await _masterData.GetAllMasterKeysAsync();
            var masterKeysViewModel = _mapper.Map <List<MasterDataKey>,List<MasterDataKeyViewModel>>(masterKeys);
            // Hold all Master Keys in session
            HttpContext.Session.SetSession("MasterKeys", masterKeysViewModel);
            return View(new MasterKeysViewModel
            {
                MasterKeys = masterKeysViewModel == null ? null : masterKeysViewModel.ToList(),
                IsEdit = false
            });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterKeys(MasterKeysViewModel masterKeyss)
        {
            masterKeyss.MasterKeys = HttpContext.Session.GetSession <List<MasterDataKeyViewModel>>("MasterKeys");
            if (!ModelState.IsValid)
            {
                return View(masterKeyss);
            }
            var masterKey = _mapper.Map<MasterDataKeyViewModel, MasterDataKey>(masterKeyss.MasterKeyInContext);
            if (masterKeyss.IsEdit)
            {
                // Update Master Key
                await _masterData.UpdateMasterKeyAsync(masterKeyss.MasterKeyInContext.
               PartitionKey, masterKey);
            }
            else
            {
                // Insert Master Key
                masterKey.RowKey = Guid.NewGuid().ToString();
                masterKey.PartitionKey = masterKey.Name;
                await _masterData.InsertMasterKeyAsync(masterKey);
            }
            return RedirectToAction("MasterKeys");
        }

        [HttpGet]
        public async Task<IActionResult> MasterValues()
        {
            // Get All Master Keys and hold them in ViewBag for Select tag
            ViewBag.MasterKeys = await _masterData.GetAllMasterKeysAsync();
            return View(new MasterValuesViewModel
            {
                MasterValues = new List<MasterDataValueViewModel>(),
                IsEdit = false
            });
        }
        [HttpGet]
        public async Task<IActionResult> MasterValuesByKey(string key)
        {
            // Get Master values based on master key.
            return Json(new { data = await _masterData.GetAllMasterValuesByKeyAsync(key) });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MasterValues(bool isEdit, MasterDataValueViewModel masterValue)
        {
            if (!ModelState.IsValid)
            {
                return Json("Error");
            }
            var masterDataValue = _mapper.Map<MasterDataValueViewModel,
           MasterDataValue>(masterValue);
            if (isEdit)
            {
                // Update Master Value
                await _masterData.UpdateMasterValueAsync(masterDataValue.PartitionKey,
               masterDataValue.RowKey, masterDataValue);
            }
            else
            {
                // Insert Master Value
                masterDataValue.RowKey = Guid.NewGuid().ToString();
                await _masterData.InsertMasterValueAsync(masterDataValue);
            }
            return Json(true);
        }

    }

    }
