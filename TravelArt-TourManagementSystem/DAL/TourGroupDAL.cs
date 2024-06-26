﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using FireSharp;

namespace DAL
{
    public class TourGroupDAL
    {
        private DbUtils _db;
        private FirebaseClient _client;

        public async Task<List<TourGroupModel>> GetAllTourGroup()
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);

            var result = await _client.GetAsync("TourGroup");
            var list = result.ResultAs<List<TourGroupModel>>();
            
            if(list == null)
            {
                return new List<TourGroupModel>();
            }
            
            // find all tour group with id != -1
            var data = list.FindAll(x => x.Id != "-1");
            return data;
        }
        
        public async Task<TourGroupModel> GetTourGroupById(string id)
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);

            var result = await _client.GetAsync("TourGroup/" + id);
            var data = result.ResultAs<TourGroupModel>();
            return data;
        }
        
        public async Task<List<TourGroupModel>> GetTourGroupByCustomerId(string id)
        {
            var allTourGroup = await GetAllTourGroup();
            var tourGroupList = allTourGroup.Where(tourGroup => tourGroup.CustomerList.Contains(id)).ToList();
            return tourGroupList;
        }
        
        public async Task<List<TourGroupModel>> GetTourGroupByTourId(string id)
        {
            var today = DateTime.Now.Date;
            var allTourGroup = await GetAllTourGroup();
            return allTourGroup.Where(tourGroup => tourGroup.TourId == id && tourGroup.StartDate <= today).ToList();
        }

        public async Task<bool> RemoveTourismFromList(string tourGroupId, string customerId)
        {
            var tourGroup = await GetTourGroupById(tourGroupId);
            var customerList = tourGroup.CustomerList;
            var customerArray = customerList.Split(',');
            var newCustomerList = "";
            for (int i = 0; i < customerArray.Length; i++)
            {
                if (customerArray[i] != customerId)
                {
                    newCustomerList += customerArray[i] + ",";
                }
            }
            newCustomerList = newCustomerList.TrimEnd(',');
            
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            
            var result = await _client.UpdateAsync("TourGroup/" + tourGroupId, new { CustomerList = newCustomerList });
            var response = result.ResultAs<TourGroupModel>();
            return response != null;
        }
        
        public async Task<bool> AddTourismToList(string tourGroupId, string customerId)
        {
            var tourGroup = await GetTourGroupById(tourGroupId);
            var newCustomerList = "";
            if (!string.IsNullOrEmpty(tourGroup.CustomerList))
            {
                newCustomerList = tourGroup.CustomerList + "," + customerId;
            }
            else
            {
                newCustomerList = customerId;
            }

            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            
            var result = await _client.UpdateAsync("TourGroup/" + tourGroupId, new { CustomerList = newCustomerList });
            var response = result.ResultAs<TourGroupModel>();
            return response != null;
        }

        public async void DeleteTourGroup(string tourGroupId)
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            
            await _client.UpdateAsync("TourGroup/" + tourGroupId, new { Id = "-1" });
        }
        
        public async void PushTourGroup(TourGroupModel tourGroup)
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config); 
            await _client.SetAsync("TourGroup/" + tourGroup.Id, tourGroup);
        }
        
        public async Task<string> InitID()
        {
            var hotelList = await GetAllTourGroup();
            var id = 0;
            
            if (hotelList != null)
            {
                id = hotelList.Count;
            }
            
            return id.ToString();
        }
        
        public async Task<bool> CheckDuplicate(TourGroupModel tourGroup)
        {
            var tourGroupList = await GetAllTourGroup();
            var result = false;
            
            if (tourGroupList != null)
            {
                if (tourGroupList.Any(item => item.Name == tourGroup.Name || item.StartDate == tourGroup.StartDate || item.EndDate == tourGroup.EndDate || item.Slot == tourGroup.Slot))
                {
                    result = true;
                }
            }
            
            return result;
        }
    }
}