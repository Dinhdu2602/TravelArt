﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DTO;
using FireSharp;

namespace DAL
{
    public class HotelDAL
    {
        private DbUtils _db;
        private FirebaseClient _client;

        public async Task<List<HotelModel>> GetAllHotel()
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            
            var result = await _client.GetAsync("Hotel");
            var data = result.ResultAs<List<HotelModel>>();

            return data;
        }
        
        public async Task<HotelModel> GetHotelById(string id)
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            
            var result = await _client.GetAsync("Hotel/" + id);
            var data = result.ResultAs<HotelModel>();

            return data;
        }
        
        public async void PushHotel(HotelModel hotel)
        {
            _db = new DbUtils();
            var config = _db.CreateConnection();
            _client = new FirebaseClient(config);
            await _client.SetAsync("Hotel/" + hotel.Id, hotel);
        }
        
        public async Task<List<HotelModel>> GetHotelByProvince(string province)
        {
            var hotelList = await GetAllHotel();
            var hotelByProvince = new List<HotelModel>();
            foreach (var item in hotelList)
            {
                if (item.Province == province)
                {
                    hotelByProvince.Add(item);
                }
            }
            return hotelByProvince;
        }
        
        public async Task<string> InitID()
        {
            var hotelList = await GetAllHotel();
            var id = 0;
            
            if (hotelList != null)
            {
                id = hotelList.Count;
            }
            
            return id.ToString();
        }
        
        public async Task<bool> CheckDuplicate(HotelModel hotel)
        {
            var hotelList = await GetAllHotel();
            var result = false;
            
            if (hotelList != null)
            {
                if (hotelList.Any(item => item.Name == hotel.Name || item.PhoneNumber == hotel.PhoneNumber || item.Price == hotel.Price ||
                                          item.Province == hotel.Province))
                {
                    result = true;
                }
            }
            
            return result;
        }
    }
}