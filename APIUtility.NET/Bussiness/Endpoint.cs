﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.SqlClient;
using log4net;
using APIUtility.NET.Data;
using APIUtility.NET.Shared;

namespace APIUtility.NET.Bussiness
{
    public class Endpoint
    {
        private readonly ILog m_Logger = LogManager.GetLogger(typeof(Endpoint));
        private string m_DbConnectionString = string.Empty;

        public Endpoint(string dbConnectionString)
        {
            m_DbConnectionString = dbConnectionString;
        }

        public List<EndpointEntity> GetEndpointsByGuid(string userGuid, string parentGuid)
        {
            m_Logger.DebugFormat("__{0}__: {1}: Enter Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            List<EndpointEntity> endpoints = new List<EndpointEntity>();
            try
            {
                using (EndpointDatabaseUtility db = new EndpointDatabaseUtility(m_DbConnectionString))
                {
                    endpoints = db.QueryEndpointsByGuid(userGuid, parentGuid);
                }
            }
            catch (SqlException ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: SQL Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            catch (Exception ex)
            {
                m_Logger.ErrorFormat("__{0}__: {1}: Exception={2}", this.GetType().Name, MethodInfo.GetCurrentMethod().Name, ex.ToString());
                throw;
            }
            m_Logger.DebugFormat("__{0}__: {1}: Leave Function", this.GetType().Name, MethodInfo.GetCurrentMethod().Name);
            return endpoints;
        }
    }
}
