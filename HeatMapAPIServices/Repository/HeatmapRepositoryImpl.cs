using HeatMapAPIServices.EFModels;
using HeatMapAPIServices.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HeatMapAPIServices.Repository
{
    public class HeatmapRepositoryImpl : IHeatmapRepository
    {
        private readonly DBUTContext context = new DBUTContext();
        public bool createDataStore(SaveDataRequest data)
        {
            try
            {
                DataStore datastore = new DataStore();
                datastore.WebId = data.WebId;
                datastore.Type = data.Type;
                datastore.Data = data.Data;
                context.DataStore.Add(datastore);
                context.SaveChanges();
                return true;
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public bool deleteData(DeleteDataRequest request)
        {
            try
            {
                IEnumerable<DataStore> datas = context.DataStore.Where(s => s.WebId == request.webId)
                    .Where(s => s.Type == request.type).ToList();
                foreach (var data in datas)
                {
                    context.DataStore.Remove(data);
                }
                context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return false;
            }
        }

        public IEnumerable<DataStore> getData(GetDataRequest request)
        {
            try
            {
                IEnumerable<DataStore> data = context.DataStore.Where(s => s.WebId == request.webId)
                    .Where(s => s.Type == request.type).ToList();
                return data;
            }
            catch (Exception ex)
            {
                Console.WriteLine("ERROR: " + ex.Message);
                return null;
            }
        }
    }
}
